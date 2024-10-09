using System.Buffers.Binary;

using AslHelp.Shared;
using AslHelp.Shared.Extensions;

namespace AslHelp.Memory.Scanning;

public readonly struct ScanPattern
{
    private const byte ByteMask = 0b1111_1111;
    private const byte NibbleMask = 0b1111;

    private const int BitsPerByte = 8;
    private const int BitsPerNibble = BitsPerByte / 2;

    private const int BytesPerUlong = sizeof(ulong);
    private const int NibblesPerUlong = sizeof(ulong) * 2;

    private const int MaxByte = BytesPerUlong - 1;
    private const int MaxNibble = NibblesPerUlong - 1;

    public static ScanPattern Parse(string pattern)
    {
        return Parse(0, pattern);
    }

    public static ScanPattern Parse(int scanOffset, string pattern)
    {
        pattern = pattern.RemoveWhiteSpace();
        if (pattern.Length % 2 != 0)
        {
            const string Msg = "Pattern was not in the expected format. All bytes must be fully specified.";
            ThrowHelper.ThrowArgumentException(nameof(pattern), Msg);
        }

        return FromString(scanOffset, pattern);
    }

    public static ScanPattern Parse(byte[] pattern)
    {
        return Parse(0, pattern);
    }

    public static ScanPattern Parse(int scanOffset, byte[] pattern)
    {
        ThrowHelper.ThrowIfNullOrEmpty(pattern);

        return FromBytes(scanOffset, pattern);
    }

    public static bool TryParse(string pattern, out ScanPattern result)
    {
        return TryParse(0, pattern, out result);
    }

    public static bool TryParse(int scanOffset, string pattern, out ScanPattern result)
    {
        string signature = pattern.RemoveWhiteSpace();
        if (signature.Length == 0 || signature.Length % 2 != 0)
        {
            result = default;
            return false;
        }

        result = FromString(scanOffset, signature);
        return true;
    }

    public static bool TryParse(byte[] pattern, out ScanPattern result)
    {
        return TryParse(0, pattern, out result);
    }

    public static bool TryParse(int scanOffset, byte[] pattern, out ScanPattern result)
    {
        if (pattern.Length == 0)
        {
            result = default;
            return false;
        }

        result = FromBytes(scanOffset, pattern);
        return true;
    }

    public int Offset { get; init; }

    public required ulong[] Values { get; init; }
    public required ulong[] Masks { get; init; }

    private static ScanPattern FromString(int scanOffset, string pattern)
    {
        int nibbles = pattern.Length;
        int aligned = (nibbles + MaxNibble) & ~MaxNibble;
        int length = aligned / NibblesPerUlong;

        ulong[] values = new ulong[length];
        ulong[] masks = new ulong[length];

        ulong fullValue = 0, fullMask = 0;

        for (int i = 0, offset = 0; i < aligned; i++)
        {
            fullValue <<= BitsPerNibble;
            fullMask <<= BitsPerNibble;

            if (i < pattern.Length)
            {
                byte value = pattern[i].ToHexValue();

                if (value != CharExtensions.InvalidHexValue)
                {
                    fullValue |= value;
                    fullMask |= NibbleMask;
                }
            }

            if (i % NibblesPerUlong == MaxNibble)
            {
                values[offset] = BinaryPrimitives.ReverseEndianness(fullValue);
                masks[offset] = BinaryPrimitives.ReverseEndianness(fullMask);

                offset++;
            }
        }

        return new()
        {
            Offset = scanOffset,
            Values = values,
            Masks = masks
        };
    }

    private static ScanPattern FromBytes(int scanOffset, byte[] pattern)
    {
        int bytes = (pattern.Length + MaxByte) & ~MaxByte;
        int length = bytes / BytesPerUlong;

        ulong[] values = new ulong[length];
        ulong[] masks = new ulong[length];

        ulong fullValue = 0, fullMask = 0;

        for (int i = 0, offset = 0; i < bytes; i++)
        {
            fullValue <<= BitsPerByte;
            fullMask <<= BitsPerByte;

            if (i < pattern.Length)
            {
                fullValue |= pattern[i];
                fullMask |= ByteMask;
            }

            if (i % BytesPerUlong == MaxByte)
            {
                values[offset] = BinaryPrimitives.ReverseEndianness(fullValue);
                masks[offset] = BinaryPrimitives.ReverseEndianness(fullMask);

                offset++;
            }
        }

        return new()
        {
            Offset = scanOffset,
            Values = values,
            Masks = masks
        };
    }
}
