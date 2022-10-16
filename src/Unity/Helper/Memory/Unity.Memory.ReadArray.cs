public partial class Unity
{
    #region ReadArray<T>
    public T[] ReadArray<T>(int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadArray<T>(out T[] result, MainModule, baseOffset, offsets);
        return result;
    }

    public T[] ReadArray<T>(string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadArray<T>(out T[] result, Modules[module], baseOffset, offsets);
        return result;
    }

    public T[] ReadArray<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadArray<T>(out T[] result, module, baseOffset, offsets);
        return result;
    }

    public T[] ReadArray<T>(nint baseAddress, params int[] offsets) where T : unmanaged
    {
        TryReadArray<T>(out T[] result, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadArray<T>
    public bool TryReadArray<T>(out T[] result, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadArray<T>(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadArray<T>(out T[] result, string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadArray<T>(out result, Modules[module], baseOffset, offsets);
    }

    public bool TryReadArray<T>(out T[] result, Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            Debug.Warn($"[Unity.ReadArray<{typeof(T).Name}>] Module could not be found.");

            result = default;
            return false;
        }

        return TryReadArray<T>(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadArray<T>(out T[] result, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryRead<nint>(out nint deref, baseAddress, offsets))
        {
            result = Array.Empty<T>();
            return false;
        }

        if (!TryRead<int>(out int length, deref + (PtrSize * 3)))
        {
            result = Array.Empty<T>();
            return false;
        }

        result = new T[length];

        return TryReadSpan_Internal<T>(result, deref + (PtrSize * 4));
    }
    #endregion
}
