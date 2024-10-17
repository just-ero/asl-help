extern alias Ls;

using System.Collections.Generic;

using AslHelp.Memory;
using AslHelp.Shared;

using Ls::LiveSplit.ComponentUtil;

public partial class Basic
{
    public IEnumerable<nint> ScanAll(int offset, string pattern)
    {
        return Memory.ScanAll(offset, pattern);
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, string moduleName)
    {
        return Memory.ScanAll(offset, pattern, moduleName);
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, Module module)
    {
        return Memory.ScanAll(offset, pattern, module);
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, nint startAddress, int size)
    {
        return Memory.ScanAll(offset, pattern, startAddress, size);
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, nint startAddress, byte[] memory)
    {
        return Memory.ScanAll(offset, pattern, startAddress, memory);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern)
    {
        return ScanAll(pattern, MainModule.Base, MainModule.MemorySize);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern, int size)
    {
        return ScanAll(pattern, MainModule.Base, size);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern, string moduleName)
    {
        Module module = Modules[moduleName];
        return ScanAll(pattern, module.Base, module.MemorySize);
    }

    public IEnumerable<nint> ScanAll(SigScanTarget pattern, string moduleName, int size)
    {
        Module module = Modules[moduleName];
        return ScanAll(pattern, module.Base, size);
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
        ThrowHelper.ThrowIfNull(Game);

        SignatureScanner scanner = new(Game, startAddress, size);
        return scanner.ScanAll(pattern);
    }
}
