using System;

using AslHelp.Memory;
using AslHelp.Memory.Native;
using AslHelp.Shared;

public partial class Basic
{
    public void WriteArray<T>(ReadOnlySpan<T> values, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        WriteArray(values, MainModule.Base + baseOffset, offsets);
    }

    public void WriteArray<T>(ReadOnlySpan<T> values, string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(Modules);

        WriteArray(values, Modules[moduleName].Base + baseOffset, offsets);
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
        ThrowHelper.ThrowIfNull(MainModule);

        return TryWriteArray(values, MainModule.Base + baseOffset, offsets);
    }

    public bool TryWriteArray<T>(ReadOnlySpan<T> values, string? moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(Modules);

        if (moduleName is null)
        {
            return false;
        }

        if (!Modules.TryGetValue(moduleName, out Module? module))
        {
            return false;
        }

        return TryWriteArray(values, module.Base + baseOffset, offsets);
    }

    public bool TryWriteArray<T>(ReadOnlySpan<T> values, Module? module, int baseOffset, params int[] offsets)
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
