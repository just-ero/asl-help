using System;
using System.Buffers;
using System.Diagnostics;
using System.Text;

namespace AslHelp.Memory.Scanning;

/// <summary>
///     Represents a parsed array-of-bytes signature, with optional wildcard nibbles,
///     stored as packed little-endian words for fast scanning.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct ScanPattern
{
    /// <summary>
    ///     The packed byte values of the pattern, eight bytes per <see cref="ulong"/>,
    ///     stored in little-endian order within each word.
    /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
    public ulong[] Values { get; }
#pragma warning restore CA1819

    /// <summary>
    ///     The packed byte masks of the pattern, or <see langword="null"/> when every
    ///     byte is fully fixed (no wildcards).
    /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
    public ulong[]? Masks { get; }
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

    private ScanPattern(ulong[] values, ulong[]? masks, int byteLength, (int, int) lead)
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
        if (!TryParse(signature, out ScanPattern pattern))
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
        Span<char> buf = signature.Length <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(signature.Length));

        int len = 0;
        foreach (char c in signature)
        {
            if (c is ' ' or '\t' or '\n' or '\r')
            {
                continue;
            }

            buf[len++] = c;
        }

        buf = buf[..len];

        if ((len & 1) != 0)
        {
            pattern = default;
            return false;
        }

        int count = len >> 1;
        int words = (count + 7) / 8;

        var values = new ulong[words];
        var masks = new ulong[words];
        bool hasMask = false;

        // lead tracking
        int bestStart = 0, bestLen = 0, cur = 0, curStart = 0;

        for (int i = 0, b = 0; b < count; i += 2, b++)
        {
            char hi = buf[i], lo = buf[i + 1];
            byte v = (byte)((ValueTable[hi] << 4) | ValueTable[lo]);
            byte m = (byte)((MaskTable[hi] << 4) | MaskTable[lo]);

            int w = b / 8, shift = b % 8 * 8;
            values[w] |= (ulong)v << shift;
            masks[w] |= (ulong)m << shift;

            if (m != 0xFF)
            {
                hasMask = true;
                cur = 0;
            }
            else
            {
                if (cur++ == 0)
                {
                    curStart = b;
                }

                if (cur > bestLen)
                {
                    bestLen = cur;
                    bestStart = curStart;
                }
            }
        }

        ArrayPool<char>.Shared.ReturnIfNotNull(rented);

        pattern = new(values, hasMask ? masks : null, count, (bestStart, bestLen));
        return true;
    }

    internal byte GetValue(int index)
    {
        return (byte)(Values[index >> 3] >> ((index & 7) << 3));
    }

    internal byte GetMask(int index)
    {
        return Masks is { } masks
            ? (byte)(masks[index >> 3] >> ((index & 7) << 3))
            : (byte)0xFF;
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
        for (int i = 0; i < ByteLength; i++)
        {
            if (i > 0)
            {
                sb.Append(' ');
            }

            byte value = GetValue(i);
            byte mask = GetMask(i);

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
