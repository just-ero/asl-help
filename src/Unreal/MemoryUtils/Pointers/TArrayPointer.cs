namespace AslHelp.MemUtils.Pointers;

public class TArrayPointer<T> : BasePointer<T[]> where T : unmanaged
{
    private static readonly Unreal _game = Unreal.Instance;

    private T[] _buffer = Array.Empty<T>();

    public TArrayPointer(nint baseAddress, int[] offsets)
        : base(baseAddress, offsets) { }

    public override bool Write(T[] value)
    {
        throw new NotImplementedException();
    }

    protected override bool TryUpdate(out T[] result)
    {
        if (!_game.TryDeref(out nint deref, _base, _offsets))
        {
            result = Array.Empty<T>();
            return false;
        }

        if (!_game.TryRead<int>(out int arrayNum, deref + _game.PtrSize))
        {
            result = Array.Empty<T>();
            return false;
        }

        if (arrayNum != _buffer.Length)
        {
            _buffer = new T[arrayNum];
        }

        if (!_game.TryRead<nint>(out nint allocator, deref))
        {
            result = Array.Empty<T>();
            return false;
        }

        if (!_game.TryReadSpan_Internal<T>(_buffer, deref))
        {
            result = Array.Empty<T>();
            return false;
        }

        result = _buffer;

        return true;
    }

    protected override bool HasChanged(T[] old, T[] current)
    {
        return old.Length != current.Length || !old.SequenceEqual(current);
    }

    public override string ToString()
    {
        return $"TArrayPointer<{typeof(T).Name}>({OffsetsToString()})";
    }
}
