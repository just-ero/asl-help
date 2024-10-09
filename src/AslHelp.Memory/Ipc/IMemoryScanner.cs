using System.Collections.Generic;

using AslHelp.Memory.Scanning;

namespace AslHelp.Memory.Ipc;

public interface IMemoryScanner
{
    nint Scan(ScanPattern pattern);
    nint Scan(ScanPattern pattern, int size);
    nint Scan(ScanPattern pattern, string moduleName);
    nint Scan(ScanPattern pattern, string moduleName, int size);
    nint Scan(ScanPattern pattern, Module module);
    nint Scan(ScanPattern pattern, Module module, int size);

    nint Scan(ScanPattern pattern, nint startAddress, int size);
    nint Scan(ScanPattern pattern, nint startAddress, byte[] memory);

    nint Scan(int offset, string pattern);
    nint Scan(int offset, string pattern, string moduleName);
    nint Scan(int offset, string pattern, Module module);

    nint Scan(int offset, string pattern, nint startAddress, int size);
    nint Scan(int offset, string pattern, nint startAddress, byte[] memory);

    IEnumerable<nint> ScanAll(ScanPattern pattern);
    IEnumerable<nint> ScanAll(ScanPattern pattern, int size);
    IEnumerable<nint> ScanAll(ScanPattern pattern, string moduleName);
    IEnumerable<nint> ScanAll(ScanPattern pattern, string moduleName, int size);
    IEnumerable<nint> ScanAll(ScanPattern pattern, Module module);
    IEnumerable<nint> ScanAll(ScanPattern pattern, Module module, int size);

    IEnumerable<nint> ScanAll(ScanPattern pattern, nint startAddress, int size);
    IEnumerable<nint> ScanAll(ScanPattern pattern, nint startAddress, byte[] memory);

    IEnumerable<nint> ScanAll(int offset, string pattern);
    IEnumerable<nint> ScanAll(int offset, string pattern, string moduleName);
    IEnumerable<nint> ScanAll(int offset, string pattern, Module module);

    IEnumerable<nint> ScanAll(int offset, string pattern, nint startAddress, int size);
    IEnumerable<nint> ScanAll(int offset, string pattern, nint startAddress, byte[] memory);
}
