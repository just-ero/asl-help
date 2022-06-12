namespace ASLHelper;

public partial class Unity
{
    #region ReadArray<T>
    public T[] ReadArray<T>(int baseOffset, params int[] offsets) where T : unmanaged
    {
        _ = TryReadArray<T>(out var result, Game?.MainModuleWow64Safe(), baseOffset, offsets);
        return result;
    }

    public T[] ReadArray<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
    {
        _ = TryReadArray<T>(out var result, GetModule(moduleName), baseOffset, offsets);
        return result;
    }

    public T[] ReadArray<T>(ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        _ = TryReadArray<T>(out var result, module, baseOffset, offsets);
        return result;
    }

    public T[] ReadArray<T>(nint baseAddress, params int[] offsets) where T : unmanaged
    {
        _ = TryReadArray<T>(out var result, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadArray<T>
    public bool TryReadArray<T>(out T[] result, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadArray<T>(out result, Game?.MainModuleWow64Safe(), baseOffset, offsets);
    }

    public bool TryReadArray<T>(out T[] result, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadArray<T>(out result, GetModule(moduleName), baseOffset, offsets);
    }

    public bool TryReadArray<T>(out T[] result, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module == null)
        {
            Debug.Warn("[Read] Module could not be found!");

            result = default;
            return false;
        }

        return TryReadArray<T>(out result, module.BaseAddress + baseOffset, offsets);
    }

    public bool TryReadArray<T>(out T[] result, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryRead<nint>(out var deref, baseAddress, offsets))
        {
            result = Array.Empty<T>();
            return false;
        }

        if (!TryRead<int>(out var length, deref + (PtrSize * 3)))
        {
            result = Array.Empty<T>();
            return false;
        }

        result = new T[length];

        return TryReadSpan<T>(result, deref + (PtrSize * 4));
    }
    #endregion
}
