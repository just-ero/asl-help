extern alias Ls;

using System.Linq;

using AslHelp.Memory;

using Ls::LiveSplit.ComponentUtil;

public partial class Basic
{
    public nint Scan(int offset, string pattern)
    {
        return Memory.Scan(offset, pattern);
    }

    public nint Scan(int offset, string pattern, string moduleName)
    {
        return Memory.Scan(offset, pattern, moduleName);
    }

    public nint Scan(int offset, string pattern, Module module)
    {
        return Memory.Scan(offset, pattern, module);
    }

    public nint Scan(int offset, string pattern, nint startAddress, int size)
    {
        return Memory.Scan(offset, pattern, startAddress, size);
    }

    public nint Scan(int offset, string pattern, nint startAddress, byte[] memory)
    {
        return Memory.Scan(offset, pattern, startAddress, memory);
    }

    public nint Scan(SigScanTarget target)
    {
        return ScanAll(target).FirstOrDefault();
    }

    public nint Scan(SigScanTarget target, int size)
    {
        return ScanAll(target, size).FirstOrDefault();
    }

    public nint Scan(SigScanTarget target, string moduleName)
    {
        return ScanAll(target, moduleName).FirstOrDefault();
    }

    public nint Scan(SigScanTarget target, string moduleName, int size)
    {
        return ScanAll(target, moduleName, size).FirstOrDefault();
    }

    public nint Scan(SigScanTarget target, Module module)
    {
        return ScanAll(target, module).FirstOrDefault();
    }

    public nint Scan(SigScanTarget target, Module module, int size)
    {
        return ScanAll(target, module, size).FirstOrDefault();
    }

    public nint Scan(SigScanTarget target, nint startAddress, int size)
    {
        return ScanAll(target, startAddress, size).FirstOrDefault();
    }
}
