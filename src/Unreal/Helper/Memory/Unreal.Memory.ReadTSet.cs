using static AslHelp.MemUtils.WinAPI;

public partial class Unreal
{
    #region ReadTSet<T>
    public T[] ReadTSet<T>(int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadTSet<T>(out T[] result, MainModule, baseOffset, offsets);
        return result;
    }

    public T[] ReadTSet<T>(string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadTSet<T>(out T[] result, Modules[module], baseOffset, offsets);
        return result;
    }

    public T[] ReadTSet<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        TryReadTSet<T>(out T[] result, module, baseOffset, offsets);
        return result;
    }

    public T[] ReadTSet<T>(nint baseAddress, params int[] offsets) where T : unmanaged
    {
        TryReadTSet<T>(out T[] result, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadTSet<T>
    public bool TryReadTSet<T>(out T[] result, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadTSet<T>(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadTSet<T>(out T[] result, string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return TryReadTSet<T>(out result, Modules[module], baseOffset, offsets);
    }

    public bool TryReadTSet<T>(out T[] result, Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            Debug.Warn($"[Unreal.ReadTArray<{typeof(T).Name}>] Module could not be found.");

            result = default;
            return false;
        }

        return TryReadTSet<T>(out result, module.Base + baseOffset, offsets);
    }

    public unsafe bool TryReadTSet<T>(out T[] result, nint baseAddress, params int[] offsets) where T : unmanaged
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

        TSetElement<T>* buffer = stackalloc TSetElement<T>[arrayNum];

        if (!Game.Read(allocator, buffer, GetTypeSize<TSetElement<T>>(Is64Bit)))
        {
            result = Array.Empty<T>();
            return false;
        }

        result = new T[arrayNum];

        for (int i = 0; i < arrayNum; i++)
        {
            result[i] = buffer[i].Element;
        }

        return true;
    }
    #endregion

#pragma warning disable CS0169, CS0649, IDE0044, IDE1006
    internal struct TSetElement<T> where T : unmanaged
    {
        public T Element;

        private int Hash;
        private int HashSize;
    }
#pragma warning restore CS0169, CS0649, IDE0044, IDE1006
}
