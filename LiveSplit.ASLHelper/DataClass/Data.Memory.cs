using static LiveSplit.ComponentUtil.SigScanTarget;

namespace ASLHelper;

internal static partial class Data
{
    public static readonly OnFoundCallback s_OnFound = (p, _, addr)
        => p.Is64Bit() ? addr + 0x4 + p.ReadValue<int>(addr) : p.ReadPointer(addr);
}
