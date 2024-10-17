using System;
using System.Collections.Generic;

using AslHelp.Collections;

namespace AslHelp.Memory.Ipc;

public interface IProcessMemory : IMemoryReader, IMemoryWriter, IMemoryScanner, IDisposable
{
    bool Is64Bit { get; }
    byte PointerSize { get; }

    Module MainModule { get; }
    IReadOnlyKeyedCollection<string, Module> Modules { get; }

    IEnumerable<MemoryRange> GetMemoryPages();

    nint ReadRelative(nint relativeAddress, int instructionSize = 0x4);
}
