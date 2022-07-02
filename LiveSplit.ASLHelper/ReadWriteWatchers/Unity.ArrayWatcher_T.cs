namespace ASLHelper.UnityHelper;

public class ArrayWatcher<T> : ReadWriteWatcher where T : unmanaged
{
    public ArrayWatcher(nint baseAddress, int staticFieldOffset, int[] offsets)
        : base(baseAddress, staticFieldOffset, offsets) { }

    public new T[] Current
    {
        get => (T[])(base.Current ?? Array.Empty<T>());
        set => base.Current = value;
    }

    public new T[] Old
    {
        get => (T[])(base.Old ?? Array.Empty<T>());
        set => base.Old = value;
    }

    public bool Write(T[] values)
    {
        throw new NotImplementedException();
    }

    private protected override bool Update_Internal()
    {
        if (!Unity.Instance.TryReadArray<T>(out var values, _baseAddress, _offsets))
        {
            if (FailAction == ReadFailAction.DontUpdate)
                return false;

            base.Current = Array.Empty<T>();
        }
        else
        {
            Current = values;
        }

        return true;
    }

    public override void Reset()
    {
        base.Old = Array.Empty<T>();
        base.Current = Array.Empty<T>();
        InitialUpdate = false;
    }
}
