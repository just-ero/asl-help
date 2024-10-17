using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Memory.Utils;

namespace AslHelp.GameEngines.Unity.Memory;

public partial class UnityMemory
{
    public List<T> ReadList<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadList<T>(MainModule, baseOffset, offsets);
    }

    public List<T> ReadList<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadList<T>(Modules[moduleName], baseOffset, offsets);
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
        return TryReadList(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadList<T>([NotNullWhen(true)] out List<T>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (moduleName is null)
        {
            result = default;
            return false;
        }

        if (!Modules.TryGetValue(moduleName, out var module))
        {
            result = default;
            return false;
        }

        return TryReadList(out result, module, baseOffset, offsets);
    }

    public bool TryReadList<T>([NotNullWhen(true)] out List<T>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
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

        if (!TryRead(out int count, deref + (PointerSize * 3))
            || count < 0)
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
        return ReadList(MainModule, baseOffset, offsets);
    }

    public List<string> ReadList(string moduleName, int baseOffset, params int[] offsets)
    {
        return ReadList(Modules[moduleName], baseOffset, offsets);
    }

    public List<string> ReadList(Module module, int baseOffset, params int[] offsets)
    {
        return ReadList(module.Base + baseOffset, offsets);
    }

    public List<string> ReadList(nint baseAddress, params int[] offsets)
    {
        nint deref = Read<nint>(baseAddress, offsets);
        int count = Read<int>(deref + (PointerSize * 3));

        List<string> result = new(count);
        for (int i = 0; i < count; i++)
        {
            string value = ReadString(deref + (PointerSize * 2), (PointerSize * 4) + (PointerSize * i));
            result.Add(value);
        }

        return result;
    }

    public bool TryReadList([NotNullWhen(true)] out List<string>? result, int baseOffset, params int[] offsets)
    {
        return TryReadList(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadList([NotNullWhen(true)] out List<string>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
    {
        if (moduleName is null)
        {
            result = default;
            return false;
        }

        if (!Modules.TryGetValue(moduleName, out var module))
        {
            result = default;
            return false;
        }

        return TryReadList(out result, module, baseOffset, offsets);
    }

    public bool TryReadList([NotNullWhen(true)] out List<string>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
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
        if (!TryRead(out nint deref, baseAddress, offsets))
        {
            result = default;
            return false;
        }

        if (!TryRead(out int count, deref + (PointerSize * 3))
            || count < 0)
        {
            result = default;
            return false;
        }

        result = new(count);

        for (int i = 0; i < count; i++)
        {
            if (!TryReadString(out string? value, deref + (PointerSize * 2), (PointerSize * 4) + (PointerSize * i)))
            {
                result = default;
                return false;
            }

            result.Add(value);
        }

        return true;
    }
}
