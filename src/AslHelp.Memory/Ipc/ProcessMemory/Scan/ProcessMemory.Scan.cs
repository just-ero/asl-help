using System.Linq;

using AslHelp.Memory.Scanning;

namespace AslHelp.Memory.Ipc;

public partial class ProcessMemory
{
    public nint Scan(int offset, string pattern)
    {
        return Scan(ScanPattern.Parse(offset, pattern));
    }

    public nint Scan(int offset, string pattern, string moduleName)
    {
        return Scan(ScanPattern.Parse(offset, pattern), moduleName);
    }

    public nint Scan(int offset, string pattern, Module module)
    {
        return Scan(ScanPattern.Parse(offset, pattern), module);
    }

    public nint Scan(int offset, string pattern, nint startAddress, int size)
    {
        return Scan(ScanPattern.Parse(offset, pattern), startAddress, size);
    }

    public nint Scan(int offset, string pattern, nint startAddress, byte[] memory)
    {
        return Scan(ScanPattern.Parse(offset, pattern), startAddress, memory);
    }

    public nint Scan(ScanPattern pattern)
    {
        return ScanAll(pattern).FirstOrDefault();
    }

    public nint Scan(ScanPattern pattern, int size)
    {
        return ScanAll(pattern, size).FirstOrDefault();
    }

    public nint Scan(ScanPattern pattern, string moduleName)
    {
        return ScanAll(pattern, moduleName).FirstOrDefault();
    }

    public nint Scan(ScanPattern pattern, string moduleName, int size)
    {
        return ScanAll(pattern, moduleName, size).FirstOrDefault();
    }

    public nint Scan(ScanPattern pattern, Module module)
    {
        return ScanAll(pattern, module).FirstOrDefault();
    }

    public nint Scan(ScanPattern pattern, Module module, int size)
    {
        return ScanAll(pattern, module, size).FirstOrDefault();
    }

    public nint Scan(ScanPattern pattern, nint startAddress, int size)
    {
        return ScanAll(pattern, startAddress, size).FirstOrDefault();
    }

    public nint Scan(ScanPattern pattern, nint startAddress, byte[] memory)
    {
        return ScanAll(pattern, startAddress, memory).FirstOrDefault();
    }
}
