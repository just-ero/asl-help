namespace ASLHelper;

public partial class Main
{
    #region Write<T>
    public bool Write<T>(T value, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return Write<T>(value, Game?.MainModuleWow64Safe(), baseOffset, offsets);
    }

    public bool Write<T>(T value, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return Write<T>(value, GetModule(moduleName), baseOffset, offsets);
    }

    public bool Write<T>(T value, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            Debug.Warn("[Write] Module could not be found!");

            return false;
        }

        return Write<T>(value, module.BaseAddress + baseOffset, offsets);
    }

    public unsafe bool Write<T>(T value, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryDeref(out var deref, baseAddress, offsets))
        {
            return false;
        }

        return Write(&value, GetTypeSize<T>(), deref);
    }
    #endregion

    #region WriteSpan<T>
    public bool WriteSpan<T>(IList<T> values, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return WriteSpan<T>(values, Game?.MainModuleWow64Safe(), baseOffset, offsets);
    }

    public bool WriteSpan<T>(IList<T> values, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return WriteSpan<T>(values, GetModule(moduleName), baseOffset, offsets);
    }

    public bool WriteSpan<T>(IList<T> values, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            Debug.Warn("[Write] Module could not be found!");

            return false;
        }

        return WriteSpan<T>(values, module.BaseAddress + baseOffset, offsets);
    }

    public unsafe bool WriteSpan<T>(IList<T> values, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryDeref(out var deref, baseAddress, offsets))
        {
            return false;
        }

        if (values is not T[] arr)
        {
            arr = values.ToArray();
        }

        fixed (T* pValues = arr)
        {
            return Write(pValues, GetTypeSize<T>(), deref);
        }
    }
    #endregion
}
