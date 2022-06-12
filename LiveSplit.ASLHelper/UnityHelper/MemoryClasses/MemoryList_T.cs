namespace ASLHelper.UnityHelper;

public class MemoryList<T> : MemoryClass where T : unmanaged
{
    public MemoryList(nint baseAddress, int staticFieldOffset, int[] offsets)
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

    public override bool Update(Process process)
    {
        Changed = false;

        if (!Enabled || !CheckInterval())
            return false;

        base.Old = Current;

        if (!Unity.Instance.TryReadList<T>(out var values, _baseAddress, _offsets))
        {
            if (FailAction == ReadFailAction.DontUpdate)
                return false;

            base.Current = new List<T>();
        }
        else
        {
            Current = values;
        }

        if (!InitialUpdate)
        {
            InitialUpdate = true;
            return false;
        }

        if (!Current.Equals(Old))
        {
            Changed = true;
            return true;
        }

        return false;
    }

    public override void Reset()
    {
        base.Old = new List<T>();
        base.Current = new List<T>();
        InitialUpdate = false;
    }
}