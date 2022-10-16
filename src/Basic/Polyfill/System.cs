using System.Runtime.CompilerServices;

namespace System;

internal readonly struct Index : IEquatable<Index>
{
    private readonly int _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Index(int value, bool fromEnd = false)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException("Non-negative number required.");
        }

        if (fromEnd)
        {
            _value = ~value;
        }
        else
        {
            _value = value;
        }
    }

    private Index(int value)
    {
        _value = value;
    }

    public int Value => _value < 0 ? ~_value : _value;
    public bool IsFromEnd => _value < 0;

    public static Index Start => new(0);
    public static Index End => new(~0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Index FromStart(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException("Non-negative number required.");
        }

        return new(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Index FromEnd(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException("Non-negative number required.");
        }

        return new Index(~value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetOffset(int length)
    {
        int offset = _value;
        if (IsFromEnd)
        {
            // offset = length - (~value)
            // offset = length + (~(~value) + 1)
            // offset = length + value + 1

            offset += length + 1;
        }

        return offset;
    }

    public static implicit operator Index(int value)
    {
        return FromStart(value);
    }

    public bool Equals(Index other)
    {
        return _value == other._value;
    }

    public override bool Equals(object obj)
    {
        return obj is Index i && _value == i._value;
    }

    public override int GetHashCode()
    {
        return _value;
    }

    public override string ToString()
    {
        if (IsFromEnd)
        {
            return '^' + Value.ToString();
        }

        return ((uint)Value).ToString();
    }
}

internal readonly struct Range : IEquatable<Range>
{
    public Range(Index start, Index end)
    {
        Start = start;
        End = end;
    }

    public Index Start { get; }
    public Index End { get; }

    public static Range All => new(Index.Start, Index.End);

    public static Range StartAt(Index start)
    {
        return new(start, Index.End);
    }

    public static Range EndAt(Index end)
    {
        return new(Index.Start, end);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Offset, int Length) GetOffsetAndLength(int length)
    {
        int start;
        Index startIndex = Start;

        if (startIndex.IsFromEnd)
        {
            start = length - startIndex.Value;
        }
        else
        {
            start = startIndex.Value;
        }

        int end;
        Index endIndex = End;

        if (endIndex.IsFromEnd)
        {
            end = length - endIndex.Value;
        }
        else
        {
            end = endIndex.Value;
        }

        if ((uint)end > (uint)length || (uint)start > (uint)end)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        return (start, end - start);
    }

    public bool Equals(Range other)
    {
        return other.Start.Equals(Start) && other.End.Equals(End);
    }

    public override bool Equals(object obj)
    {
        return obj is Range r && r.Start.Equals(Start) && r.End.Equals(End);
    }

    public override int GetHashCode()
    {
        int h1 = Start.GetHashCode(), h2 = End.GetHashCode();

        uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
        return ((int)rol5 + h1) ^ h2;
    }

    public override string ToString()
    {
        return Start.ToString() + ".." + End.ToString();
    }
}
