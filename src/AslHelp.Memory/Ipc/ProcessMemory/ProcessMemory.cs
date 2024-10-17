using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

using AslHelp.Collections;
using AslHelp.Memory.Native;
using AslHelp.Memory.Native.Enums;

namespace AslHelp.Memory.Ipc;

public partial class ProcessMemory : IProcessMemory
{
    private const ProcessAccess VmAccess =
        ProcessAccess.VmOperation
        | ProcessAccess.VmRead
        | ProcessAccess.VmWrite
        | ProcessAccess.QueryInformation;

    protected readonly nint _handle;

    private bool _disposed;

    public ProcessMemory(Process process)
    {
        _handle = WinInterop.OpenProcess(process.Id, VmAccess, false);

        Is64Bit = WinInteropWrapper.ProcessIs64Bit(_handle);
        PointerSize = (byte)(Is64Bit ? 0x8 : 0x4);

        Modules = new ModuleCollection(process);
        MainModule = Modules.First();
    }

    public bool Is64Bit { get; }
    public byte PointerSize { get; }

    public Module MainModule { get; }
    public IReadOnlyKeyedCollection<string, Module> Modules { get; }

    public IEnumerable<MemoryRange> GetMemoryPages()
    {
        return WinInteropWrapper.GetMemoryPages(_handle, Is64Bit);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        WinInterop.CloseHandle(_handle);

        _disposed = true;
    }

    public nint ReadRelative(nint relativeAddress, int instructionSize = 0x4)
    {
        if (Is64Bit)
        {
            return relativeAddress + instructionSize + Read<int>(relativeAddress);
        }
        else
        {
            return Read<nint>(relativeAddress);
        }
    }

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
}
