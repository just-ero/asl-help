using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace AslHelp.Memory.NativeStructs;

public sealed class NativeField
{
    private readonly int _trailingZeroes;

    internal NativeField(string type, string name, uint offset, uint size, uint alignment, (uint, uint)? bitfield)
    {
        Type = type;
        Name = name;
        Offset = offset;
        Size = size;
        Alignment = alignment;

        if (bitfield is (uint bfSize, uint bfOffset))
        {
            BitMask = ((1u << (int)bfSize) - 1) << (int)bfOffset;
            _trailingZeroes = TrailingZeroCount(BitMask);
        }
    }

    public static byte operator +(byte value, NativeField field)
    {
        return checked((byte)(value + field.Offset));
    }

    public static sbyte operator +(sbyte value, NativeField field)
    {
        return checked((sbyte)(value + field.Offset));
    }

    public static ushort operator +(ushort value, NativeField field)
    {
        return checked((ushort)(value + field.Offset));
    }

    public static short operator +(short value, NativeField field)
    {
        return checked((short)(value + field.Offset));
    }

    public static uint operator +(uint value, NativeField field)
    {
        return checked(value + field.Offset);
    }

    public static int operator +(int value, NativeField field)
    {
        return checked((int)(value + field.Offset));
    }

    public static ulong operator +(ulong value, NativeField field)
    {
        return checked(value + field.Offset);
    }

    public static long operator +(long value, NativeField field)
    {
        return checked(value + field.Offset);
    }

    public static nuint operator +(nuint value, NativeField field)
    {
        return checked(value + field.Offset);
    }

    public static nint operator +(nint value, NativeField field)
    {
        return checked((nint)(value + field.Offset));
    }

    public static byte operator -(byte value, NativeField field)
    {
        return checked((byte)(value - field.Offset));
    }

    public static sbyte operator -(sbyte value, NativeField field)
    {
        return checked((sbyte)(value - field.Offset));
    }

    public static ushort operator -(ushort value, NativeField field)
    {
        return checked((ushort)(value - field.Offset));
    }

    public static short operator -(short value, NativeField field)
    {
        return checked((short)(value - field.Offset));
    }

    public static uint operator -(uint value, NativeField field)
    {
        return checked(value - field.Offset);
    }

    public static int operator -(int value, NativeField field)
    {
        return checked((int)(value - field.Offset));
    }

    public static ulong operator -(ulong value, NativeField field)
    {
        return checked(value - field.Offset);
    }

    public static long operator -(long value, NativeField field)
    {
        return checked(value - field.Offset);
    }

    public static nuint operator -(nuint value, NativeField field)
    {
        return checked(value - field.Offset);
    }

    public static nint operator -(nint value, NativeField field)
    {
        return checked((nint)(value - field.Offset));
    }

    public static byte operator &(byte value, NativeField field)
    {
        return checked((byte)((value & field.BitMask) >> field._trailingZeroes));
    }

    public static sbyte operator &(sbyte value, NativeField field)
    {
        return checked((sbyte)((value & field.BitMask) >> field._trailingZeroes));
    }

    public static ushort operator &(ushort value, NativeField field)
    {
        return checked((ushort)((value & field.BitMask) >> field._trailingZeroes));
    }

    public static short operator &(short value, NativeField field)
    {
        return checked((short)((value & field.BitMask) >> field._trailingZeroes));
    }

    public static uint operator &(uint value, NativeField field)
    {
        return checked((value & field.BitMask) >> field._trailingZeroes);
    }

    public static int operator &(int value, NativeField field)
    {
        return checked((int)((value & field.BitMask) >> field._trailingZeroes));
    }

    public static ulong operator &(ulong value, NativeField field)
    {
        return checked((value & field.BitMask) >> field._trailingZeroes);
    }

    public static long operator &(long value, NativeField field)
    {
        return checked((value & field.BitMask) >> field._trailingZeroes);
    }

    public string Type { get; }
    public string Name { get; }
    public uint Offset { get; }
    public uint Size { get; }
    public uint Alignment { get; }

    public uint BitMask { get; }
    public bool IsBitfield => BitMask != 0;

    // https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.BitOperations.TrailingZeroCount
    private static int TrailingZeroCount(uint mask)
    {
        return Unsafe.AddByteOffset(
            ref MemoryMarshal.GetReference(TrailingZeroCountDeBruijn),
            (nint)(((mask & (uint)-(int)mask) * 0x077CB531u) >> 27));
    }

    private static ReadOnlySpan<byte> TrailingZeroCountDeBruijn => [
        00, 01, 28, 02, 29, 14, 24, 03,
        30, 22, 20, 15, 25, 17, 04, 08,
        31, 27, 13, 23, 21, 19, 16, 07,
        26, 12, 18, 06, 11, 05, 10, 09
    ];

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.Append($"{Type,-32} {Name,-32} // 0x{Offset:X3}");

        if (BitMask > 0)
        {
            sb.Append($" (0x{Size:X3}, 0b{Convert.ToString(BitMask, 2).PadLeft(8, '0')})");
        }
        else
        {
            sb.Append($" (0x{Size:X3})");
        }

        return sb.ToString();
    }
}
