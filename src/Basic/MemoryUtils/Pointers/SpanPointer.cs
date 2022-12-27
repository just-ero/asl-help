namespace AslHelp.MemUtils.Pointers;

public class SpanPointer<T> : BasePointer<T[]> where T : unmanaged
{
    private readonly int _length;

    public SpanPointer(int length, nint baseAddress, params int[] offsets)
        : base(baseAddress, offsets)
    {
        _length = length;
    }

    protected override T[] Default { get; } = Array.Empty<T>();

    public override bool Write(T[] value)
    {
        return Basic.Instance.WriteSpan(value, _base, _offsets);
    }

    protected override bool TryUpdate(out T[] result)
    {
        result = new T[_length];

        if (!Basic.Instance.TryReadSpan_Internal<T>(result, _base, _offsets))
        {
            result = Array.Empty<T>();
            return false;
        }

        return true;
    }

    protected override bool HasChanged(T[] old, T[] current)
    {
        return !old.SequenceEqual(current);
    }

    public override string ToString()
    {
        return $"SpanPointer<{typeof(T).Name}>({OffsetsToString()})";
    }
}
