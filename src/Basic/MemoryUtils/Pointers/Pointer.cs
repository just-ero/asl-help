namespace AslHelp.MemUtils.Pointers;

public class Pointer<T> : BasePointer<T> where T : unmanaged
{
    public Pointer(nint baseAddress, params int[] offsets)
        : base(baseAddress, offsets) { }

    public override bool Write(T value)
    {
        return Basic.Instance.Write(value, _base, _offsets);
    }

    protected override bool TryUpdate(out T result)
    {
        return Basic.Instance.TryRead(out result, _base, _offsets);
    }

    protected override bool HasChanged(T old, T current)
    {
        return !old.Equals(current);
    }

    public override string ToString()
    {
        return $"Pointer<{typeof(T).Name}>({OffsetsToString()})";
    }
}
