using System.Buffers.Binary;

using AslHelp.Memory.Errors;
using AslHelp.Shared.Extensions;
using AslHelp.Shared.Results;

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

    public static Result<ScanPattern> Parse(params string[] pattern)
    {
        return Parse(0, pattern);
    }

    public static Result<ScanPattern> Parse(int scanOffset, params string[] pattern)
    {
        if (pattern.Length == 0)
        {
            return ScanPatternError.EmptyPattern;
        }

        string signature = pattern.Concat().RemoveWhiteSpace();
        if (signature.Length % 2 != 0)
        {
            return ScanPatternError.InvalidFormat;
        }

        int nibbles = signature.Length;
        int aligned = (nibbles + MaxNibble) & ~MaxNibble;
        int length = aligned / NibblesPerUlong;

        ulong[] values = new ulong[length];
        ulong[] masks = new ulong[length];

        ulong fullValue = 0, fullMask = 0;

        for (int i = 0, offset = 0; i < aligned; i++)
        {
            fullValue <<= BitsPerNibble;
            fullMask <<= BitsPerNibble;

            if (i < signature.Length)
            {
                byte value = signature[i].ToHexValue();

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

        return new ScanPattern()
        {
            Offset = scanOffset,
            Values = values,
            Masks = masks
        };
    }

    public static Result<ScanPattern> Parse(params byte[] pattern)
    {
        return Parse(0, pattern);
    }

    public static Result<ScanPattern> Parse(int scanOffset, params byte[] pattern)
    {
        if (pattern.Length == 0)
        {
            return ScanPatternError.EmptyPattern;
        }

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

        return new ScanPattern()
        {
            Offset = scanOffset,
            Values = values,
            Masks = masks
        };
    }

    public int Offset { get; private init; }

    public ulong[] Values { get; private init; }
    public ulong[] Masks { get; private init; }
}
