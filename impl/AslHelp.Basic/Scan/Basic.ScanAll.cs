using System.Collections.Generic;

using AslHelp.Memory;
using AslHelp.Memory.Ipc;
using AslHelp.Memory.Scanning;

using LiveSplit.ComponentUtil;

public partial class Basic
{
    public IEnumerable<nint> ScanAll(int offset, string pattern)
    {
        return ((IMemoryScanner)this).ScanAll(ScanPattern.Parse(offset, pattern));
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, string moduleName)
    {
        return ((IMemoryScanner)this).ScanAll(ScanPattern.Parse(offset, pattern), moduleName);
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, Module module)
    {
        return ((IMemoryScanner)this).ScanAll(ScanPattern.Parse(offset, pattern), module);
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, nint startAddress, int size)
    {
        return ((IMemoryScanner)this).ScanAll(ScanPattern.Parse(offset, pattern), startAddress, size);
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, nint startAddress, byte[] memory)
    {
        return ((IMemoryScanner)this).ScanAll(ScanPattern.Parse(offset, pattern), startAddress, memory);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern)
    {
        return ScanAll(pattern, MainModule);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern, int size)
    {
        return ScanAll(pattern, MainModule, size);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern, string moduleName)
    {
        return ScanAll(pattern, Modules[moduleName]);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern, string moduleName, int size)
    {
        return ScanAll(pattern, Modules[moduleName], size);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern, Module module)
    {
        return ScanAll(pattern, module.Base, module.MemorySize);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern, Module module, int size)
    {
        return ScanAll(pattern, module.Base, size);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern, nint startAddress, int size)
    {
        byte[] memory = ReadArray<byte>(size, startAddress);
        return ScanAll(pattern, startAddress, memory);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern, nint startAddress, byte[] memory)
    {

    }

    IEnumerable<nint> IMemoryScanner.ScanAll(ScanPattern pattern)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, MainModule);
    }

    IEnumerable<nint> IMemoryScanner.ScanAll(ScanPattern pattern, int size)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, MainModule, size);
    }

    IEnumerable<nint> IMemoryScanner.ScanAll(ScanPattern pattern, string moduleName)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, Modules[moduleName]);
    }

    IEnumerable<nint> IMemoryScanner.ScanAll(ScanPattern pattern, string moduleName, int size)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, Modules[moduleName], size);
    }

    IEnumerable<nint> IMemoryScanner.ScanAll(ScanPattern pattern, Module module)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, module.Base, module.MemorySize);
    }

    IEnumerable<nint> IMemoryScanner.ScanAll(ScanPattern pattern, Module module, int size)
    {
        return ((IMemoryScanner)this).ScanAll(pattern, module.Base, size);
    }

    IEnumerable<nint> IMemoryScanner.ScanAll(ScanPattern pattern, nint startAddress, int size)
    {
        byte[] memory = ReadArray<byte>(size, startAddress);
        return ((IMemoryScanner)this).ScanAll(pattern, startAddress, memory);
    }

    IEnumerable<nint> IMemoryScanner.ScanAll(ScanPattern pattern, nint startAddress, byte[] memory)
    {
        ScanEnumerator scanner = new(memory, pattern);

        while (scanner.MoveNext())
        {
            yield return startAddress + scanner.Current + pattern.Offset;
        }
    }
}
