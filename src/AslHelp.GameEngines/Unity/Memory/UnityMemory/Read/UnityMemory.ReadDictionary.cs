using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Unity.Collections;

namespace AslHelp.GameEngines.Unity.Memory;

public partial class UnityMemory
{
    public IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return ReadDictionary<TKey, TValue>(MainModule, baseOffset, offsets);
    }

    public IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(string moduleName, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return ReadDictionary<TKey, TValue>(Modules[moduleName], baseOffset, offsets);
    }

    public IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(Module module, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return ReadDictionary<TKey, TValue>(module.Base + baseOffset, offsets);
    }

    public IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(nint baseAddress, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        nint deref = Read<nint>(baseAddress, offsets);

        if (_version == DotnetRuntimeVersion.Net35)
        {
            int count = Read<int>(deref + (PointerSize * 6) + (sizeof(int) * 2));

            var table = ReadArray<int>(deref + (PointerSize * 2));
            var links = ReadArray<Link>(deref + (PointerSize * 3));
            var keys = ReadArray<TKey>(deref + (PointerSize * 4));
            var values = ReadArray<TValue>(deref + (PointerSize * 5));

            return new Net35Dictionary<TKey, TValue>(count, table, links, keys, values);
        }
        else
        {
            int count = Read<int>(deref + (PointerSize * 8));

            var buckets = ReadArray<int>(deref + (PointerSize * 2));
            var entries = ReadArray<Net40Dictionary<TKey, TValue>.Entry>(deref + (PointerSize * 3));

            return new Net40Dictionary<TKey, TValue>(count, buckets, entries);
        }
    }

    public bool TryReadDictionary<TKey, TValue>([NotNullWhen(true)] out IReadOnlyDictionary<TKey, TValue>? result, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return TryReadDictionary(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadDictionary<TKey, TValue>([NotNullWhen(true)] out IReadOnlyDictionary<TKey, TValue>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
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

        return TryReadDictionary(out result, module, baseOffset, offsets);
    }

    public bool TryReadDictionary<TKey, TValue>([NotNullWhen(true)] out IReadOnlyDictionary<TKey, TValue>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
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

    public bool TryReadDictionary<TKey, TValue>([NotNullWhen(true)] out IReadOnlyDictionary<TKey, TValue>? result, nint baseAddress, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        return ReadDictionary<TValue>(MainModule, baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(string moduleName, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        return ReadDictionary<TValue>(Modules[moduleName], baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(Module module, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        return ReadDictionary<TValue>(module.Base + baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(nint baseAddress, params int[] offsets)
        where TValue : unmanaged
    {
        throw new NotImplementedException();
    }

    public bool TryReadDictionary<TValue>([NotNullWhen(true)] out IReadOnlyDictionary<string, TValue>? result, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        return TryReadDictionary(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadDictionary<TValue>([NotNullWhen(true)] out IReadOnlyDictionary<string, TValue>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
        where TValue : unmanaged
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

        return TryReadDictionary(out result, module, baseOffset, offsets);
    }

    public bool TryReadDictionary<TValue>([NotNullWhen(true)] out IReadOnlyDictionary<string, TValue>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadDictionary(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary<TValue>([NotNullWhen(true)] out IReadOnlyDictionary<string, TValue>? result, nint baseAddress, params int[] offsets)
        where TValue : unmanaged
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<string, string> ReadDictionary(int baseOffset, params int[] offsets)
    {
        return ReadDictionary(MainModule, baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, string> ReadDictionary(string moduleName, int baseOffset, params int[] offsets)
    {
        return ReadDictionary(Modules[moduleName], baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, string> ReadDictionary(Module module, int baseOffset, params int[] offsets)
    {
        return ReadDictionary(module.Base + baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, string> ReadDictionary(nint baseAddress, params int[] offsets)
    {
        throw new NotImplementedException();
    }

    public bool TryReadDictionary([NotNullWhen(true)] out IReadOnlyDictionary<string, string>? result, int baseOffset, params int[] offsets)
    {
        return TryReadDictionary(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadDictionary([NotNullWhen(true)] out IReadOnlyDictionary<string, string>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

        return TryReadDictionary(out result, module, baseOffset, offsets);
    }

    public bool TryReadDictionary([NotNullWhen(true)] out IReadOnlyDictionary<string, string>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadDictionary(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary([NotNullWhen(true)] out IReadOnlyDictionary<string, string>? result, nint baseAddress, params int[] offsets)
    {
        throw new NotImplementedException();
    }
}
