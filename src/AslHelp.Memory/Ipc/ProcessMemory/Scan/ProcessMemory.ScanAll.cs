using System.Collections.Generic;

using AslHelp.Memory.Scanning;

namespace AslHelp.Memory.Ipc;

public partial class ProcessMemory
{
    public IEnumerable<nint> ScanAll(int offset, string pattern)
    {
        return ScanAll(ScanPattern.Parse(offset, pattern));
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, string moduleName)
    {
        return ScanAll(ScanPattern.Parse(offset, pattern), moduleName);
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, Module module)
    {
        return ScanAll(ScanPattern.Parse(offset, pattern), module);
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, nint startAddress, int size)
    {
        return ScanAll(ScanPattern.Parse(offset, pattern), startAddress, size);
    }

    public IEnumerable<nint> ScanAll(int offset, string pattern, nint startAddress, byte[] memory)
    {
        return ScanAll(ScanPattern.Parse(offset, pattern), startAddress, memory);
    }

    public IEnumerable<nint> ScanAll(ScanPattern pattern)
    {
        return ScanAll(pattern, MainModule.Base, MainModule.MemorySize);
    }

    public IEnumerable<nint> ScanAll(ScanPattern pattern, int size)
    {
        return ScanAll(pattern, MainModule.Base, size);
    }

    public IEnumerable<nint> ScanAll(ScanPattern pattern, string moduleName)
    {
        Module module = Modules[moduleName];
        return ScanAll(pattern, module.Base, module.MemorySize);
    }

    public IEnumerable<nint> ScanAll(ScanPattern pattern, string moduleName, int size)
    {
        Module module = Modules[moduleName];
        return ScanAll(pattern, module.Base, size);
    }

    public IEnumerable<nint> ScanAll(ScanPattern pattern, Module module)
    {
        return ScanAll(pattern, module.Base, module.MemorySize);
    }

    public IEnumerable<nint> ScanAll(ScanPattern pattern, Module module, int size)
    {
        return ScanAll(pattern, module.Base, size);
    }

    public IEnumerable<nint> ScanAll(ScanPattern pattern, nint startAddress, int size)
    {
        byte[] memory = ReadArray<byte>(size, startAddress);
        return ScanAll(pattern, startAddress, memory);
    }

    public IEnumerable<nint> ScanAll(ScanPattern pattern, nint startAddress, byte[] memory)
    {
        ScanEnumerator scanner = new(memory, pattern);

        while (scanner.MoveNext())
        {
            yield return startAddress + scanner.Current + pattern.Offset;
        }
    }
}
