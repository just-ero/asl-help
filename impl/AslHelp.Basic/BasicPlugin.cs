using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using AslHelp.Collections;
using AslHelp.Memory;
using AslHelp.Memory.Ipc;

public partial class Basic : IProcessMemory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static bool IsNativeInt<T>()
    {
        return typeof(T) == typeof(nint) || typeof(T) == typeof(nuint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected unsafe int GetNativeSizeOf<T>()
        where T : unmanaged
    {
        return IsNativeInt<T>() ? PointerSize : sizeof(T);
    }

    private nint _handle;
    private bool _disposed;

    public Process Process => throw new NotImplementedException();

    public bool Is64Bit => throw new NotImplementedException();

    public byte PointerSize => throw new NotImplementedException();

    public Module MainModule => throw new NotImplementedException();

    public IReadOnlyKeyedCollection<string, Module> Modules => throw new NotImplementedException();

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<MemoryRange> GetMemoryPages(bool allPages = false)
    {
        throw new NotImplementedException();
    }

    public nint ReadRelative(nint relativeAddress, uint instructionSize = 4)
    {
        throw new NotImplementedException();
    }
}
