namespace ASLHelper.UnityHelper;

public class MemoryList<T> : MemoryClass where T : unmanaged
{
    public MemoryList(nint baseAddress, int staticFieldOffset, int[] offsets)
        : base(baseAddress, staticFieldOffset, offsets)
    {
        Offsets = new[]
        {
            Data.s_Helper.Is64Bit ? 0x18 : 0xC,
            Data.s_Helper.Is64Bit ? 0x10 : 0x8,
        };
    }

    public int[] Offsets;

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

        var addr = Address;
        var helper = Data.s_Helper;

        if (addr == 0
            || !helper.TryRead<int>(out var count, addr + Offsets[0]) || count > 512
            || !helper.TryRead<nint>(out var items, addr + Offsets[1]))
        {
            if (FailAction == ReadFailAction.DontUpdate)
                return false;

            base.Current = new List<T>();
        }
        else
        {
            var buf = new T[count];
            if (!helper.TryReadSpan<T>(buf, items))
            {
                if (FailAction == ReadFailAction.DontUpdate)
                    return false;

                base.Current = new List<T>();
            }
            else
            {
                Current = buf.ToList();
            }
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