namespace ASLHelper.UnityHelper;

public class MemoryString : MemoryClass
{
    public MemoryString(nint baseAddress, int staticFieldOffset, int[] offsets)
        : base(baseAddress, staticFieldOffset, offsets) { }

    public int[] Offsets;

    public new string Old
    {
        get => (string)base.Old;
        set => base.Old = value;
    }

    public new string Current
    {
        get => (string)base.Current;
        set => base.Current = value;
    }

    public override bool Update(Process process)
    {
        Changed = false;

        if (!Enabled || !CheckInterval())
            return false;

        base.Old = Current;

        if (!Unity.Instance.TryReadString(out var result, _baseAddress, _offsets))
        {
            if (FailAction == ReadFailAction.DontUpdate)
                return false;

            base.Current = null;
        }
        else
        {
            Current = result;
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
        base.Old = null;
        base.Current = null;
        InitialUpdate = false;
    }
}
