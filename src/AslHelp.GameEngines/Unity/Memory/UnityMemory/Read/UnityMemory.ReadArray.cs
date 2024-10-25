using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;

namespace AslHelp.GameEngines.Unity.Memory;

public partial class UnityMemory
{
    private int ArrayLength => PointerSize * 3;
    private int ArrayData => PointerSize * 4;

    public T[]? ReadArray<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadArray<T>(MainModule, baseOffset, offsets);
    }

    public T[]? ReadArray<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadArray<T>(Modules[moduleName], baseOffset, offsets);
    }

    public T[]? ReadArray<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadArray<T>(module.Base + baseOffset, offsets);
    }

    public T[]? ReadArray<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        nint deref = Read<nint>(baseAddress, offsets);
        if (deref == 0)
        {
            return null;
        }

        int length = Read<int>(deref + ArrayLength);

        return ReadArray<T>(length, deref + ArrayData);
    }

    public bool TryReadArray<T>(out T[]? result, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return TryReadArray(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadArray<T>(out T[]? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

        return TryReadArray(out result, module, baseOffset, offsets);
    }

    public bool TryReadArray<T>(out T[]? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadArray(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadArray<T>(out T[]? result, nint baseAddress, params int[] offsets)
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

        if (!TryRead(out int length, deref + ArrayLength))
        {
            result = default;
            return false;
        }

        return TryReadArray(out result, length, deref + ArrayData);
    }

    public string?[]? ReadArray(int baseOffset, params int[] offsets)
    {
        return ReadArray(MainModule, baseOffset, offsets);
    }

    public string?[]? ReadArray(string moduleName, int baseOffset, params int[] offsets)
    {
        return ReadArray(Modules[moduleName], baseOffset, offsets);
    }

    public string?[]? ReadArray(Module module, int baseOffset, params int[] offsets)
    {
        return ReadArray(module.Base + baseOffset, offsets);
    }

    public string?[]? ReadArray(nint baseAddress, params int[] offsets)
    {
        nint deref = Read<nint>(baseAddress, offsets);
        if (deref == 0)
        {
            return null;
        }

        int length = Read<int>(deref + ArrayLength);

        string?[] results = new string?[length];
        for (int i = 0; i < length; i++)
        {
            results[i] = ReadString(deref + ArrayData + (PointerSize * i));
        }

        return results;
    }

    public bool TryReadArray(out string?[]? result, int baseOffset, params int[] offsets)
    {
        return TryReadArray(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadArray(out string?[]? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

        return TryReadArray(out result, module, baseOffset, offsets);
    }

    public bool TryReadArray(out string?[]? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadArray(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadArray(out string?[]? result, nint baseAddress, params int[] offsets)
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

        if (!TryRead(out int length, deref + ArrayLength))
        {
            result = default;
            return false;
        }

        string?[] results = new string?[length];

        for (int i = 0; i < length; i++)
        {
            results[i] = TryReadString(out string? value, deref + ArrayData + (PointerSize * i))
                ? value
                : null;
        }

        result = results;
        return true;
    }
}
