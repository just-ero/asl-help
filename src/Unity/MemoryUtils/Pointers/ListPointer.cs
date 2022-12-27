namespace AslHelp.MemUtils.Pointers;

public class ListPointer<T> : BasePointer<List<T>> where T : unmanaged
{
    private static readonly Unity _game = Unity.Instance;

    public ListPointer(nint baseAddress, int[] offsets)
        : base(baseAddress, offsets) { }

    protected override List<T> Default { get; } = new();

    public override bool Write(List<T> value)
    {
        throw new NotImplementedException();
    }

    protected override bool TryUpdate(out List<T> result)
    {
        if (!_game.TryRead<nint>(out nint deref, _base, _offsets))
        {
            result = new();
            return false;
        }

        if (!_game.TryRead<int>(out int count, deref + (_game.PtrSize * 3)))
        {
            result = new();
            return false;
        }

        T[] resultArr = new T[count];

        if (!_game.TryRead<nint>(out nint items, deref + (_game.PtrSize * 2)))
        {
            result = new();
            return false;
        }

        if (!_game.TryReadSpan_Internal<T>(resultArr, items + (_game.PtrSize * 4)))
        {
            result = new();
            return false;
        }

        result = new(resultArr);

        return true;
    }

    protected override bool HasChanged(List<T> old, List<T> current)
    {
        return old.Count != current.Count || !old.SequenceEqual(current);
    }

    public override string ToString()
    {
        return $"ListPointer<{typeof(T).Name}>({OffsetsToString()})";
    }
}
