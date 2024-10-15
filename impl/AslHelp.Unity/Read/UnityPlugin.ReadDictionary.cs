using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Shared;

public partial class Unity
{
    public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadDictionary<TKey, TValue>(MainModule.Base + baseOffset, offsets);
    }

    public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(string moduleName, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadDictionary<TKey, TValue>(Modules[moduleName].Base + baseOffset, offsets);
    }

    public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(Module module, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return ReadDictionary<TKey, TValue>(module.Base + baseOffset, offsets);
    }

    public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(nint baseAddress, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {

    }

    public bool TryReadDictionary<TKey, TValue>(
        [NotNullWhen(true)] out Dictionary<TKey, TValue>? result,
        int baseOffset,
        params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadDictionary(out result, MainModule.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary<TKey, TValue>(
        [NotNullWhen(true)] out Dictionary<TKey, TValue>? result,
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

        return TryReadDictionary(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary<TKey, TValue>(
        [NotNullWhen(true)] out Dictionary<TKey, TValue>? result,
        [NotNullWhen(true)] Module? module,
        int baseOffset,
        params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadDictionary(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary<TKey, TValue>(
        [NotNullWhen(true)] out Dictionary<TKey, TValue>? result,
        nint baseAddress,
        params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {

    }

    public Dictionary<string, TValue> ReadDictionary<TValue>(int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadDictionary<TValue>(MainModule.Base + baseOffset, offsets);
    }

    public Dictionary<string, TValue> ReadDictionary<TValue>(string moduleName, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadDictionary<TValue>(Modules[moduleName].Base + baseOffset, offsets);
    }

    public Dictionary<string, TValue> ReadDictionary<TValue>(Module module, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        return ReadDictionary<TValue>(module.Base + baseOffset, offsets);
    }

    public Dictionary<string, TValue> ReadDictionary<TValue>(nint baseAddress, params int[] offsets)
        where TValue : unmanaged
    {

    }

    public bool TryReadDictionary<TValue>(
        [NotNullWhen(true)] out Dictionary<string, TValue>? result,
        int baseOffset,
        params int[] offsets)
        where TValue : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadDictionary(out result, MainModule.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary<TValue>(
        [NotNullWhen(true)] out Dictionary<string, TValue>? result,
        [NotNullWhen(true)] string? moduleName,
        int baseOffset,
        params int[] offsets)
        where TValue : unmanaged
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

        return TryReadDictionary(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary<TValue>(
        [NotNullWhen(true)] out Dictionary<string, TValue>? result,
        [NotNullWhen(true)] Module? module,
        int baseOffset,
        params int[] offsets)
        where TValue : unmanaged
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadDictionary(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary<TValue>(
        [NotNullWhen(true)] out Dictionary<string, TValue>? result,
        nint baseAddress,
        params int[] offsets)
        where TValue : unmanaged
    {

    }

    public Dictionary<string, string> ReadDictionary(int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadDictionary(MainModule.Base + baseOffset, offsets);
    }

    public Dictionary<string, string> ReadDictionary(string moduleName, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadDictionary(Modules[moduleName].Base + baseOffset, offsets);
    }

    public Dictionary<string, string> ReadDictionary(Module module, int baseOffset, params int[] offsets)
    {
        return ReadDictionary(module.Base + baseOffset, offsets);
    }

    public Dictionary<string, string> ReadDictionary(nint baseAddress, params int[] offsets)
    {

    }

    public bool TryReadDictionary(
        [NotNullWhen(true)] out Dictionary<string, string>? result,
        int baseOffset,
        params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadDictionary(out result, MainModule.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary(
        [NotNullWhen(true)] out Dictionary<string, string>? result,
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

        return TryReadDictionary(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary(
        [NotNullWhen(true)] out Dictionary<string, string>? result,
        [NotNullWhen(true)] Module? module,
        int baseOffset,
        params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadDictionary(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary(
        [NotNullWhen(true)] out Dictionary<string, string>? result,
        nint baseAddress,
        params int[] offsets)
    {

    }
}
