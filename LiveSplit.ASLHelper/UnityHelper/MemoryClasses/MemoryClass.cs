namespace ASLHelper.UnityHelper;

public abstract class MemoryClass : MemoryWatcher
{
    public MemoryClass(nint baseAddress, int staticFieldOffset, int[] offsets)
        : base(null)
    {
        _baseAddress = baseAddress + staticFieldOffset;
        _offsets = offsets;
    }

    private readonly nint _baseAddress;
    private readonly int[] _offsets;

    protected new nint Address
    {
        get => Data.s_Helper.Read<nint>(Data.s_Helper.Deref(_baseAddress, _offsets));
    }

    #region Classes
    internal static MonoClass s_listClass;
    internal static MonoClass s_stringClass;
    #endregion
}