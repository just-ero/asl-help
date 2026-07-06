using System;
using System.Buffers;
using System.Diagnostics;
using System.Text;

namespace AslHelp.Memory.Scanning;

/// <summary>
///     Represents a parsed array-of-bytes signature, with optional wildcard nibbles,
///     stored as parallel value/mask byte arrays for fast scanning.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct ScanPattern
{
    /// <summary>
    ///     The expected byte values of the pattern, one entry per byte. Wildcard nibbles are
    ///     zeroed, so a match is exactly <c>(data[i] &amp; Masks[i]) == Values[i]</c>.
    /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
    public byte[] Values { get; }
#pragma warning restore CA1819

    /// <summary>
    ///     The per-byte masks (<c>0xFF</c> fixed, <c>0x00</c> wildcard, <c>0xF0</c>/<c>0x0F</c>
    ///     for a single wildcard nibble), or <see langword="null"/> when every byte is fully
    ///     fixed (no wildcards).
    /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
    public byte[]? Masks { get; }
#pragma warning restore CA1819

    /// <summary>
    ///     The number of bytes in the pattern.
    /// </summary>
    public int ByteLength { get; }

    /// <summary>
    ///     The longest run of fully fixed bytes, as (byte offset, length); used as the search
    ///     anchor.
    /// </summary>
    internal (int Offset, int Length) Lead { get; }

    private ScanPattern(byte[] values, byte[]? masks, int byteLength, (int, int) lead)
    {
        Values = values;
        Masks = masks;
        ByteLength = byteLength;
        Lead = lead;
    }

    /// <summary>
    ///     Parses an array-of-bytes signature (e.g. <c>"48 8B ?? 05"</c>).
    /// </summary>
    /// <remarks>
    ///     Whitespace is ignored. Any non-hex character is a wildcard nibble; <c>"12 xx 56"</c>,
    ///     <c>"12 // 56"</c>, and <c>"12 .. 56"</c> are all equivalent to <c>"12 ?? 56"</c>.
    /// </remarks>
    /// <param name="signature">The signature to parse.</param>
    /// <returns>
    ///     The parsed pattern.
    /// </returns>
    /// <exception cref="FormatException">
    ///     <paramref name="signature"/> contains an odd number of non-whitespace characters.
    /// </exception>
    public static ScanPattern Parse(ReadOnlySpan<char> signature)
    {
        if (!TryParse(signature, out var pattern))
        {
            FormatException.Throw(
                "Signature must contain an even number of non-whitespace characters.");
        }

        return pattern;
    }

    /// <summary>
    ///     Attempts to parse an array-of-bytes signature (e.g. <c>"48 8B ?? 05"</c>).
    /// </summary>
    /// <remarks>
    ///     Whitespace is ignored. Any non-hex character is a wildcard nibble; <c>"12 xx 56"</c>,
    ///     <c>"12 // 56"</c>, and <c>"12 .. 56"</c> are all equivalent to <c>"12 ?? 56"</c>.
    /// </remarks>
    /// <param name="signature">The signature to parse.</param>
    /// <param name="pattern">
    ///     When this method returns <see langword="true"/>, the parsed pattern;
    ///     otherwise, <see langword="default"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="signature"/> was parsed successfully;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryParse(ReadOnlySpan<char> signature, out ScanPattern pattern)
    {
        char[]? rented = null;
        var buf = signature.Length <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(signature.Length));

        var len = 0;
        foreach (var c in signature)
        {
            if (c is ' ' or '\t' or '\n' or '\r')
            {
                continue;
            }

            buf[len++] = c;
        }

        if ((len & 1) != 0)
        {
            ArrayPool<char>.Shared.ReturnIfNotNull(rented);
            pattern = default;
            return false;
        }

        var count = len >> 1;
        var values = new byte[count];
        var masks = new byte[count];
        var hasMask = false;

        // Longest run of fully fixed (mask 0xFF) bytes; used as the scan anchor.
        int bestStart = 0, bestLen = 0, runStart = 0, runLen = 0;

        for (int i = 0, b = 0; b < count; i += 2, b++)
        {
            char hi = buf[i], lo = buf[i + 1];

            var v = (byte)((lookup(ValueTable, hi) << 4) | lookup(ValueTable, lo));
            var m = (byte)((lookup(MaskTable, hi) << 4) | lookup(MaskTable, lo));

            // Pre-mask the value so a match is exactly (data & mask) == value, with no
            // per-byte branching at scan time.
            v &= m;

            values[b] = v;
            masks[b] = m;

            if (m != 0xFF)
            {
                hasMask = true;
                runLen = 0;
            }
            else
            {
                if (runLen++ == 0)
                {
                    runStart = b;
                }

                if (runLen > bestLen)
                {
                    bestLen = runLen;
                    bestStart = runStart;
                }
            }
        }

        ArrayPool<char>.Shared.ReturnIfNotNull(rented);

        pattern = new(values, hasMask ? masks : null, count, (bestStart, bestLen));
        return true;

        static byte lookup(ReadOnlySpan<byte> table, char c)
        {
            return c < (uint)table.Length ? table[c] : (byte)0;
        }
    }

    /// <summary>
    ///     Returns the canonical signature string (hex digits with <c>?</c> wildcard nibbles).
    /// </summary>
    /// <returns>
    ///     The rendered signature, e.g. <c>"48 ?? 1? 05"</c>.
    /// </returns>
    public override string ToString()
    {
        const string Hex = "0123456789ABCDEF";

        var sb = new StringBuilder(ByteLength * 3);
        for (var i = 0; i < ByteLength; i++)
        {
            if (i > 0)
            {
                sb.Append(' ');
            }

            var value = Values[i];
            var mask = Masks?[i] ?? 0xFF;

            sb.Append((mask & 0xF0) == 0xF0 ? Hex[value >> 4] : '?');
            sb.Append((mask & 0x0F) == 0x0F ? Hex[value & 0xF] : '?');
        }

        return sb.ToString();
    }

    private static ReadOnlySpan<byte> ValueTable => [
        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 01,
        02, 03, 04, 05, 06, 07, 08, 09, 00, 00, 00, 00, 00, 00, 00, 10, 11, 12, 13, 14, 15, 00, 00, 00, 00,
        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 10, 11, 12,
        13, 14, 15, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
        00, 00, 00, 00, 00, 00
    ];

    private static ReadOnlySpan<byte> MaskTable => [
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x0F,
        0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x0F, 0x0F,
        0x0F, 0x0F, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00
    ];
}
