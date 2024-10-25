using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.GameEngines.Unity.Collections;
using AslHelp.Memory;

namespace AslHelp.GameEngines.Unity.Memory;

public partial class UnityMemory
{
    private int MonoSetTable => PointerSize * 2;
    private int MonoSetLinks => PointerSize * 3;
    private int MonoSetSlots => PointerSize * 4;
    private int MonoSetTouched => PointerSize * 7;
    private int MonoSetCount => (PointerSize * 7) + (sizeof(int) * 2);

    private int NetFxSetBuckets => PointerSize * 2;
    private int NetFxSetSlots => PointerSize * 3;
    private int NetFxSetCount => PointerSize * 6;
    private int NetFxSetLastIndex => (PointerSize * 6) + sizeof(int);

    public ISet<T>? ReadSet<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadSet<T>(MainModule, baseOffset, offsets);
    }

    public ISet<T>? ReadSet<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadSet<T>(Modules[moduleName], baseOffset, offsets);
    }

    public ISet<T>? ReadSet<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadSet<T>(module.Base + baseOffset, offsets);
    }

    public ISet<T>? ReadSet<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        nint deref = Read<nint>(baseAddress, offsets);
        if (deref == 0)
        {
            return null;
        }

        if (_version == RuntimeVersion.Mono)
        {
            var table = ReadArray<int>(deref + MonoSetTable);
            var links = ReadArray<Link>(deref + MonoSetLinks);
            var slots = ReadArray<T>(deref + MonoSetSlots);

            var touched = Read<int>(deref + MonoSetTouched);
            var count = Read<int>(deref + MonoSetCount);

            return new MonoHashSet<T>(table!, links!, slots!, touched, count);
        }
        else
        {
            if (!Is64Bit)
            {
                if (IsNativeInt<T>())
                {
                    throw new NotImplementedException();
                }
            }

            var buckets = ReadArray<int>(deref + NetFxSetBuckets);
            var slots = ReadArray<NetFxHashSet<T>.Slot>(deref + NetFxSetSlots);

            var count = Read<int>(deref + NetFxSetCount);
            var lastIndex = Read<int>(deref + NetFxSetLastIndex);

            return new NetFxHashSet<T>(buckets!, slots!, count, lastIndex);
        }
    }

    public bool TryReadSet<T>(out ISet<T>? result, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return TryReadSet(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadSet<T>(out ISet<T>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
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

        return TryReadSet(out result, module, baseOffset, offsets);
    }

    public bool TryReadSet<T>(out ISet<T>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadSet(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadSet<T>(out ISet<T>? result, nint baseAddress, params int[] offsets)
        where T : unmanaged
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

        if (_version == RuntimeVersion.Mono)
        {
            if (!TryReadArray(out int[]? table, deref + MonoSetTable)
                || !TryReadArray(out Link[]? links, deref + MonoSetLinks)
                || !TryReadArray(out T[]? slots, deref + MonoSetSlots)
                || !TryRead(out int touched, deref + MonoSetTouched)
                || !TryRead(out int count, deref + MonoSetCount))
            {
                result = default;
                return false;
            }

            result = new MonoHashSet<T>(table!, links!, slots!, touched, count);
            return true;
        }
        else
        {
            if (!Is64Bit)
            {
                if (IsNativeInt<T>())
                {
                    throw new NotImplementedException();
                }
            }

            if (!TryReadArray(out int[]? buckets, deref + NetFxSetBuckets)
                || !TryReadArray(out NetFxHashSet<T>.Slot[]? slots, deref + NetFxSetSlots)
                || !TryRead(out int count, deref + NetFxSetCount)
                || !TryRead(out int lastIndex, deref + NetFxSetLastIndex))
            {
                result = default;
                return false;
            }

            result = new NetFxHashSet<T>(buckets!, slots!, count, lastIndex);
            return true;
        }
    }

    public ISet<string?>? ReadSet(int baseOffset, params int[] offsets)
    {
        return ReadSet(MainModule, baseOffset, offsets);
    }

    public ISet<string?>? ReadSet(string moduleName, int baseOffset, params int[] offsets)
    {
        return ReadSet(Modules[moduleName], baseOffset, offsets);
    }

    public ISet<string?>? ReadSet(Module module, int baseOffset, params int[] offsets)
    {
        return ReadSet(module.Base + baseOffset, offsets);
    }

    public ISet<string?>? ReadSet(nint baseAddress, params int[] offsets)
    {
        nint deref = Read<nint>(baseAddress, offsets);
        if (deref == 0)
        {
            return null;
        }

        if (_version == RuntimeVersion.Mono)
        {
            var table = ReadArray<int>(deref + MonoSetTable);
            var links = ReadArray<Link>(deref + MonoSetLinks);
            var slots = ReadArray<nint>(deref + MonoSetSlots);

            var touched = Read<int>(deref + MonoSetTouched);
            var count = Read<int>(deref + MonoSetCount);

            return new MonoHashSet(this, table!, links!, slots!, touched, count);
        }
        else
        {
            if (!Is64Bit)
            {
                throw new NotImplementedException();
            }

            var buckets = ReadArray<int>(deref + NetFxSetBuckets);
            var slots = ReadArray<NetFxHashSet<nint>.Slot>(deref + NetFxSetSlots);

            var count = Read<int>(deref + NetFxSetCount);
            var lastIndex = Read<int>(deref + NetFxSetLastIndex);

            return new NetFxHashSet(this, buckets!, slots!, count, lastIndex);
        }
    }

    public bool TryReadSet(out ISet<string?>? result, int baseOffset, params int[] offsets)
    {
        return TryReadSet(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadSet(out ISet<string?>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

        return TryReadSet(out result, module, baseOffset, offsets);
    }

    public bool TryReadSet(out ISet<string?>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadSet(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadSet(out ISet<string?>? result, nint baseAddress, params int[] offsets)
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

        if (_version == RuntimeVersion.Mono)
        {
            if (!TryReadArray(out int[]? table, deref + MonoSetTable)
                || !TryReadArray(out Link[]? links, deref + MonoSetLinks)
                || !TryReadArray(out nint[]? slots, deref + MonoSetSlots)
                || !TryRead(out int touched, deref + MonoSetTouched)
                || !TryRead(out int count, deref + MonoSetCount))
            {
                result = default;
                return false;
            }

            result = new MonoHashSet(this, table!, links!, slots!, touched, count);
            return true;
        }
        else
        {
            if (!Is64Bit)
            {
                throw new NotImplementedException();
            }

            if (!TryReadArray(out int[]? buckets, deref + NetFxSetBuckets)
                || !TryReadArray(out NetFxHashSet<nint>.Slot[]? slots, deref + NetFxSetSlots)
                || !TryRead(out int count, deref + NetFxSetCount)
                || !TryRead(out int lastIndex, deref + NetFxSetLastIndex))
            {
                result = default;
                return false;
            }

            result = new NetFxHashSet(this, buckets!, slots!, count, lastIndex);
            return true;
        }
    }
}
