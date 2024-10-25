using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Shared.Extensions;

namespace AslHelp.GameEngines.Unity.Memory;

public partial class UnityMemory
{
    private int StringLength => PointerSize * 2;
    private int StringFirstChar => (PointerSize * 2) + sizeof(int);

    public string? ReadString(int baseOffset, params int[] offsets)
    {
        return ReadString(MainModule, baseOffset, offsets);
    }

    public string? ReadString(string moduleName, int baseOffset, params int[] offsets)
    {
        return ReadString(Modules[moduleName], baseOffset, offsets);
    }

    public string? ReadString(Module module, int baseOffset, params int[] offsets)
    {
        return ReadString(module.Base + baseOffset, offsets);
    }

    public string? ReadString(nint baseAddress, params int[] offsets)
    {
        nint deref = Read<nint>(baseAddress, offsets);
        if (deref == 0)
        {
            return null;
        }

        int length = Read<int>(deref + StringLength);

        char[]? rented = null;
        Span<char> buffer = length <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(length));

        ReadArray(buffer[..length], deref + StringFirstChar);
        string result = buffer[..length].ToString();

        ArrayPool<char>.Shared.ReturnIfNotNull(rented);
        return result;
    }

    public bool TryReadString(out string? result, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadString(out string? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
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

        return TryReadString(out result, module, baseOffset, offsets);
    }

    public bool TryReadString(out string? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadString(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadString(out string? result, nint baseAddress, params int[] offsets)
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

        if (!TryRead(out int length, deref + StringLength))
        {
            result = default;
            return false;
        }

        char[]? rented = null;
        Span<char> buffer = length <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(length));

        if (!TryReadArray(buffer[..length], deref + StringFirstChar))
        {
            result = default;

            ArrayPool<char>.Shared.ReturnIfNotNull(rented);
            return false;
        }

        result = buffer[..length].ToString();

        ArrayPool<char>.Shared.ReturnIfNotNull(rented);
        return true;
    }
}
