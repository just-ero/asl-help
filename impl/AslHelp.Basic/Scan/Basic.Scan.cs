extern alias Ls;

using Ls::LiveSplit.ComponentUtil;

using System.Linq;

using AslHelp.Memory;
using AslHelp.Memory.Ipc;
using AslHelp.Memory.Scanning;

public partial class Basic
{
    public nint Scan(int offset, string pattern)
    {
        return ((IMemoryScanner)this).Scan(ScanPattern.Parse(offset, pattern));
    }

    public nint Scan(int offset, string pattern, string moduleName)
    {
        return ((IMemoryScanner)this).Scan(ScanPattern.Parse(offset, pattern), moduleName);
    }

    public nint Scan(int offset, string pattern, Module module)
    {
        return ((IMemoryScanner)this).Scan(ScanPattern.Parse(offset, pattern), module);
    }

    public nint Scan(int offset, string pattern, nint startAddress, int size)
    {
        return ((IMemoryScanner)this).Scan(ScanPattern.Parse(offset, pattern), startAddress, size);
    }

    public nint Scan(int offset, string pattern, nint startAddress, byte[] memory)
    {
        return ((IMemoryScanner)this).Scan(ScanPattern.Parse(offset, pattern), startAddress, memory);
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

    nint IMemoryScanner.Scan(ScanPattern pattern)
    {
        return ((IMemoryScanner)this).ScanAll(pattern).FirstOrDefault();
    }

    nint IMemoryScanner.Scan(ScanPattern pattern, int size)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, size).FirstOrDefault();
    }

    nint IMemoryScanner.Scan(ScanPattern pattern, string moduleName)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, moduleName).FirstOrDefault();
    }

    nint IMemoryScanner.Scan(ScanPattern pattern, string moduleName, int size)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, moduleName, size).FirstOrDefault();
    }

    nint IMemoryScanner.Scan(ScanPattern pattern, Module module)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, module).FirstOrDefault();
    }

    nint IMemoryScanner.Scan(ScanPattern pattern, Module module, int size)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, module, size).FirstOrDefault();
    }

    nint IMemoryScanner.Scan(ScanPattern pattern, nint startAddress, int size)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, startAddress, size).FirstOrDefault();
    }

    nint IMemoryScanner.Scan(ScanPattern pattern, nint startAddress, byte[] memory)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, startAddress, memory).FirstOrDefault();
    }
}
