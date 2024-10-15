using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Shared;
using AslHelp.Shared.Extensions;

public partial class Unity
{
    public string ReadString(int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadString(MainModule.Base + baseOffset, offsets);
    }

    public string ReadString(string moduleName, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadString(Modules[moduleName].Base + baseOffset, offsets);
    }

    public string ReadString(Module module, int baseOffset, params int[] offsets)
    {
        return ReadString(module.Base + baseOffset, offsets);
    }

    public string ReadString(nint baseAddress, params int[] offsets)
    {
        nint deref = Read<nint>(baseAddress, offsets);
        int length = Read<int>(deref + (PointerSize * 2));

        char[]? rented = null;
        Span<char> buffer = length <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(length));

        ReadArray(buffer, deref + (PointerSize * 2) + sizeof(int));

        string result = buffer[..length].ToString();
        ArrayPool<char>.Shared.ReturnIfNotNull(rented);

        return result;
    }

    public bool TryReadString([NotNullWhen(true)] out string? result, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadString(out result, MainModule.Base + baseOffset, offsets);
    }

    public bool TryReadString(
        [NotNullWhen(true)] out string? result,
        [NotNullWhen(true)] string? moduleName,
        int baseOffset,
        params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(Modules);

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

        return TryReadString(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadString(
        [NotNullWhen(true)] out string? result,
        [NotNullWhen(true)] Module? module,
        int baseOffset,
        params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadString(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadString([NotNullWhen(true)] out string? result, nint baseAddress, params int[] offsets)
    {
        if (!TryRead(out nint deref, baseAddress, offsets))
        {
            result = default;
            return false;
        }

        if (!TryRead(out int length, deref + (PointerSize * 2)))
        {
            result = default;
            return false;
        }

        char[]? rented = null;
        Span<char> buffer = length <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(length));

        if (!TryReadArray(buffer[..length], deref + (PointerSize * 2) + sizeof(int)))
        {
            ArrayPool<char>.Shared.ReturnIfNotNull(rented);

            result = default;
            return false;
        }

        result = buffer[..length].ToString();
        ArrayPool<char>.Shared.ReturnIfNotNull(rented);

        return true;
    }
}
