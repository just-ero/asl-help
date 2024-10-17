using System;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory.Native;
using AslHelp.Shared;

namespace AslHelp.Memory.Ipc;

public partial class ProcessMemory
{
    public void WriteArray<T>(ReadOnlySpan<T> values, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        WriteArray(values, MainModule, baseOffset, offsets);
    }

    public void WriteArray<T>(ReadOnlySpan<T> values, string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        WriteArray(values, Modules[moduleName], baseOffset, offsets);
    }

    public void WriteArray<T>(ReadOnlySpan<T> values, Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        WriteArray(values, module.Base + baseOffset, offsets);
    }

    public unsafe void WriteArray<T>(ReadOnlySpan<T> values, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        nint deref = Deref(baseAddress, offsets);
        int size = GetNativeSizeOf<T>() * values.Length;

        fixed (T* pValues = values)
        {
            if (!WinInteropWrapper.WriteMemory(_handle, deref, pValues, size))
            {
                string msg = $"Failed to write memory at {(ulong)deref:X}: {WinInteropWrapper.GetLastWin32ErrorMessage()}";
                ThrowHelper.ThrowException(msg);
            }
        }
    }

    public bool TryWriteArray<T>(ReadOnlySpan<T> values, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return TryWriteArray(values, MainModule, baseOffset, offsets);
    }

    public bool TryWriteArray<T>(ReadOnlySpan<T> values, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (moduleName is null)
        {
            return false;
        }

        if (!Modules.TryGetValue(moduleName, out Module? module))
        {
            return false;
        }

        return TryWriteArray(values, module, baseOffset, offsets);
    }

    public bool TryWriteArray<T>(ReadOnlySpan<T> values, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (module is null)
        {
            return false;
        }

        return TryWriteArray(values, module.Base + baseOffset, offsets);
    }

    public unsafe bool TryWriteArray<T>(ReadOnlySpan<T> values, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            return false;
        }

        int size = GetNativeSizeOf<T>() * values.Length;

        fixed (T* pValues = values)
        {
            return WinInteropWrapper.WriteMemory(_handle, deref, pValues, size);
        }
    }
}
