using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Shared;

public partial class Unity
{
    public T[] ReadArray<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadArray<T>(MainModule, baseOffset, offsets);
    }

    public T[] ReadArray<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadArray<T>(Modules[moduleName], baseOffset, offsets);
    }

    public T[] ReadArray<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadArray<T>(module.Base + baseOffset, offsets);
    }

    public T[] ReadArray<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        nint deref = Read<nint>(baseAddress, offsets);
        int length = Read<int>(deref + (PointerSize * 3));
        return ReadArray<T>(length, deref + (PointerSize * 4));
    }

    public bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (MainModule is null)
        {
            result = default;
            return false;
        }

        return TryReadArray(out result, MainModule.Base + baseOffset, offsets);
    }

    public bool TryReadArray<T>(
        [NotNullWhen(true)] out T[]? result,
        [NotNullWhen(true)] string? moduleName,
        int baseOffset,
        params int[] offsets)
        where T : unmanaged
    {
        if (moduleName is null)
        {
            result = default;
            return false;
        }

        if (Modules is null)
        {
            result = default;
            return false;
        }

        if (!Modules.TryGetValue(moduleName, out Module? module))
        {
            result = default;
            return false;
        }

        return TryReadArray(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadArray<T>(
        [NotNullWhen(true)] out T[]? result,
        [NotNullWhen(true)] Module? module,
        int baseOffset,
        params int[] offsets)
        where T : unmanaged
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadArray(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        if (!TryRead(out nint deref, baseAddress, offsets))
        {
            result = default;
            return false;
        }

        if (!TryRead(out int length, deref + (PointerSize * 3)))
        {
            result = default;
            return false;
        }

        return TryReadArray(out result, length, deref + (PointerSize * 4));
    }

    public string[] ReadArray(int length, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadArray(length, MainModule.Base + baseOffset, offsets);
    }

    public string[] ReadArray(int length, string moduleName, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadArray(length, Modules[moduleName].Base + baseOffset, offsets);
    }

    public string[] ReadArray(int length, Module module, int baseOffset, params int[] offsets)
    {
        return ReadArray(length, module.Base + baseOffset, offsets);
    }

    public string[] ReadArray(int length, nint baseAddress, params int[] offsets)
    {

    }

    public bool TryReadArray([NotNullWhen(true)] out string[]? result, int length, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadArray(out result, length, MainModule.Base + baseOffset, offsets);
    }

    public bool TryReadArray(
        [NotNullWhen(true)] out string[]? result,
        int length,
        [NotNullWhen(true)] string? moduleName,
        int baseOffset,
        params int[] offsets)
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

        return TryReadArray(out result, length, module.Base + baseOffset, offsets);
    }

    public bool TryReadArray(
        [NotNullWhen(true)] out string[]? result,
        int length,
        [NotNullWhen(true)] Module? module,
        int baseOffset,
        params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadArray(out result, length, module.Base + baseOffset, offsets);
    }

    public bool TryReadArray([NotNullWhen(true)] out string[]? result, int length, nint baseAddress, params int[] offsets)
    {

    }
}
