namespace ASLHelper;

public abstract class ReadWriteWatcher : MemoryWatcher
{
    private protected readonly nint _baseAddress;
    private protected readonly int[] _offsets;

    internal ReadWriteWatcher(nint baseAddress, int staticFieldOffset, int[] offsets)
        : base(null)
    {
        _baseAddress = baseAddress + staticFieldOffset;
        _offsets = offsets;
    }

    internal ReadWriteWatcher(nint baseAddress, int[] offsets)
        : base(null)
    {
        _baseAddress = baseAddress;
        _offsets = offsets;
    }

    private protected abstract bool Update_Internal();

    public sealed override bool Update(Process process)
    {
        Changed = false;

        if (!Enabled || !CheckInterval())
            return false;

        Old = Current;

        if (!Update_Internal())
            return false;

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
}
