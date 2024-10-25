using System.Collections.Generic;

using AslHelp.Memory;

public partial class Unity
{
    public IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<TKey, TValue>? result, baseOffset, offsets);
        return result ?? new Dictionary<TKey, TValue>();
    }

    public IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(string moduleName, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<TKey, TValue>? result, moduleName, baseOffset, offsets);
        return result ?? new Dictionary<TKey, TValue>();
    }

    public IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(Module module, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<TKey, TValue>? result, module, baseOffset, offsets);
        return result ?? new Dictionary<TKey, TValue>();
    }

    public IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(nint baseAddress, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<TKey, TValue>? result, baseAddress, offsets);
        return result ?? new Dictionary<TKey, TValue>();
    }

    public IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<string, TValue>? result, baseOffset, offsets);
        return result ?? new Dictionary<string, TValue>();
    }

    public IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(string moduleName, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<string, TValue>? result, moduleName, baseOffset, offsets);
        return result ?? new Dictionary<string, TValue>();
    }

    public IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(Module module, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<string, TValue>? result, module, baseOffset, offsets);
        return result ?? new Dictionary<string, TValue>();
    }

    public IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(nint baseAddress, params int[] offsets)
        where TValue : unmanaged
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<string, TValue>? result, baseAddress, offsets);
        return result ?? new Dictionary<string, TValue>();
    }

    public IReadOnlyDictionary<string, string?> ReadDictionary(int baseOffset, params int[] offsets)
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, baseOffset, offsets);
        return result ?? new Dictionary<string, string?>();
    }

    public IReadOnlyDictionary<string, string?> ReadDictionary(string moduleName, int baseOffset, params int[] offsets)
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, moduleName, baseOffset, offsets);
        return result ?? new Dictionary<string, string?>();
    }

    public IReadOnlyDictionary<string, string?> ReadDictionary(Module module, int baseOffset, params int[] offsets)
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, module, baseOffset, offsets);
        return result ?? new Dictionary<string, string?>();
    }

    public IReadOnlyDictionary<string, string?> ReadDictionary(nint baseAddress, params int[] offsets)
    {
        Memory.TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, baseAddress, offsets);
        return result ?? new Dictionary<string, string?>();
    }
}
