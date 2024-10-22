using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.GameEngines.Unity.Collections;
using AslHelp.Memory;

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

        if (_version == RuntimeVersion.Mono)
        {
            var table = ReadArray<int>(deref + (PointerSize * 2));
            var links = ReadArray<Link>(deref + (PointerSize * 3));
            var keys = ReadArray<TKey>(deref + (PointerSize * 4));
            var values = ReadArray<TValue>(deref + (PointerSize * 5));

            var touched = Read<int>(deref + (PointerSize * 6));
            var count = Read<int>(deref + (PointerSize * 6) + (sizeof(int) * 2));

            return new MonoDictionary<TKey, TValue>(table, links, keys, values, touched, count);
        }
        else
        {
            if (!Is64Bit)
            {
                if (IsNativeInt<TKey>())
                {
                    if (IsNativeInt<TValue>())
                    {
                        var buckets = ReadArray<int>(deref + (PointerSize * 2));
                        var entries = ReadArray<NetFxDictionary<int, int>.Entry>(deref + (PointerSize * 3));

                        var count = Read<int>(deref + (PointerSize * 8));

                        return new NetFxDictionary<int, int>(buckets, entries, count);
                    }
                    else
                    {
                        var buckets = ReadArray<int>(deref + (PointerSize * 2));
                        var entries = ReadArray<NetFxDictionary<int, TValue>.Entry>(deref + (PointerSize * 3));

                        var count = Read<int>(deref + (PointerSize * 8));

                        return new NetFxDictionary<int, TValue>(buckets, entries, count);
                    }
                }
            }

#error Holy fuck how do you do nint.

            var buckets = ReadArray<int>(deref + (PointerSize * 2));
            var entries = ReadArray<NetFxDictionary<TKey, TValue>.Entry>(deref + (PointerSize * 3));

            var count = Read<int>(deref + (PointerSize * 8));

            return new NetFxDictionary<TKey, TValue>(buckets, entries, count);
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
        if (!TryRead(out nint deref, baseAddress, offsets))
        {
            result = default;
            return false;
        }

        if (_version == RuntimeVersion.Mono)
        {
            if (!TryReadArray(out int[]? table, deref + (PointerSize * 2))
                || !TryReadArray(out Link[]? links, deref + (PointerSize * 3))
                || !TryReadArray(out TKey[]? keys, deref + (PointerSize * 4))
                || !TryReadArray(out TValue[]? values, deref + (PointerSize * 5))
                || !TryRead(out int touched, deref + (PointerSize * 6))
                || !TryRead(out int count, deref + (PointerSize * 6) + (sizeof(int) * 2)))
            {
                result = default;
                return false;
            }

            result = new MonoDictionary<TKey, TValue>(table, links, keys, values, touched, count);
            return true;
        }
        else
        {
#error Holy fuck how do you do nint.

            if (!TryReadArray(out int[]? buckets, deref + (PointerSize * 2))
                || !TryReadArray(out NetFxDictionary<TKey, TValue>.Entry[]? entries, deref + (PointerSize * 3))
                || !TryRead(out int count, deref + (PointerSize * 8)))
            {
                result = default;
                return false;
            }

            result = new NetFxDictionary<TKey, TValue>(buckets, entries, count);
            return true;
        }
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
