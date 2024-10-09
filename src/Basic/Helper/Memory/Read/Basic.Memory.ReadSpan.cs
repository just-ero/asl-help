using AslHelp.MemUtils;

using System.Runtime.InteropServices;

using static AslHelp.MemUtils.WinAPI;

public partial class Basic
{
    #region ReadArray<T>
    public T[] ReadArray<T>(int length, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return ReadArray<T>(length, MainModule, baseOffset, offsets);
    }

    public T[] ReadArray<T>(int length, string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return ReadArray<T>(length, Modules[module], baseOffset, offsets);
    }

    public T[] ReadArray<T>(int length, Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            Debug.Warn($"[ReadArray<{typeof(T).Name}>] Module could not be found.");

            return Array.Empty<T>();
        }

        return ReadArray<T>(length, module.Base + baseOffset, offsets);
    }

    public T[] ReadArray<T>(int length, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        T[] buffer = new T[length];

        if (!TryReadArray_Internal<T>(buffer, baseAddress, offsets))
        {
            return Array.Empty<T>();
        }

        return buffer;
    }
    #endregion

    #region TryReadArray<T> Out
    public bool TryReadArray<T>(out T[] result, int length, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadArray<T>(out result, length, MainModule, baseOffset, offsets);
    }

    public bool TryReadArray<T>(out T[] result, int length, string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadArray<T>(out result, length, Modules[module], baseOffset, offsets);
    }

    public bool TryReadArray<T>(out T[] result, int length, Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        result = new T[length];
        return TryReadArray<T>(result, module, baseOffset, offsets);
    }

    public unsafe bool TryReadArray<T>(out T[] result, int length, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        result = new T[length];
        return TryReadArray<T>(result, baseAddress, offsets);
    }
    #endregion

    #region TryReadArray<T> Buffer
    public bool TryReadArray<T>(T[] buffer, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadArray<T>(buffer, MainModule, baseOffset, offsets);
    }

    public bool TryReadArray<T>(T[] buffer, string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadArray<T>(buffer, Modules[module], baseOffset, offsets);
    }

    public bool TryReadArray<T>(T[] buffer, Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            Debug.Warn($"[ReadArray<{typeof(T).Name}>] Module could not be found.");

            return false;
        }

        return TryReadArray<T>(buffer, module.Base + baseOffset, offsets);
    }

    public unsafe bool TryReadArray<T>(T[] buffer, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        return TryReadArray_Internal<T>(buffer, baseAddress, offsets);
    }
    #endregion

    internal unsafe bool TryReadArray_Internal<T>(Span<T> buffer, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            return false;
        }

        if (!Is64Bit && IsPointerType<T>())
        {
            Span<uint> buf32 = MemoryMarshal.Cast<T, uint>(buffer);
            Span<ulong> buf64 = MemoryMarshal.Cast<T, ulong>(buffer);

            if (!TryReadArray_Internal<uint>(buf32[buf64.Length..], deref))
            {
                return false;
            }

            for (int i = 0; i < buf64.Length; i++)
            {
                buf64[i] = buf32[buf64.Length + i];
            }

            return true;
        }

        fixed (T* pBuffer = buffer)
        {
            return Game.Read(deref, pBuffer, GetTypeSize<T>(Is64Bit) * buffer.Length);
        }
    }
}
