namespace ASLHelper.UnityHelper;

public class MemoryString : MemoryClass
{
    public MemoryString(nint baseAddress, int staticFieldOffset, int[] offsets)
        : base(baseAddress, staticFieldOffset, offsets)
    {
        Offsets = new[]
        {
            Data.s_Helper.Is64Bit ? 0x10 : 0x8,
            Data.s_Helper.Is64Bit ? 0x14 : 0xC,
        };
    }

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

        var addr = Address;
        if (addr == 0 || !process.ReadValue<int>(addr + Offsets[0], out var len))
        {
            if (FailAction == ReadFailAction.DontUpdate)
                return false;

            base.Current = null;
        }
        else
        {
            base.Current = len > 0 ? process.ReadString(addr + Offsets[1], ReadStringType.UTF16, len * 2) : "";
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
