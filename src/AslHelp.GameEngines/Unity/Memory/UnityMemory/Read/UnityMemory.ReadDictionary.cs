using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.GameEngines.Unity.Collections;
using AslHelp.Memory;

namespace AslHelp.GameEngines.Unity.Memory;

public partial class UnityMemory
{
    private int MonoDictTable => PointerSize * 2;
    private int MonoDictLinks => PointerSize * 3;
    private int MonoDictKeys => PointerSize * 4;
    private int MonoDictValues => PointerSize * 5;
    private int MonoDictTouched => PointerSize * 6;
    private int MonoDictCount => (PointerSize * 6) + (sizeof(int) * 2);

    private int NetFxDictBuckets => PointerSize * 2;
    private int NetFxDictEntries => PointerSize * 3;
    private int NetFxDictCount => PointerSize * 8;

    public IReadOnlyDictionary<TKey, TValue>? ReadDictionary<TKey, TValue>(int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return ReadDictionary<TKey, TValue>(MainModule, baseOffset, offsets);
    }

    public IReadOnlyDictionary<TKey, TValue>? ReadDictionary<TKey, TValue>(string moduleName, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return ReadDictionary<TKey, TValue>(Modules[moduleName], baseOffset, offsets);
    }

    public IReadOnlyDictionary<TKey, TValue>? ReadDictionary<TKey, TValue>(Module module, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return ReadDictionary<TKey, TValue>(module.Base + baseOffset, offsets);
    }

