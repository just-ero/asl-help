using static AslHelp.MemUtils.WinAPI;

public partial class Unreal
{
    #region ReadTMap<TKey, TValue>
    public Dictionary<TKey, TValue> ReadTMap<TKey, TValue>(int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        TryReadTMap<TKey, TValue>(out Dictionary<TKey, TValue> result, MainModule, baseOffset, offsets);
        return result;
    }

    public Dictionary<TKey, TValue> ReadTMap<TKey, TValue>(string module, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        TryReadTMap<TKey, TValue>(out Dictionary<TKey, TValue> result, Modules[module], baseOffset, offsets);
        return result;
    }

    public Dictionary<TKey, TValue> ReadTMap<TKey, TValue>(Module module, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        TryReadTMap<TKey, TValue>(out Dictionary<TKey, TValue> result, module, baseOffset, offsets);
        return result;
    }

    public Dictionary<TKey, TValue> ReadTMap<TKey, TValue>(nint baseAddress, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        TryReadTMap<TKey, TValue>(out Dictionary<TKey, TValue> result, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadTMap<TKey, TValue>
    public bool TryReadTMap<TKey, TValue>(out Dictionary<TKey, TValue> result, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return TryReadTMap<TKey, TValue>(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadTMap<TKey, TValue>(out Dictionary<TKey, TValue> result, string module, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return TryReadTMap<TKey, TValue>(out result, Modules[module], baseOffset, offsets);
    }

    public bool TryReadTMap<TKey, TValue>(out Dictionary<TKey, TValue> result, Module module, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        if (module is null)
        {
            Debug.Warn($"[Unreal.ReadTMap<{typeof(TKey).Name}, {typeof(TValue).Name}>] Module could not be found.");

            result = default;
            return false;
        }

        return TryReadTMap<TKey, TValue>(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadTMap<TKey, TValue>(out Dictionary<TKey, TValue> result, nint baseAddress, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        result = new();
        return TryReadTMap<TKey, TValue>(result, baseAddress, offsets);
    }
    #endregion

    #region TryReadTMap<TKey, TValue> 2
    public bool TryReadTMap<TKey, TValue>(Dictionary<TKey, TValue> result, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return TryReadTMap<TKey, TValue>(result, MainModule, baseOffset, offsets);
    }

    public bool TryReadTMap<TKey, TValue>(Dictionary<TKey, TValue> result, string module, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        return TryReadTMap<TKey, TValue>(result, Modules[module], baseOffset, offsets);
    }

    public bool TryReadTMap<TKey, TValue>(Dictionary<TKey, TValue> result, Module module, int baseOffset, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        if (module is null)
        {
            Debug.Warn($"[Unreal.ReadTMap<{typeof(TKey).Name}, {typeof(TValue).Name}>] Module could not be found.");

            result = default;
            return false;
        }

        return TryReadTMap<TKey, TValue>(result, module.Base + baseOffset, offsets);
    }

    public unsafe bool TryReadTMap<TKey, TValue>(Dictionary<TKey, TValue> result, nint baseAddress, params int[] offsets)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            return false;
        }

        if (!TryRead<int>(out int arrayNum, deref + PtrSize))
        {
            return false;
        }

        if (!TryRead<nint>(out nint allocator, deref))
        {
            return false;
        }

        if (!Is64Bit)
        {
            bool keyIsPtr = IsPointerType<TKey>(), valueIsPtr = IsPointerType<TValue>();

            if (keyIsPtr)
            {
                if (valueIsPtr)
                {
                    Dictionary<uint, uint> temp = new();
                    PopulateDict(temp, allocator, arrayNum);

                    foreach (KeyValuePair<uint, uint> kvp in temp)
                    {
                        result[(TKey)(object)kvp.Key] = (TValue)(object)kvp.Value;
                    }

                    return true;
                }
                else
                {
                    Dictionary<uint, TValue> temp = new();
                    PopulateDict(temp, allocator, arrayNum);

                    foreach (KeyValuePair<uint, TValue> kvp in temp)
                    {
                        result[(TKey)(object)kvp.Key] = kvp.Value;
                    }

                    return true;
                }
            }
            else if (valueIsPtr)
            {
                Dictionary<TKey, uint> temp = new();
                PopulateDict(temp, allocator, arrayNum);

                foreach (KeyValuePair<TKey, uint> kvp in temp)
                {
                    result[kvp.Key] = (TValue)(object)kvp.Value;
                }

                return true;
            }
        }

        PopulateDict(result, allocator, arrayNum);

        return true;
    }

    private unsafe bool PopulateDict<TKey, TValue>(Dictionary<TKey, TValue> temp, nint allocator, int arrayNum)
        where TKey : unmanaged
        where TValue : unmanaged
    {
        TSetElement<TPair<TKey, TValue>>* buffer = stackalloc TSetElement<TPair<TKey, TValue>>[arrayNum];

        if (!Game.Read(allocator, buffer, GetTypeSize<TSetElement<TPair<TKey, TValue>>>(Is64Bit)))
        {
            return false;
        }

        for (int i = 0; i < arrayNum; i++)
        {
            TPair<TKey, TValue> element = buffer[i].Element;

            TKey key = element.Key;
            TValue value = element.Value;

            temp[key] = value;
        }

        return true;
    }
    #endregion

#pragma warning disable CS0649
    internal struct TPair<TKey, TValue>
        where TKey : unmanaged
        where TValue : unmanaged
    {
        public TKey Key;
        public TValue Value;
    }
#pragma warning restore CS0649
}
