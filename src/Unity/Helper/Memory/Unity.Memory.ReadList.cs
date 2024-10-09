public partial class Unity
{
    #region ReadList<T>
    public List<T> ReadList<T>(int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadList<T>(out List<T> result, MainModule, baseOffset, offsets);
        return result;
    }

    public List<T> ReadList<T>(string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadList<T>(out List<T> result, Modules[module], baseOffset, offsets);
        return result;
    }

    public List<T> ReadList<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadList<T>(out List<T> result, module, baseOffset, offsets);
        return result;
    }

    public List<T> ReadList<T>(nint baseAddress, params int[] offsets) where T : unmanaged
    {
        TryReadList<T>(out List<T> result, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadList<T>
    public bool TryReadList<T>(out List<T> result, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadList<T>(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadList<T>(out List<T> result, string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadList<T>(out result, Modules[module], baseOffset, offsets);
    }

    public bool TryReadList<T>(out List<T> result, Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            Debug.Warn($"[Unity.ReadList<{typeof(T).Name}>] Module could not be found.");

            result = default;
            return false;
        }

        return TryReadList<T>(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadList<T>(out List<T> result, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryRead<nint>(out nint deref, baseAddress, offsets))
        {
            result = new();
            return false;
        }

        if (!TryRead<int>(out int count, deref + (PtrSize * 3)))
        {
            result = new();
            return false;
        }

        if (!TryRead<nint>(out nint items, deref + (PtrSize * 2)))
        {
            result = new();
            return false;
        }

        T[] buf = new T[count];

        if (!TryReadArray_Internal<T>(buf, items + (PtrSize * 4)))
        {
            result = new();
            return false;
        }

        result = buf.ToList();
        return true;
    }
    #endregion
}
