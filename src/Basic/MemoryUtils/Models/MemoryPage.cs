namespace AslHelp.MemUtils.Models;

public record MemoryPage
{
    internal unsafe MemoryPage(MEMORY_BASIC_INFORMATION mbi)
    {
        Base = (nint)mbi.BaseAddress;
        RegionSize = (int)mbi.RegionSize;
        Protect = mbi.Protect;
        State = mbi.State;
        Type = mbi.Type;
    }

    public nint Base { get; }
    public int RegionSize { get; }

    public MemPageProtect Protect { get; }
    public MemPageState State { get; }
    public MemPageType Type { get; }
}
