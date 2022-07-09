namespace ASLHelper;

public partial class Main
{
    #region ReadSpan<T>
    public IList<T> ReadSpan<T>(int length, int baseOffset, params int[] offsets) where T : unmanaged
    {
        IList<T> buffer = new T[length];
        _ = TryReadSpan<T>(buffer, Game?.MainModuleWow64Safe(), baseOffset, offsets);
        return buffer;
    }

    public IList<T> ReadSpan<T>(int length, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
    {
        IList<T> buffer = new T[length];
        _ = TryReadSpan<T>(buffer, GetModule(moduleName), baseOffset, offsets);
        return buffer;
    }

    public IList<T> ReadSpan<T>(int length, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        IList<T> buffer = new T[length];
        _ = TryReadSpan<T>(buffer, module, baseOffset, offsets);
        return buffer;
    }

    public IList<T> ReadSpan<T>(int length, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        IList<T> buffer = new T[length];
        _ = TryReadSpan<T>(buffer, baseAddress, offsets);
        return buffer;
    }
    #endregion

    #region TryReadSpan<T>
    public bool TryReadSpan<T>(IList<T> buffer, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadSpan<T>(buffer, Game?.MainModuleWow64Safe(), baseOffset, offsets);
    }

    public bool TryReadSpan<T>(IList<T> buffer, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadSpan<T>(buffer, GetModule(moduleName), baseOffset, offsets);
    }

    public bool TryReadSpan<T>(IList<T> buffer, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            Debug.Warn("[Read] Module could not be found!");

            _ = Array.Empty<T>();
            return false;
        }

        return TryReadSpan<T>(buffer, module.BaseAddress + baseOffset, offsets);
    }

    public unsafe bool TryReadSpan<T>(IList<T> buffer, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        var deref = Deref(baseAddress, offsets);
        if (deref == 0)
            return false;

        if (!Is64Bit && IsPointerType<T>())
        {
            var buf32 = new uint[buffer.Count];
            if (!TryReadSpan<uint>(buf32, deref))
                return false;

            for (int i = 0; i < buf32.Length; i++)
            {
                fixed (uint* pBuf = &buf32[i])
                {
                    buffer[i] = *(T*)pBuf;
                }
            }

            return true;
        }

        if (buffer is not T[] arr)
        {
            arr = buffer.ToArray();
        }

        fixed (T* pBuffer = arr)
        {
            return Read(pBuffer, GetTypeSize<T>() * arr.Length, deref);
        }
    }
    #endregion
}
