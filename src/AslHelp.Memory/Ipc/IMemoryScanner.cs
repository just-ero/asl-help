using System.Collections.Generic;

using AslHelp.Memory.Scanning;

namespace AslHelp.Memory.Ipc;

public interface IMemoryScanner
{
    nuint Scan(ScanPattern pattern);
    nuint Scan(ScanPattern pattern, nuint size);
    nuint Scan(ScanPattern pattern, string moduleName);
    nuint Scan(ScanPattern pattern, string moduleName, nuint size);
    nuint Scan(ScanPattern pattern, Module module);
    nuint Scan(ScanPattern pattern, Module module, nuint size);

    nuint Scan(ScanPattern pattern, nuint startAddress, nuint size);
    nuint Scan(ScanPattern pattern, nuint startAddress, byte[] memory);

    nuint Scan(int offset, string pattern);
    nuint Scan(int offset, string pattern, string moduleName);
    nuint Scan(int offset, string pattern, Module module);

    nuint Scan(int offset, string pattern, nuint startAddress, nuint size);
    nuint Scan(int offset, string pattern, nuint startAddress, byte[] memory);

    IEnumerable<nuint> ScanAll(ScanPattern pattern);
    IEnumerable<nuint> ScanAll(ScanPattern pattern, nuint size);
    IEnumerable<nuint> ScanAll(ScanPattern pattern, string moduleName);
    IEnumerable<nuint> ScanAll(ScanPattern pattern, string moduleName, nuint size);
    IEnumerable<nuint> ScanAll(ScanPattern pattern, Module module);
    IEnumerable<nuint> ScanAll(ScanPattern pattern, Module module, nuint size);

    IEnumerable<nuint> ScanAll(ScanPattern pattern, nuint startAddress, nuint size);
    IEnumerable<nuint> ScanAll(ScanPattern pattern, nuint startAddress, byte[] memory);

    IEnumerable<nuint> ScanAll(int offset, string pattern);
    IEnumerable<nuint> ScanAll(int offset, string pattern, string moduleName);
    IEnumerable<nuint> ScanAll(int offset, string pattern, Module module);

    IEnumerable<nuint> ScanAll(int offset, string pattern, nuint startAddress, nuint size);
    IEnumerable<nuint> ScanAll(int offset, string pattern, nuint startAddress, byte[] memory);
}
