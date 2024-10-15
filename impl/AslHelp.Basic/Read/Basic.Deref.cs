using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Memory.Native;
using AslHelp.Shared;

public partial class Basic
{
    public nint Deref(int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return Deref(MainModule.Base + baseOffset, offsets);
    }

    public nint Deref(string moduleName, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return Deref(Modules[moduleName].Base + baseOffset, offsets);
    }

    public nint Deref(Module module, int baseOffset, params int[] offsets)
    {
        return Deref(module.Base + baseOffset, offsets);
    }

    public unsafe nint Deref(nint baseAddress, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(Game);

        if (baseAddress == 0)
        {
            const string Msg = "The provided base address must not be null.";
            ThrowHelper.ThrowArgumentException(Msg, nameof(baseAddress));
        }

        nint result = baseAddress;
        int size = PointerSize;

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
        ThrowHelper.ThrowIfNull(MainModule);

        return TryDeref(out result, MainModule.Base + baseOffset, offsets);
    }

    public bool TryDeref(out nint result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(Modules);

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

        return TryDeref(out result, module.Base + baseOffset, offsets);
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
        ThrowHelper.ThrowIfNull(Game);

        result = 0;

        if (baseAddress == 0)
        {
            return false;
        }

        nint tResult = baseAddress;
        int size = PointerSize;

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
