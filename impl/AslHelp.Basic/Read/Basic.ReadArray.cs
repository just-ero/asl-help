using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using AslHelp.Memory;
using AslHelp.Memory.Native;
using AslHelp.Shared;

public partial class Basic
{
    public T[] ReadArray<T>(int length, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return ReadArray<T>(length, MainModule, baseOffset, offsets);
    }

    public T[] ReadArray<T>(int length, string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(Modules);

        return ReadArray<T>(length, Modules[moduleName], baseOffset, offsets);
    }

    public T[] ReadArray<T>(int length, Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadArray<T>(length, module.Base + baseOffset, offsets);
    }

    public T[] ReadArray<T>(int length, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        var results = new T[length];
        ReadArray<T>(results, baseAddress, offsets);

        return results;
    }

    public bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, int length, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadArray(out result, length, MainModule, baseOffset, offsets);
    }

    public bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, int length, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(Modules);

        if (moduleName is null)
        {
            result = null;
            return false;
        }

        return TryReadArray(out result, length, Modules[moduleName], baseOffset, offsets);
    }

    public bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, int length, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (module is null)
        {
            result = null;
            return false;
        }

        return TryReadArray(out result, length, module.Base + baseOffset, offsets);
    }

    public bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, int length, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        result = new T[length];
        return TryReadArray<T>(result, baseAddress, offsets);
    }

    public void ReadArray<T>(Span<T> buffer, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        ReadArray(buffer, MainModule, baseOffset, offsets);
    }

    public void ReadArray<T>(Span<T> buffer, string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(Modules);

        ReadArray(buffer, Modules[moduleName], baseOffset, offsets);
    }

    public void ReadArray<T>(Span<T> buffer, Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ReadArray(buffer, module.Base + baseOffset, offsets);
    }

    public unsafe void ReadArray<T>(Span<T> buffer, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        if (buffer.Length == 0)
        {
            return;
        }

        if (!Is64Bit && IsNativeInt<T>())
        {
            // Need to read 32-bit integers for the pointers and then copy.

            Span<uint> buf32 = MemoryMarshal.Cast<T, uint>(buffer);
            Span<ulong> buf64 = MemoryMarshal.Cast<T, ulong>(buffer);

            ReadArray(buf32[buf64.Length..], baseAddress, offsets);

            for (int i = 0; i < buf64.Length; i++)
            {
                buf64[i] = buf32[buf64.Length + i];
            }

            return;
        }

        nint deref = Deref(baseAddress, offsets);
        int size = GetNativeSizeOf<T>() * buffer.Length;

        fixed (T* pBuffer = buffer)
        {
            if (!WinInteropWrapper.ReadMemory(_handle, deref, pBuffer, size))
            {
                string msg = $"Failed to read memory at {(ulong)deref:X}: {WinInteropWrapper.GetLastWin32ErrorMessage()}";
                ThrowHelper.ThrowException(msg);
            }
        }
    }

    public bool TryReadArray<T>(Span<T> buffer, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return TryReadArray(buffer, MainModule, baseOffset, offsets);
    }

    public bool TryReadArray<T>(Span<T> buffer, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfNull(Modules);

        if (moduleName is null)
        {
            return false;
        }

        return TryReadArray(buffer, Modules[moduleName], baseOffset, offsets);
    }

    public bool TryReadArray<T>(Span<T> buffer, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (module is null)
        {
            return false;
        }

        return TryReadArray(buffer, module.Base + baseOffset, offsets);
    }

    public unsafe bool TryReadArray<T>(Span<T> buffer, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        if (buffer.Length == 0)
        {
            return true;
        }

        if (!Is64Bit && IsNativeInt<T>())
        {
            // Need to read 32-bit integers for the pointers and then copy.

            Span<uint> buf32 = MemoryMarshal.Cast<T, uint>(buffer);
            Span<ulong> buf64 = MemoryMarshal.Cast<T, ulong>(buffer);

            if (!TryReadArray(buf32[buf64.Length..], baseAddress, offsets))
            {
                return false;
            }

            for (int i = 0; i < buf64.Length; i++)
            {
                buf64[i] = buf32[buf64.Length + i];
            }

            return true;
        }

        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            return false;
        }

        int size = GetNativeSizeOf<T>() * buffer.Length;

        fixed (T* pBuffer = buffer)
        {
            return WinInteropWrapper.ReadMemory(_handle, deref, pBuffer, size);
        }
    }

    private unsafe bool TryReadArray_Internal<T>(Span<T> buffer, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            return false;
        }

        if (!Is64Bit && IsNativeInt<T>())
        {
            Span<uint> buf32 = MemoryMarshal.Cast<T, uint>(buffer);
            Span<ulong> buf64 = MemoryMarshal.Cast<T, ulong>(buffer);

            if (!TryReadArray_Internal(buf32[buf64.Length..], deref))
            {
                return false;
            }

            for (int i = 0; i < buf64.Length; i++)
            {
                buf64[i] = buf32[buf64.Length + i];
            }

            return true;
        }

        int size = GetNativeSizeOf<T>() * buffer.Length;

        fixed (T* pBuffer = buffer)
        {
            return WinInteropWrapper.ReadMemory(_handle, deref, pBuffer, size);
        }
    }
}
