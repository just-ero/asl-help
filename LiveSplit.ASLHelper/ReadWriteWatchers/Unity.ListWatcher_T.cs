namespace ASLHelper.UnityHelper;

public class ListWatcher<T> : ReadWriteWatcher where T : unmanaged
{
    public ListWatcher(nint baseAddress, int staticFieldOffset, int[] offsets)
        : base(baseAddress, staticFieldOffset, offsets) { }

    public new List<T> Old
    {
        get => (List<T>)(base.Old ?? new List<T>());
        set => base.Old = value;
    }

    public new List<T> Current
    {
        get => (List<T>)(base.Current ?? new List<T>());
        set => base.Current = value;
    }

    public bool Write(List<T> values)
    {
        throw new NotImplementedException();
    }

    private protected override bool Update_Internal()
    {
        if (!Unity.Instance.TryReadList<T>(out var values, _baseAddress, _offsets))
        {
            if (FailAction == ReadFailAction.DontUpdate)
                return false;

            base.Current = new();
        }
        else
        {
            Current = values;
        }

        return true;
    }

    public override void Reset()
    {
        base.Old = new();
        base.Current = new();
        InitialUpdate = false;
    }
}