    public unsafe IReadOnlyDictionary<TKey, TValue>? ReadDictionary<TKey, TValue>(nint baseAddress, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        nint deref = Read<nint>(baseAddress, offsets);
        if (deref == 0)
        {
            return null;
        }

        if (_version == DotnetRuntimeVersion.Mono)
        {
            var table = ReadArray<int>(deref + MonoDictTable);
            var links = ReadArray<Link>(deref + MonoDictLinks);
            var keys = ReadArray<TKey>(deref + MonoDictKeys);
            var values = ReadArray<TValue>(deref + MonoDictValues);

            var touched = Read<int>(deref + MonoDictTouched);
            var count = Read<int>(deref + MonoDictCount);

            return new MonoDictionary<TKey, TValue>(table!, links!, keys!, values!, touched, count);
        }
        else
        {
            if (!Is64Bit)
            {
                if (IsNativeInt<TKey>())
                {
                    if (IsNativeInt<TValue>())
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else if (IsNativeInt<TValue>())
                {
                    throw new NotImplementedException();
                }
            }

            var buckets = ReadArray<int>(deref + NetFxDictBuckets);
            var entries = ReadArray<NetFxDictionary<TKey, TValue>.Entry>(deref + NetFxDictEntries);
            var count = Read<int>(deref + NetFxDictCount);

            return new NetFxDictionary<TKey, TValue>(buckets!, entries!, count);
        }
    }

    public bool TryReadDictionary<TKey, TValue>(out IReadOnlyDictionary<TKey, TValue>? result, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return TryReadDictionary(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadDictionary<TKey, TValue>(out IReadOnlyDictionary<TKey, TValue>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

    public bool TryReadDictionary<TKey, TValue>(out IReadOnlyDictionary<TKey, TValue>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
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

    public bool TryReadDictionary<TKey, TValue>(out IReadOnlyDictionary<TKey, TValue>? result, nint baseAddress, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        if (!TryRead(out nint deref, baseAddress, offsets))
        {
            result = default;
            return false;
        }

        if (deref == 0)
        {
            result = null;
            return true;
        }

        if (_version == DotnetRuntimeVersion.Mono)
        {
            if (!TryReadArray(out int[]? table, deref + MonoDictTable)
                || !TryReadArray(out Link[]? links, deref + MonoDictLinks)
                || !TryReadArray(out TKey[]? keys, deref + MonoDictKeys)
                || !TryReadArray(out TValue[]? values, deref + MonoDictValues)
                || !TryRead(out int touched, deref + MonoDictTouched)
                || !TryRead(out int count, deref + MonoDictCount))
            {
                result = default;
                return false;
            }

            result = new MonoDictionary<TKey, TValue>(table!, links!, keys!, values!, touched, count);
            return true;
        }
        else
        {
            if (!Is64Bit)
            {
                if (IsNativeInt<TKey>())
                {
                    if (IsNativeInt<TValue>())
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else if (IsNativeInt<TValue>())
                {
                    throw new NotImplementedException();
                }
            }

            if (!TryReadArray(out int[]? buckets, deref + NetFxDictBuckets)
                || !TryReadArray(out NetFxDictionary<TKey, TValue>.Entry[]? entries, deref + NetFxDictEntries)
                || !TryRead(out int count, deref + NetFxDictCount))
            {
                result = default;
                return false;
            }

            result = new NetFxDictionary<TKey, TValue>(buckets!, entries!, count);
            return true;
        }
    }

    public IReadOnlyDictionary<string, TValue>? ReadDictionary<TValue>(int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        return ReadDictionary<TValue>(MainModule, baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, TValue>? ReadDictionary<TValue>(string moduleName, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        return ReadDictionary<TValue>(Modules[moduleName], baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, TValue>? ReadDictionary<TValue>(Module module, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        return ReadDictionary<TValue>(module.Base + baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, TValue>? ReadDictionary<TValue>(nint baseAddress, params int[] offsets)
        where TValue : unmanaged
    {
        nint deref = Read<nint>(baseAddress, offsets);
        if (deref == 0)
        {
            return null;
        }

        if (_version == DotnetRuntimeVersion.Mono)
        {
            var table = ReadArray<int>(deref + MonoDictTable);
            var links = ReadArray<Link>(deref + MonoDictLinks);
            var keys = ReadArray<nint>(deref + MonoDictKeys);
            var values = ReadArray<TValue>(deref + MonoDictValues);

            var touched = Read<int>(deref + MonoDictTouched);
            var count = Read<int>(deref + MonoDictCount);

            return new MonoDictionary<TValue>(this, table!, links!, keys!, values!, touched, count);
        }
        else
        {
            if (!Is64Bit)
            {
                if (IsNativeInt<TValue>())
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            var buckets = ReadArray<int>(deref + NetFxDictBuckets);
            var entries = ReadArray<NetFxDictionary<TValue>.Entry>(deref + NetFxDictEntries);
            var count = Read<int>(deref + NetFxDictCount);

            return new NetFxDictionary<TValue>(this, buckets!, entries!, count);
        }
    }

    public bool TryReadDictionary<TValue>(out IReadOnlyDictionary<string, TValue>? result, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        return TryReadDictionary(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadDictionary<TValue>(out IReadOnlyDictionary<string, TValue>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

    public bool TryReadDictionary<TValue>(out IReadOnlyDictionary<string, TValue>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
        where TValue : unmanaged
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadDictionary(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary<TValue>(out IReadOnlyDictionary<string, TValue>? result, nint baseAddress, params int[] offsets)
        where TValue : unmanaged
    {
        if (!TryRead(out nint deref, baseAddress, offsets))
        {
            result = default;
            return false;
        }

        if (deref == 0)
        {
            result = null;
            return true;
        }

        if (_version == DotnetRuntimeVersion.Mono)
        {
            if (!TryReadArray(out int[]? table, deref + MonoDictTable)
                || !TryReadArray(out Link[]? links, deref + MonoDictLinks)
                || !TryReadArray(out nint[]? keys, deref + MonoDictKeys)
                || !TryReadArray(out TValue[]? values, deref + MonoDictValues)
                || !TryRead(out int touched, deref + MonoDictTouched)
                || !TryRead(out int count, deref + MonoDictCount))
            {
                result = default;
                return false;
            }

            result = new MonoDictionary<TValue>(this, table!, links!, keys!, values!, touched, count);
            return true;
        }
        else
        {
            if (!Is64Bit)
            {
                if (IsNativeInt<TValue>())
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            if (!TryReadArray(out int[]? buckets, deref + NetFxDictBuckets)
                || !TryReadArray(out NetFxDictionary<TValue>.Entry[]? entries, deref + NetFxDictEntries)
                || !TryRead(out int count, deref + NetFxDictCount))
            {
                result = default;
                return false;
            }

            result = new NetFxDictionary<TValue>(this, buckets!, entries!, count);
            return true;
        }
    }

    public IReadOnlyDictionary<string, string?>? ReadDictionary(int baseOffset, params int[] offsets)
    {
        return ReadDictionary(MainModule, baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, string?>? ReadDictionary(string moduleName, int baseOffset, params int[] offsets)
    {
        return ReadDictionary(Modules[moduleName], baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, string?>? ReadDictionary(Module module, int baseOffset, params int[] offsets)
    {
        return ReadDictionary(module.Base + baseOffset, offsets);
    }

    public IReadOnlyDictionary<string, string?>? ReadDictionary(nint baseAddress, params int[] offsets)
    {
        nint deref = Read<nint>(baseAddress, offsets);
        if (deref == 0)
        {
            return null;
        }

        if (_version == DotnetRuntimeVersion.Mono)
        {
            var table = ReadArray<int>(deref + MonoDictTable);
            var links = ReadArray<Link>(deref + MonoDictLinks);
            var keys = ReadArray<nint>(deref + MonoDictKeys);
            var values = ReadArray<nint>(deref + MonoDictValues);

            var touched = Read<int>(deref + MonoDictTouched);
            var count = Read<int>(deref + MonoDictCount);

            return new MonoDictionary(this, table!, links!, keys!, values!, touched, count);
        }
        else
        {
            if (!Is64Bit)
            {
                throw new NotImplementedException();
            }

            var buckets = ReadArray<int>(deref + NetFxDictBuckets);
            var entries = ReadArray<NetFxDictionary.Entry>(deref + NetFxDictEntries);
            var count = Read<int>(deref + NetFxDictCount);

            return new NetFxDictionary(this, buckets!, entries!, count);
        }
    }

    public bool TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, int baseOffset, params int[] offsets)
    {
        return TryReadDictionary(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

    public bool TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadDictionary(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, nint baseAddress, params int[] offsets)
    {
        if (!TryRead(out nint deref, baseAddress, offsets))
        {
            result = default;
            return false;
        }

        if (deref == 0)
        {
            result = null;
            return true;
        }

        if (_version == DotnetRuntimeVersion.Mono)
        {
            if (!TryReadArray(out int[]? table, deref + MonoDictTable)
                || !TryReadArray(out Link[]? links, deref + MonoDictLinks)
                || !TryReadArray(out nint[]? keys, deref + MonoDictKeys)
                || !TryReadArray(out nint[]? values, deref + MonoDictValues)
                || !TryRead(out int touched, deref + MonoDictTouched)
                || !TryRead(out int count, deref + MonoDictCount))
            {
                result = default;
                return false;
            }

            result = new MonoDictionary(this, table!, links!, keys!, values!, touched, count);
            return true;
        }
        else
        {
            if (!Is64Bit)
            {
                throw new NotImplementedException();
            }

            if (!TryReadArray(out int[]? buckets, deref + NetFxDictBuckets)
                || !TryReadArray(out NetFxDictionary.Entry[]? entries, deref + NetFxDictEntries)
                || !TryRead(out int count, deref + NetFxDictCount))
            {
                result = default;
                return false;
            }

            result = new NetFxDictionary(this, buckets!, entries!, count);
            return true;
        }
    }
}
