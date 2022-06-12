namespace ASLHelper;

public partial class Unity
{
    #region ReadList<T>
    public List<T> ReadList<T>(int baseOffset, params int[] offsets) where T : unmanaged
    {
        _ = TryReadList<T>(out var result, Game?.MainModuleWow64Safe(), baseOffset, offsets);
        return result;
    }

    public List<T> ReadList<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
    {
        _ = TryReadList<T>(out var result, GetModule(moduleName), baseOffset, offsets);
        return result;
    }

    public List<T> ReadList<T>(ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        _ = TryReadList<T>(out var result, module, baseOffset, offsets);
        return result;
    }

    public List<T> ReadList<T>(nint baseAddress, params int[] offsets) where T : unmanaged
    {
        _ = TryReadList<T>(out var result, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadList<T>
    public bool TryReadList<T>(out List<T> result, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadList<T>(out result, Game?.MainModuleWow64Safe(), baseOffset, offsets);
    }

    public bool TryReadList<T>(out List<T> result, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadList<T>(out result, GetModule(moduleName), baseOffset, offsets);
    }

    public bool TryReadList<T>(out List<T> result, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module == null)
        {
            Debug.Warn("[Read] Module could not be found!");

            result = default;
            return false;
        }

        return TryReadList<T>(out result, module.BaseAddress + baseOffset, offsets);
    }

    public bool TryReadList<T>(out List<T> result, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryRead<nint>(out var deref, baseAddress, offsets))
        {
            result = new();
            return false;
        }

        if (!TryRead<int>(out var count, deref + (Is64Bit ? 0x18 : 0xC)))
        {
            result = new();
            return false;
        }

        if (!TryRead<nint>(out var items, deref + (Is64Bit ? 0x10 : 0x8)))
        {
            result = new();
            return false;
        }

        var buf = new T[count];
        if (!TryReadSpan<T>(buf, items + (PtrSize * 4)))
        {
            result = new();
            return false;
        }

        result = buf.ToList();
        return true;
    }
    #endregion
}
