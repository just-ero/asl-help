namespace AslHelp.MemUtils.Pointers;

public class TSetPointer<T> : BasePointer<T[]> where T : unmanaged
{
    private static readonly Unreal _game = Unreal.Instance;

    private T[] _buffer = Array.Empty<T>();

    public TSetPointer(nint baseAddress, int[] offsets)
        : base(baseAddress, offsets) { }

    public override bool Write(T[] value)
    {
        throw new NotImplementedException();
    }

    protected override unsafe bool TryUpdate(out T[] result)
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

        Unreal.TSetElement<T>* buffer = stackalloc Unreal.TSetElement<T>[arrayNum];

        if (!_game.Game.Read(allocator, buffer, WinAPI.GetTypeSize<Unreal.TSetElement<T>>(_game.Is64Bit)))
        {
            result = Array.Empty<T>();
            return false;
        }

        for (int i = 0; i < arrayNum; i++)
        {
            _buffer[i] = buffer[i].Element;
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
