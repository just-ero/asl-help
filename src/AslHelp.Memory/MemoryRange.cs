using AslHelp.Memory.Native.Enums;
using AslHelp.Memory.Native.Structs;

namespace AslHelp.Memory;

public readonly struct MemoryRange
{
    public MemoryRange(nint @base, int regionSize, MemoryRangeProtect protect, MemoryRangeState state, MemoryRangeType type)
    {
        Base = @base;
        Size = regionSize;
        Protect = protect;
        State = state;
        Type = type;
    }

    internal unsafe MemoryRange(MemoryBasicInformation mbi)
    {
        Base = (nint)mbi.BaseAddress;
        Size = (int)mbi.RegionSize;
        Protect = mbi.Protect;
        State = mbi.State;
        Type = mbi.Type;
    }

    public nint Base { get; }
    public int Size { get; }

    public MemoryRangeProtect Protect { get; }
    public MemoryRangeState State { get; }
    public MemoryRangeType Type { get; }

    public override string ToString()
    {
        return $"{nameof(MemoryRange)} {{ {nameof(Base)} = 0x{(ulong)Base:X}, {nameof(Size)} = 0x{Size:X} }}";
    }
}
