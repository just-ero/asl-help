namespace ASLHelper;

public partial class Main
{
    #region Read<T>
    public T Read<T>(int baseOffset, params int[] offsets) where T : unmanaged
    {
        _ = TryRead<T>(out var result, Game?.MainModuleWow64Safe(), baseOffset, offsets);
        return result;
    }

    public T Read<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
    {
        _ = TryRead<T>(out var result, GetModule(moduleName), baseOffset, offsets);
        return result;
    }

    public T Read<T>(ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        _ = TryRead<T>(out var result, module, baseOffset, offsets);
        return result;
    }

    public T Read<T>(IntPtr baseAddress, params int[] offsets) where T : unmanaged
    {
        _ = TryRead<T>(out var result, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryRead<T>
    public bool TryRead<T>(out T result, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryRead<T>(out result, Game?.MainModuleWow64Safe(), baseOffset, offsets);
    }

    public bool TryRead<T>(out T result, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryRead<T>(out result, GetModule(moduleName), baseOffset, offsets);
    }

    public bool TryRead<T>(out T result, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module == null)
        {
            Debug.Warn("[Read] Module could not be found!");

            result = default;
            return false;
        }

        return TryRead<T>(out result, module.BaseAddress + baseOffset, offsets);
    }

    public unsafe bool TryRead<T>(out T result, IntPtr baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryDeref(out var deref, baseAddress, offsets))
        {
            result = default;
            return false;
        }

        fixed (T* pResult = &result)
        {
            return Read(&pResult, GetTypeSize<T>(), deref);
        }
    }
    #endregion
}
