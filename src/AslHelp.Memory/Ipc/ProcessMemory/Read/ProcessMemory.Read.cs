using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory.Native;
using AslHelp.Shared;

namespace AslHelp.Memory.Ipc;

public partial class ProcessMemory
{
    public T Read<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return Read<T>(MainModule, baseOffset, offsets);
    }

    public T Read<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return Read<T>(Modules[moduleName], baseOffset, offsets);
    }

    public T Read<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return Read<T>(module.Base + baseOffset, offsets);
    }

    public unsafe T Read<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        nint deref = Deref(baseAddress, offsets);
        int size = GetNativeSizeOf<T>();

        T result;
        if (!WinInteropWrapper.ReadMemory(_handle, deref, &result, size))
        {
            string msg = $"Failed to read memory at {(ulong)deref:X}: {WinInteropWrapper.GetLastWin32ErrorMessage()}";
            ThrowHelper.ThrowException(msg);
        }

        return result;
    }

    public bool TryRead<T>(out T result, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return TryRead(out result, MainModule, baseOffset, offsets);
    }

    public bool TryRead<T>(out T result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (moduleName is null)
        {
            result = default;
            return false;
        }

        if (!Modules.TryGetValue(moduleName, out Module? module))
        {
            result = default;
            return false;
        }

        return TryRead(out result, module, baseOffset, offsets);
    }

    public bool TryRead<T>(out T result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryRead(out result, module.Base + baseOffset, offsets);
    }

    public unsafe bool TryRead<T>(out T result, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        result = default;

        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            return false;
        }

        int size = GetNativeSizeOf<T>();

        fixed (T* pResult = &result)
        {
            return WinInteropWrapper.ReadMemory(_handle, deref, pResult, size);
        }
    }
}
