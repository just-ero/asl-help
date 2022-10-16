using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AslHelp.MemUtils.Reflect;

internal record EngineField
{
    private readonly int _trailingZeroCount;

    public EngineField(string name, string type, int offset, int size, int alignment, uint bitMask)
    {
        Name = name;
        Type = type;
        Offset = offset;
        Size = size;
        Alignment = alignment;

        if (bitMask != 0)
        {
            BitMask = bitMask;
            _trailingZeroCount = TrailingZeroCount(bitMask);
        }
    }

    public string Name { get; }
    public string Type { get; }
    public int Offset { get; }
    public int Size { get; }
    public int Alignment { get; }

    public uint BitMask { get; }

    public static implicit operator int(EngineField field)
    {
        return field.Offset;
    }

    public static byte operator &(byte value, EngineField field)
    {
        return (byte)((value & field.BitMask) >> field._trailingZeroCount);
    }

    public static sbyte operator &(sbyte value, EngineField field)
    {
        return (sbyte)((value & field.BitMask) >> field._trailingZeroCount);
    }

    public static ushort operator &(ushort value, EngineField field)
    {
        return (ushort)((value & field.BitMask) >> field._trailingZeroCount);
    }

    public static short operator &(short value, EngineField field)
    {
        return (short)((value & field.BitMask) >> field._trailingZeroCount);
    }

    public static uint operator &(uint value, EngineField field)
    {
        return (value & field.BitMask) >> field._trailingZeroCount;
    }

    public static int operator &(int value, EngineField field)
    {
        return (int)((value & field.BitMask) >> field._trailingZeroCount);
    }

    public static ulong operator &(ulong value, EngineField field)
    {
        return (value & field.BitMask) >> field._trailingZeroCount;
    }

    public static long operator &(long value, EngineField field)
    {
        return (value & field.BitMask) >> field._trailingZeroCount;
    }

    // https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.BitOperations.TrailingZeroCount
    private static int TrailingZeroCount(uint mask)
    {
        return Unsafe.AddByteOffset(
            ref MemoryMarshal.GetReference(_trailingZeroCountDeBruijn),
            (nint)(int)(((mask & (uint)-(int)mask) * 0x077CB531u) >> 27));
    }

    private static ReadOnlySpan<byte> _trailingZeroCountDeBruijn => new byte[32]
    {
        00, 01, 28, 02, 29, 14, 24, 03,
        30, 22, 20, 15, 25, 17, 04, 08,
        31, 27, 13, 23, 21, 19, 16, 07,
        26, 12, 18, 06, 11, 05, 10, 09
    };
}
