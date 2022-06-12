namespace ASLHelper.UnityHelper;

public abstract class MemoryClass : MemoryWatcher
{
    internal MemoryClass(nint baseAddress, int staticFieldOffset, int[] offsets)
        : base(null)
    {
        _baseAddress = baseAddress + staticFieldOffset;
        _offsets = offsets;
    }

    private protected readonly nint _baseAddress;
    private protected readonly int[] _offsets;
}