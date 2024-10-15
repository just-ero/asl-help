using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Memory.Utils;
using AslHelp.Shared;

public partial class Unity
{
    public List<T> ReadList<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadList<T>(MainModule.Base + baseOffset, offsets);
    }

    public List<T> ReadList<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadList<T>(Modules[moduleName].Base + baseOffset, offsets);
    }

    public List<T> ReadList<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadList<T>(module.Base + baseOffset, offsets);
    }

    public List<T> ReadList<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        nint deref = Read<nint>(baseAddress, offsets);
        int count = Read<int>(deref + (PointerSize * 3));

        List<T> result = new(count);
        ReadArray(CollectionsMarshal<T>.AsSpan(result), deref + (PointerSize * 2), PointerSize * 4);

        return result;
    }

    public bool TryReadList<T>([NotNullWhen(true)] out List<T>? result, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadList(out result, MainModule.Base + baseOffset, offsets);
    }

    public bool TryReadList<T>(
        [NotNullWhen(true)] out List<T>? result,
        [NotNullWhen(true)] string? moduleName,
        int baseOffset,
        params int[] offsets)
        where T : unmanaged
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

        return TryReadList(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadList<T>(
        [NotNullWhen(true)] out List<T>? result,
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

        return TryReadList(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadList<T>([NotNullWhen(true)] out List<T>? result, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        if (!TryRead(out nint deref, baseAddress, offsets))
        {
            result = default;
            return false;
        }

        if (!TryRead(out int count, deref + (PointerSize * 3)))
        {
            result = default;
            return false;
        }

        result = new(count);

        if (!TryReadArray(CollectionsMarshal<T>.AsSpan(result), deref + (PointerSize * 2), PointerSize * 4))
        {
            result = default;
            return false;
        }

        return true;
    }

    public List<string> ReadList(int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadList(MainModule.Base + baseOffset, offsets);
    }

    public List<string> ReadList(string moduleName, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadList(Modules[moduleName].Base + baseOffset, offsets);
    }

    public List<string> ReadList(Module module, int baseOffset, params int[] offsets)
    {
        return ReadList(module.Base + baseOffset, offsets);
    }

    public List<string> ReadList(nint baseAddress, params int[] offsets)
    {

    }

    public bool TryReadList([NotNullWhen(true)] out List<string>? result, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadList(out result, MainModule.Base + baseOffset, offsets);
    }

    public bool TryReadList(
        [NotNullWhen(true)] out List<string>? result,
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

        return TryReadList(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadList(
        [NotNullWhen(true)] out List<string>? result,
        [NotNullWhen(true)] Module? module,
        int baseOffset,
        params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadList(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadList([NotNullWhen(true)] out List<string>? result, nint baseAddress, params int[] offsets)
    {

    }
}
