using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;

namespace AslHelp.GameEngines.Unity.Memory;

public partial class UnityMemory
{
    public HashSet<T> ReadSet<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadSet<T>(MainModule, baseOffset, offsets);
    }

    public HashSet<T> ReadSet<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadSet<T>(Modules[moduleName], baseOffset, offsets);
    }

    public HashSet<T> ReadSet<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadSet<T>(module.Base + baseOffset, offsets);
    }

    public HashSet<T> ReadSet<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public bool TryReadSet<T>([NotNullWhen(true)] out HashSet<T>? result, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return TryReadSet(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadSet<T>([NotNullWhen(true)] out HashSet<T>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

    public bool TryReadSet<T>([NotNullWhen(true)] out HashSet<T>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadSet(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadSet<T>([NotNullWhen(true)] out HashSet<T>? result, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        throw new NotImplementedException();
    }

    public HashSet<string> ReadSet(int baseOffset, params int[] offsets)
    {
        return ReadSet(MainModule, baseOffset, offsets);
    }

    public HashSet<string> ReadSet(string moduleName, int baseOffset, params int[] offsets)
    {
        return ReadSet(Modules[moduleName], baseOffset, offsets);
    }

    public HashSet<string> ReadSet(Module module, int baseOffset, params int[] offsets)
    {
        return ReadSet(module.Base + baseOffset, offsets);
    }

    public HashSet<string> ReadSet(nint baseAddress, params int[] offsets)
    {
        throw new NotImplementedException();
    }

    public bool TryReadSet([NotNullWhen(true)] out HashSet<string>? result, int baseOffset, params int[] offsets)
    {
        return TryReadSet(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadSet([NotNullWhen(true)] out HashSet<string>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

    public bool TryReadSet([NotNullWhen(true)] out HashSet<string>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadSet(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadSet([NotNullWhen(true)] out HashSet<string>? result, nint baseAddress, params int[] offsets)
    {
        throw new NotImplementedException();
    }
}
