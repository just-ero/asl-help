using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory.Native;
using AslHelp.Shared;

namespace AslHelp.Memory.Ipc;

public partial class ProcessMemory
{
    public nint Deref(int baseOffset, params int[] offsets)
    {
        return Deref(MainModule, baseOffset, offsets);
    }

    public nint Deref(string moduleName, int baseOffset, params int[] offsets)
    {
        return Deref(Modules[moduleName], baseOffset, offsets);
    }

    public nint Deref(Module module, int baseOffset, params int[] offsets)
    {
        return Deref(module.Base + baseOffset, offsets);
    }

    public unsafe nint Deref(nint baseAddress, params int[] offsets)
    {
        if (_disposed)
        {
            const string Msg = "Cannot interact with the process memory after it has been disposed.";
            ThrowHelper.ThrowObjectDisposedException(Msg);
        }

        if (baseAddress == 0)
        {
            const string Msg = "The provided base address must not be null.";
            ThrowHelper.ThrowArgumentException(Msg, nameof(baseAddress));
        }

        nint result = baseAddress;
        nint size = PointerSize;

        for (int i = 0; i < offsets.Length; i++)
        {
            if (!WinInteropWrapper.ReadMemory(_handle, result, &result, size))
            {
                string msg = $"Failed to dereference address at {(ulong)result:X}: {WinInteropWrapper.GetLastWin32ErrorMessage()}";
                ThrowHelper.ThrowException(msg);
            }

            if (result == 0)
            {
                const string Msg = "Dereference resulted in a null pointer.";
                ThrowHelper.ThrowException(Msg);
            }

            result += offsets[i];
        }

        return result;
    }

    public bool TryDeref(out nint result, int baseOffset, params int[] offsets)
    {
        return TryDeref(out result, MainModule, baseOffset, offsets);
    }

    public bool TryDeref(out nint result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

        return TryDeref(out result, module, baseOffset, offsets);
    }

    public bool TryDeref(out nint result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryDeref(out result, module.Base + baseOffset, offsets);
    }

    public unsafe bool TryDeref(out nint result, nint baseAddress, params int[] offsets)
    {
        result = default;

        if (_disposed)
        {
            return false;
        }

        if (baseAddress == 0)
        {
            return false;
        }

        nint tResult = baseAddress;
        nint size = PointerSize;

        for (int i = 0; i < offsets.Length; i++)
        {
            if (!WinInteropWrapper.ReadMemory(_handle, tResult, &tResult, size))
            {
                return false;
            }

            if (tResult == 0)
            {
                return false;
            }

            tResult += offsets[i];
        }

        result = tResult;
        return true;
    }
}
