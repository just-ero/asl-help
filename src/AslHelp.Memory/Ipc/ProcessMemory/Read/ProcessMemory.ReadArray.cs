using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using AslHelp.Memory.Native;
using AslHelp.Shared;

namespace AslHelp.Memory.Ipc;

public partial class ProcessMemory
{
    public T[] ReadArray<T>(int length, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return ReadArray<T>(length, MainModule, baseOffset, offsets);
    }

    public T[] ReadArray<T>(int length, string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
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
        ReadArray(results, baseAddress, offsets);

        return results;
    }

    public bool TryReadArray<T>([NotNullWhen(true)] out T[]? results, int length, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return TryReadArray(out results, length, MainModule, baseOffset, offsets);
    }

    public bool TryReadArray<T>([NotNullWhen(true)] out T[]? results, int length, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (moduleName is null)
        {
            results = default;
            return false;
        }

        if (!Modules.TryGetValue(moduleName, out Module? module))
        {
            results = default;
            return false;
        }

        return TryReadArray(out results, length, module, baseOffset, offsets);
    }

    public bool TryReadArray<T>([NotNullWhen(true)] out T[]? results, int length, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (module is null)
        {
            results = default;
            return false;
        }

        return TryReadArray(out results, length, module.Base + baseOffset, offsets);
    }

    public bool TryReadArray<T>([NotNullWhen(true)] out T[]? results, int length, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        results = new T[length];
        return TryReadArray(results, baseAddress, offsets);
    }

    public void ReadArray<T>(Span<T> buffer, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        ReadArray(buffer, MainModule, baseOffset, offsets);
    }

    public void ReadArray<T>(Span<T> buffer, string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
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
        return TryReadArray(buffer, MainModule, baseOffset, offsets);
    }

    public bool TryReadArray<T>(Span<T> buffer, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        if (moduleName is null)
        {
            return false;
        }

        if (!Modules.TryGetValue(moduleName, out Module? module))
        {
            return false;
        }

        return TryReadArray(buffer, module, baseOffset, offsets);
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
}
