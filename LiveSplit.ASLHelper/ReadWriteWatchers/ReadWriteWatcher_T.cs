namespace ASLHelper;

public class ReadWriteWatcher<T> : ReadWriteWatcher where T : unmanaged
{
    internal ReadWriteWatcher(nint baseAddress, int[] offsets)
        : base(baseAddress, offsets) { }

    public new T Current
    {
        get => (T)(base.Current ?? default(T));
        set => base.Current = value;
    }

    public new T Old
    {
        get => (T)(base.Old ?? default(T));
        set => base.Old = value;
    }

    public bool Write(T value)
    {
        return Main.Instance.Write<T>(value, _baseAddress, _offsets);
    }

    private protected override bool Update_Internal()
    {
        if (!Main.Instance.TryRead<T>(out var value, _baseAddress, _offsets))
        {
            if (FailAction == ReadFailAction.DontUpdate)
                return false;

            base.Current = default(T);
        }
        else
        {
            Current = value;
        }

        return true;
    }

    public override void Reset()
    {
        base.Old = default(T);
        base.Current = default(T);
        InitialUpdate = false;
    }
}
