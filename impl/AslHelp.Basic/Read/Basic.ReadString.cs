extern alias Ls;

using Ls::LiveSplit.ComponentUtil;

using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

using AslHelp.Memory;
using AslHelp.Shared;
using AslHelp.Shared.Extensions;

public partial class Basic
{
    public string ReadString(int maxLength, ReadStringType stringType, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadString(maxLength, stringType, MainModule.Base + baseOffset, offsets);
    }

    public string ReadString(int maxLength, ReadStringType stringType, string moduleName, int baseOffset, params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadString(maxLength, stringType, Modules[moduleName].Base + baseOffset, offsets);
    }

    public string ReadString(int maxLength, ReadStringType stringType, Module module, int baseOffset, params int[] offsets)
    {
        return ReadString(maxLength, stringType, module.Base + baseOffset, offsets);
    }

    public string ReadString(int maxLength, ReadStringType stringType, nint baseAddress, params int[] offsets)
    {
        ThrowHelper.ThrowIfLessThan(maxLength, 0);

        if (maxLength == 0)
        {
            return "";
        }

        return stringType switch
        {
            ReadStringType.ASCII => ReadAsciiString(maxLength, baseAddress, offsets),
            ReadStringType.UTF8 => ReadUtf8String(maxLength, baseAddress, offsets),
            ReadStringType.UTF16 => ReadUtf16String(maxLength, baseAddress, offsets),
            _ => ReadAutoString(maxLength, baseAddress, offsets)
        };
    }

    public bool TryReadString(
        [NotNullWhen(true)] out string? result,
        int maxLength,
        ReadStringType stringType,
        int baseOffset,
        params int[] offsets)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadString(out result, maxLength, stringType, MainModule.Base + baseOffset, offsets);
    }

    public bool TryReadString(
        [NotNullWhen(true)] out string? result,
        int maxLength,
        ReadStringType stringType,
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

        return TryReadString(out result, maxLength, stringType, module.Base + baseOffset, offsets);
    }

    public bool TryReadString(
        [NotNullWhen(true)] out string? result,
        int maxLength,
        ReadStringType stringType,
        [NotNullWhen(true)] Module? module,
        int baseOffset,
        params int[] offsets)
    {
        if (module is null)
        {
            result = default;
            return false;
        }

        return TryReadString(out result, maxLength, stringType, module.Base + baseOffset, offsets);
    }

    public bool TryReadString(
        [NotNullWhen(true)] out string? result,
        int maxLength,
        ReadStringType stringType,
        nint baseAddress,
        params int[] offsets)
    {
        if (maxLength < 0)
        {
            result = default;
            return false;
        }

        if (maxLength == 0)
        {
            result = "";
            return true;
        }

        result = stringType switch
        {
            ReadStringType.ASCII => ReadAsciiString(maxLength, baseAddress, offsets),
            ReadStringType.UTF8 => ReadUtf8String(maxLength, baseAddress, offsets),
            ReadStringType.UTF16 => ReadUtf16String(maxLength, baseAddress, offsets),
            _ => ReadAutoString(maxLength, baseAddress, offsets)
        };

        return true;
    }

    private unsafe string ReadAsciiString(int maxLength, nint baseAddress, int[] offsets)
    {
        byte[]? rented = null;
        Span<byte> buffer = maxLength <= 1024
            ? stackalloc byte[1024]
            : (rented = ArrayPool<byte>.Shared.Rent(maxLength));

        ReadArray(buffer[..maxLength], baseAddress, offsets);

        string result = GetStringFromByteSpan(buffer[..maxLength], Encoding.ASCII);

        ArrayPool<byte>.Shared.ReturnIfNotNull(rented);

        return result;
    }

    private unsafe string ReadUtf8String(int maxLength, nint baseAddress, int[] offsets)
    {
        byte[]? rented = null;
        Span<byte> buffer = maxLength <= 1024
            ? stackalloc byte[1024]
            : (rented = ArrayPool<byte>.Shared.Rent(maxLength));

        ReadArray(buffer[..maxLength], baseAddress, offsets);

        string result = GetStringFromByteSpan(buffer[..maxLength], Encoding.UTF8);

        ArrayPool<byte>.Shared.ReturnIfNotNull(rented);

        return result;
    }

    private unsafe string ReadUtf16String(int maxLength, nint baseAddress, int[] offsets)
    {
        char[]? rented = null;
        Span<char> buffer = maxLength <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(maxLength));

        ReadArray(buffer[..maxLength], baseAddress, offsets);

        string result = GetStringFromCharSpan(buffer[..maxLength]);

        ArrayPool<char>.Shared.ReturnIfNotNull(rented);

        return result;
    }

    private unsafe string ReadAutoString(int maxLength, nint baseAddress, int[] offsets)
    {
        // Assume unicode for the worst-case scenario and just allocate maxLength * 2.
        byte[]? rented = null;
        Span<byte> buffer = maxLength * 2 <= 1024
            ? stackalloc byte[1024]
            : (rented = ArrayPool<byte>.Shared.Rent(maxLength * 2));

        ReadArray(buffer[..(maxLength * 2)], baseAddress, offsets);

        string result;
        if (maxLength >= 2 && buffer is [> 0, 0, > 0, 0, ..]) // Best assumption we can make.
        {
            Span<char> charBuffer = MemoryMarshal.Cast<byte, char>(buffer);
            result = GetStringFromCharSpan(charBuffer[..maxLength]);
        }
        else
        {
            result = GetStringFromByteSpan(buffer[..maxLength], Encoding.UTF8);
        }

        ArrayPool<byte>.Shared.ReturnIfNotNull(rented);

        return result;
    }

    private static string GetStringFromCharSpan(Span<char> buffer)
    {
        int length = buffer.IndexOf('\0');
        if (length != -1)
        {
            return buffer[..length].ToString();
        }
        else
        {
            return buffer.ToString();
        }
    }

    private static unsafe string GetStringFromByteSpan(Span<byte> buffer, Encoding encoding)
    {
        int length = buffer.IndexOf((byte)'\0');
        if (length != -1)
        {
            fixed (byte* pBuffer = buffer)
            {
                return encoding.GetString(pBuffer, length);
            }
        }
        else
        {
            fixed (byte* pBuffer = buffer)
            {
                return encoding.GetString(pBuffer, buffer.Length);
            }
        }
    }
}
