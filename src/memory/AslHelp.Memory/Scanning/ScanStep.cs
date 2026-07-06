using System;
using System.Collections.Generic;

namespace AslHelp.Memory.Scanning;

/// <summary>
///     A single stage of an array-of-bytes scan: a signature to find, optionally within a window
///     relative to the previous stage's match, optionally followed by address transforms and a cap.
///     Build one with <see cref="For"/>, <see cref="Forward"/>, or <see cref="Backward"/> and run a
///     chain of them with <see cref="Scan"/>.
/// </summary>
public readonly struct ScanStep
{
    private enum Kind
    {
        Region,
        Forward,
        Backward,
    }

    private readonly Kind _kind;
    private readonly int _window;
    private readonly ScanPattern _pattern;
    private readonly Func<nint, Result<nint>>? _transform;
    private readonly int _cap;

    private ScanStep(Kind kind, int window, ScanPattern pattern, Func<nint, Result<nint>>? transform, int cap)
    {
        _kind = kind;
        _window = window;
        _pattern = pattern;
        _transform = transform;
        _cap = cap;
    }

    /// <summary>
    ///     A step that scans the whole region for <paramref name="signature"/>; use it to open a
    ///     <see cref="Scan"/> chain.
    /// </summary>
    /// <param name="signature">The signature to search for (e.g. <c>"48 8B ?? 05"</c>).</param>
    /// <returns>
    ///     The opening step.
    /// </returns>
    /// <exception cref="FormatException">
    ///     <paramref name="signature"/> has an odd number of non-whitespace characters.
    /// </exception>
    public static ScanStep For(string signature)
    {
        return new ScanStep(Kind.Region, 0, ScanPattern.Parse(signature), null, 0);
    }

    /// <summary>
    ///     Begins a step that scans <paramref name="window"/> bytes forward from the previous step's
    ///     match. Call <see cref="Window.For(string)"/> to supply the signature.
    /// </summary>
    /// <param name="window">The number of bytes to scan ahead of the anchor.</param>
    /// <returns>
    ///     A builder awaiting the signature to search the window for.
    /// </returns>
    public static Window Forward(int window)
    {
        return new Window(window, backward: false);
    }

    /// <summary>
    ///     Begins a step that scans <paramref name="window"/> bytes backward, ending at the previous
    ///     step's match. Call <see cref="Window.For(string)"/> to supply the signature.
    /// </summary>
    /// <param name="window">The number of bytes to scan behind the anchor.</param>
    /// <returns>
    ///     A builder awaiting the signature to search the window for.
    /// </returns>
    public static Window Backward(int window)
    {
        return new Window(window, backward: true);
    }

    internal static ScanStep Windowed(int window, bool backward, ScanPattern pattern)
    {
        return new ScanStep(backward ? Kind.Backward : Kind.Forward, window, pattern, null, 0);
    }

    /// <summary>
    ///     Maps each match through <paramref name="map"/> (e.g. <c>a =&gt; a + 1</c>), replacing it
    ///     with the projected address.
    /// </summary>
    /// <param name="map">The address projection.</param>
    /// <returns>
    ///     A step that applies <paramref name="map"/> after the scan.
    /// </returns>
    public ScanStep Transform(Func<nint, nint> map)
    {
        ArgumentNullException.ThrowIfNull(map);

        return Append(a => map(a));
    }

    /// <summary>
    ///     Maps each match through <paramref name="map"/> (e.g. a pointer deref); a match whose
    ///     projection fails is dropped.
    /// </summary>
    /// <param name="map">The fallible address projection.</param>
    /// <returns>
    ///     A step that applies <paramref name="map"/> after the scan, dropping failures.
    /// </returns>
    public ScanStep Transform(Func<nint, Result<nint>> map)
    {
        ArgumentNullException.ThrowIfNull(map);

        return Append(map);
    }

    private ScanStep Append(Func<nint, Result<nint>> map)
    {
        var existing = _transform;
        var composed = existing is null
            ? map
            : a => existing(a).AndThen(map);

        return new ScanStep(_kind, _window, _pattern, composed, _cap);
    }

    /// <summary>
    ///     Caps this step at its first surviving match per anchor.
    /// </summary>
    /// <returns>
    ///     The capped step.
    /// </returns>
    public Capped First()
    {
        return Take(1);
    }

    /// <summary>
    ///     Caps this step at its first <paramref name="count"/> surviving matches per anchor.
    /// </summary>
    /// <param name="count">The maximum number of matches to keep per anchor.</param>
    /// <returns>
    ///     The capped step.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="count"/> is not positive.
    /// </exception>
    public Capped Take(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        return new Capped(new ScanStep(_kind, _window, _pattern, _transform, count));
    }

    /// <summary>
    ///     Yields this step's surviving matches for a given <paramref name="anchor"/>: the raw scan,
    ///     each match projected through the transforms (dropping failures), capped per anchor.
    /// </summary>
    internal IEnumerable<nint> Evaluate(byte[] buffer, nint @base, nint anchor)
    {
        var produced = 0;
        foreach (var match in RawScan(buffer, @base, anchor))
        {
            nint addr;
            if (_transform is null)
            {
                addr = match;
            }
            else if (!_transform(match).TryUnwrap(out addr))
            {
                continue;
            }

            yield return addr;

            if (_cap > 0 && ++produced >= _cap)
            {
                yield break;
            }
        }
    }

    private IEnumerable<nint> RawScan(byte[] buffer, nint @base, nint anchor)
    {
        int start, length;
        if (_kind == Kind.Region)
        {
            start = 0;
            length = buffer.Length;
        }
        else
        {
            var cursor = (int)(anchor - @base);
            var from = _kind == Kind.Backward ? cursor - _window : cursor;
            var to = _kind == Kind.Backward ? cursor : cursor + _window;

            if (from < 0)
            {
                from = 0;
            }

            if (to > buffer.Length)
            {
                to = buffer.Length;
            }

            start = from;
            length = to - from;
        }

        if (length <= 0)
        {
            yield break;
        }

        foreach (var offset in Search(buffer, start, length, _pattern))
        {
            yield return @base + offset;
        }
    }

    // Searches buffer[from, from + count) for needle, yielding offsets relative to buffer[0].
    private static IEnumerable<int> Search(byte[] buffer, int from, int count, ScanPattern needle)
    {
        var length = needle.ByteLength;
        if (length == 0 || count < length)
        {
            yield break;
        }

        var values = needle.Values;
        var masks = needle.Masks;
        var (leadOffset, leadLength) = needle.Lead;

        // Last offset at which the whole pattern still fits inside the window.
        var limit = from + count - length;

        // No fully fixed byte to anchor on (every byte carries a wildcard nibble): there is
        // nothing for a vectorized search to lock onto, so verify every position.
        if (leadLength == 0)
        {
            for (var start = from; start <= limit; start++)
            {
                if (Matches(buffer, values, masks, start))
                {
                    yield return start;
                }
            }

            yield break;
        }

        // Anchor on the longest run of fixed bytes, located with a vectorized IndexOf, then
        // verify the full pattern (honouring wildcards) around each anchor hit.
        var maxLead = limit + leadOffset;
        var pos = from + leadOffset;
        while (pos <= maxLead)
        {
            int lead;
            {
                // Span scope kept off the yield path: a ref struct cannot live across a yield.
                ReadOnlySpan<byte> window = buffer.AsSpan(pos, maxLead - pos + leadLength);
                var index = window.IndexOf(values.AsSpan(leadOffset, leadLength));
                if (index < 0)
                {
                    yield break;
                }

                lead = pos + index;
            }

            var start = lead - leadOffset;
            if (Matches(buffer, values, masks, start))
            {
                yield return start;
            }

            pos = lead + 1;
        }
    }

    private static bool Matches(byte[] buffer, byte[] values, byte[]? masks, int start)
    {
        if (masks is null)
        {
            return buffer.AsSpan(start, values.Length).SequenceEqual(values);
        }

        for (var i = 0; i < values.Length; i++)
        {
            if ((byte)(buffer[start + i] & masks[i]) != values[i])
            {
                return false;
            }
        }

        return true;
    }
}

