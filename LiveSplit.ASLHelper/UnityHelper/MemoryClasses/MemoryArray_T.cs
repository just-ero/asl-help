namespace ASLHelper.UnityHelper;

public class MemoryArray<T> : MemoryClass where T : unmanaged
{
    public MemoryArray(nint baseAddress, int staticFieldOffset, int[] offsets)
        : base(baseAddress, staticFieldOffset, offsets) { }

    public new T[] Old
    {
        get => (T[])(base.Old ?? Array.Empty<T>());
        set => base.Old = value;
    }

    public new T[] Current
    {
        get => (T[])(base.Current ?? Array.Empty<T>());
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
            || !helper.TryRead<int>(out var len, addr + (helper.PtrSize * 3)) || len > 512)
        {
            if (FailAction == ReadFailAction.DontUpdate)
                return false;

            base.Current = Array.Empty<T>();
        }
        else
        {
            var buf = new T[len];
            if (!helper.TryReadSpan(buf, addr + (helper.PtrSize * 4)))
            {
                if (FailAction == ReadFailAction.DontUpdate)
                    return false;

                base.Current = Array.Empty<T>();
            }
            else
            {
                Current = buf;
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
        base.Old = Array.Empty<T>();
        base.Current = Array.Empty<T>();
        InitialUpdate = false;
    }
}
