namespace AslHelp.MemUtils;

internal unsafe struct MEMORY_BASIC_INFORMATION
{
    public void* BaseAddress;
    public void* AllocationBase;
    public MemPageProtect AllocationProtect;
    public nuint RegionSize;
    public MemPageState State;
    public MemPageProtect Protect;
    public MemPageType Type;

    public static nuint SelfSize => (nuint)sizeof(MEMORY_BASIC_INFORMATION);
}