/// <summary>
///     A pending windowed scan; produced by <see cref="ScanStep.Forward(int)"/> or
///     <see cref="ScanStep.Backward(int)"/> and completed by <see cref="For(string)"/>.
/// </summary>
public readonly struct Window
{
    private readonly int _window;
    private readonly bool _backward;

    internal Window(int window, bool backward)
    {
        _window = window;
        _backward = backward;
    }

    /// <summary>
    ///     Scans the window for <paramref name="signature"/>, yielding every match in ascending
    ///     order — forward from the anchor, or backward ending at it.
    /// </summary>
    /// <param name="signature">The signature to search the window for.</param>
    /// <returns>
    ///     The windowed step.
    /// </returns>
    /// <exception cref="FormatException">
    ///     <paramref name="signature"/> has an odd number of non-whitespace characters.
    /// </exception>
    public ScanStep For(string signature)
    {
        return ScanStep.Windowed(_window, _backward, ScanPattern.Parse(signature));
    }
}

/// <summary>
///     A <see cref="ScanStep"/> whose output is capped; the terminal of a step chain, accepting no
///     further transforms. Converts implicitly to <see cref="ScanStep"/>.
/// </summary>
public readonly struct Capped
{
    private readonly ScanStep _step;

    internal Capped(ScanStep step)
    {
        _step = step;
    }

    /// <summary>
    ///     Unwraps the capped step.
    /// </summary>
    /// <param name="capped">The capped step.</param>
    public static implicit operator ScanStep(Capped capped)
    {
        return capped._step;
    }

    /// <summary>
    ///     Unwraps the capped step.
    /// </summary>
    /// <returns>
    ///     The underlying step.
    /// </returns>
    public ScanStep ToStep()
    {
        return _step;
    }
}
