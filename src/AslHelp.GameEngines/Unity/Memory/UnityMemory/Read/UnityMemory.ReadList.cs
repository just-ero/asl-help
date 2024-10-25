using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Memory.Utils;

namespace AslHelp.GameEngines.Unity.Memory;

public partial class UnityMemory
{
    private int ListItems => PointerSize * 2;
    private int ListSize => PointerSize * 3;

    public IReadOnlyList<T>? ReadList<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadList<T>(MainModule, baseOffset, offsets);
    }

    public IReadOnlyList<T>? ReadList<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadList<T>(Modules[moduleName], baseOffset, offsets);
    }

    public IReadOnlyList<T>? ReadList<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadList<T>(module.Base + baseOffset, offsets);
    }

    public IReadOnlyList<T>? ReadList<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        nint deref = Read<nint>(baseAddress, offsets);
        if (deref == 0)
        {
            return null;
        }

        int count = Read<int>(deref + ListSize);

        List<T> result = new(count);
        ReadArray(CollectionsMarshal<T>.AsSpan(result), deref + ListItems, ArrayData);

        return result;
    }

    public bool TryReadList<T>(out IReadOnlyList<T>? result, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return TryReadList(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadList<T>(out IReadOnlyList<T>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

    public bool TryReadList<T>(out IReadOnlyList<T>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadList(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadList<T>(out IReadOnlyList<T>? result, nint baseAddress, params int[] offsets)
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

        if (!TryRead(out int count, deref + ListSize)
            || count < 0)
        {
            result = default;
            return false;
        }

        List<T> values = new(count);

        if (!TryReadArray(CollectionsMarshal<T>.AsSpan(values), deref + ListItems, ArrayData))
        {
            result = default;
            return false;
        }

        result = values;
        return true;
    }

    public IReadOnlyList<string?>? ReadList(int baseOffset, params int[] offsets)
    {
        return ReadList(MainModule, baseOffset, offsets);
    }

    public IReadOnlyList<string?>? ReadList(string moduleName, int baseOffset, params int[] offsets)
    {
        return ReadList(Modules[moduleName], baseOffset, offsets);
    }

    public IReadOnlyList<string?>? ReadList(Module module, int baseOffset, params int[] offsets)
    {
        return ReadList(module.Base + baseOffset, offsets);
    }

    public IReadOnlyList<string?>? ReadList(nint baseAddress, params int[] offsets)
    {
        nint deref = Read<nint>(baseAddress, offsets);
        if (deref == 0)
        {
            return null;
        }

        int count = Read<int>(deref + ListSize);

        List<string?> result = new(count);
        for (int i = 0; i < count; i++)
        {
            string? value = ReadString(deref + ListItems, ArrayData + (PointerSize * i));
            result.Add(value);
        }

        return result;
    }

    public bool TryReadList(out IReadOnlyList<string?>? result, int baseOffset, params int[] offsets)
    {
        return TryReadList(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadList(out IReadOnlyList<string?>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

    public bool TryReadList(out IReadOnlyList<string?>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadList(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadList(out IReadOnlyList<string?>? result, nint baseAddress, params int[] offsets)
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

        if (!TryRead(out int count, deref + ListSize)
            || count < 0)
        {
            result = default;
            return false;
        }

        List<string?> values = new(count);

        for (int i = 0; i < count; i++)
        {
            values.Add(TryReadString(out string? value, deref + ListItems, ArrayData + (PointerSize * i))
                ? value
                : null);
        }

        result = values;
        return true;
    }
}
