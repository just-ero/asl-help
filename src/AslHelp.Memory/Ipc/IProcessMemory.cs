using System;
using System.Collections.Generic;
using System.Diagnostics;

using AslHelp.Collections;

namespace AslHelp.Memory.Ipc;

public interface IProcessMemory : IMemoryReader, IMemoryWriter, IMemoryScanner, IDisposable
{
    Process Process { get; }
    bool Is64Bit { get; }
    byte PointerSize { get; }

    Module MainModule { get; }

    IReadOnlyKeyedCollection<string, Module> Modules { get; }

    IEnumerable<MemoryRange> GetMemoryPages(bool allPages = false);

    nuint ReadRelative(nuint relativeAddress, uint instructionSize = 0x4U);
}
