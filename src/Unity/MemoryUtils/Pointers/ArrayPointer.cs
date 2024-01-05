namespace AslHelp.MemUtils.Pointers;

public class ArrayPointer<T> : BasePointer<T[]> where T : unmanaged
{
    private static readonly Unity _game = Unity.Instance;

    public ArrayPointer(nint baseAddress, int[] offsets)
        : base(baseAddress, offsets) { }

    protected override T[] Default => Array.Empty<T>();

    public override bool Write(T[] value)
    {
        throw new NotImplementedException();
    }

    protected override bool TryUpdate(out T[] result)
    {
        if (!_game.TryRead(out nint deref, _base, _offsets))
        {
            result = Default;
            return false;
        }

        if (!_game.TryRead(out int length, deref + (_game.PtrSize * 3)))
        {
            result = Default;
            return false;
        }

        result = new T[length];

        if (!_game.TryReadSpan_Internal<T>(result, deref + (_game.PtrSize * 4)))
        {
            result = Default;
            return false;
        }

        return true;
    }

    protected override bool HasChanged(T[] old, T[] current)
    {
        return old.Length != current.Length || !old.SequenceEqual(current);
    }

    public override string ToString()
    {
        return $"ArrayPointer<{typeof(T).Name}>({OffsetsToString()})";
    }
}
