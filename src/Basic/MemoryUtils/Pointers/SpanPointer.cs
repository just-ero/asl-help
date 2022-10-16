namespace AslHelp.MemUtils.Pointers;

public class SpanPointer<T> : BasePointer<T[]> where T : unmanaged
{
    private readonly T[] _buffer;
    private readonly T[] _default;

    public SpanPointer(int length, nint baseAddress, params int[] offsets)
        : base(baseAddress, offsets)
    {
        _buffer = _default = new T[length];
    }

    protected override T[] Default { get; } = Array.Empty<T>();

    public override bool Write(T[] value)
    {
        return Basic.Instance.WriteSpan(value, _base, _offsets);
    }

    protected override bool TryUpdate(out T[] result)
    {
        if (!Basic.Instance.TryReadSpan_Internal<T>(_buffer, _base, _offsets))
        {
            result = new T[_buffer.Length];
            return false;
        }

        result = _buffer;
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
