public partial class Unreal
{
    #region ReadTArray<T>
    public T[] ReadTArray<T>(int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadTArray<T>(out T[] result, MainModule, baseOffset, offsets);
        return result;
    }

    public T[] ReadTArray<T>(string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadTArray<T>(out T[] result, Modules[module], baseOffset, offsets);
        return result;
    }

    public T[] ReadTArray<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadTArray<T>(out T[] result, module, baseOffset, offsets);
        return result;
    }

    public T[] ReadTArray<T>(nint baseAddress, params int[] offsets) where T : unmanaged
    {
        TryReadTArray<T>(out T[] result, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadTArray<T>
    public bool TryReadTArray<T>(out T[] result, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadTArray<T>(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadTArray<T>(out T[] result, string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadTArray<T>(out result, Modules[module], baseOffset, offsets);
    }

    public bool TryReadTArray<T>(out T[] result, Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            Debug.Warn($"[Unreal.ReadTArray<{typeof(T).Name}>] Module could not be found.");

            result = Array.Empty<T>();
            return false;
        }

        return TryReadTArray<T>(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadTArray<T>(out T[] result, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            result = Array.Empty<T>();
            return false;
        }

        if (!TryRead<int>(out int arrayNum, deref + PtrSize))
        {
            result = Array.Empty<T>();
            return false;
        }

        if (!TryRead<nint>(out nint allocator, deref))
        {
            result = Array.Empty<T>();
            return false;
        }

        result = new T[arrayNum];

        return TryReadSpan<T>(result, allocator);
    }
    #endregion
}
