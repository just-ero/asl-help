using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Shared;

public partial class Unity
{
    public ISet<T> ReadHashSet<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadHashSet<T>(MainModule.Base + baseOffset, offsets);
    }

    public ISet<T> ReadHashSet<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadHashSet<T>(Modules[moduleName].Base + baseOffset, offsets);
    }

    public ISet<T> ReadHashSet<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadHashSet<T>(module.Base + baseOffset, offsets);
    }

    public ISet<T> ReadHashSet<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {

    }

    public bool TryReadHashSet<T>([NotNullWhen(true)] out HashSet<T>? result, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadHashSet(out result, MainModule.Base + baseOffset, offsets);
    }

    public bool TryReadHashSet<T>(
        [NotNullWhen(true)] out HashSet<T>? result,
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

        return TryReadHashSet(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadHashSet<T>(
        [NotNullWhen(true)] out HashSet<T>? result,
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

        return TryReadHashSet(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadHashSet<T>([NotNullWhen(true)] out HashSet<T>? result, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {

    }

    public ISet<string> ReadHashSet(int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadHashSet(MainModule.Base + baseOffset, offsets);
    }

    public ISet<string> ReadHashSet(string moduleName, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadHashSet(Modules[moduleName].Base + baseOffset, offsets);
    }

    public ISet<string> ReadHashSet(Module module, int baseOffset, params int[] offsets)
    {
        return ReadHashSet(module.Base + baseOffset, offsets);
    }

    public ISet<string> ReadHashSet(nint baseAddress, params int[] offsets)
    {

    }

    public bool TryReadHashSet([NotNullWhen(true)] out HashSet<string>? result, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadHashSet(out result, MainModule.Base + baseOffset, offsets);
    }

    public bool TryReadHashSet(
        [NotNullWhen(true)] out HashSet<string>? result,
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

        return TryReadHashSet(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadHashSet(
        [NotNullWhen(true)] out HashSet<string>? result,
        [NotNullWhen(true)] Module? module,
        int baseOffset,
        params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadHashSet(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadHashSet([NotNullWhen(true)] out HashSet<string>? result, nint baseAddress, params int[] offsets)
    {

    }
}
