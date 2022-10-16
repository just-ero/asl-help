using LiveSplit.ComponentUtil;

public partial class Unity
{
    #region ReadString
    public string ReadString(int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, MainModule, baseOffset, offsets);
        return result;
    }

    public string ReadString(string module, int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, Modules[module], baseOffset, offsets);
        return result;
    }

    public string ReadString(Module module, int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, module, baseOffset, offsets);
        return result;
    }

    public string ReadString(nint baseAddress, params int[] offsets)
    {
        TryReadString(out string result, baseAddress, offsets);
        return result;
    }
    #endregion

    #region ReadString (bool derefAddress)
    public string ReadString(bool derefAddress, int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, derefAddress, MainModule, baseOffset, offsets);
        return result;
    }

    public string ReadString(bool derefAddress, string module, int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, derefAddress, Modules[module], baseOffset, offsets);
        return result;
    }

    public string ReadString(bool derefAddress, Module module, int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, derefAddress, module, baseOffset, offsets);
        return result;
    }

    public string ReadString(bool derefAddress, nint baseAddress, params int[] offsets)
    {
        TryReadString(out string result, derefAddress, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadString
    public bool TryReadString(out string result, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadString(out string result, string module, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, Modules[module], baseOffset, offsets);
    }

    public bool TryReadString(out string result, Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            Debug.Warn("[Unity.ReadString] Module could not be found.");

            result = default;
            return false;
        }

        return TryReadString(out result, module.Base + baseOffset, offsets);
    }

    public bool TryReadString(out string result, nint baseAddress, params int[] offsets)
    {
        return TryReadString(out result, true, baseAddress, offsets);
    }
    #endregion

    #region TryReadString (bool derefAddress)
    public bool TryReadString(out string result, bool derefAddress, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, derefAddress, MainModule, baseOffset, offsets);
    }

    public bool TryReadString(out string result, bool derefAddress, string module, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, derefAddress, Modules[module], baseOffset, offsets);
    }

    public bool TryReadString(out string result, bool derefAddress, Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            Debug.Warn("[Unity.ReadString] Module could not be found.");

            result = default;
            return false;
        }

        return TryReadString(out result, derefAddress, module.Base + baseOffset, offsets);
    }

    public bool TryReadString(out string result, bool derefAddress, nint baseAddress, params int[] offsets)
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            result = null;
            return false;
        }

        if (derefAddress && !TryRead<nint>(out deref, deref))
        {
            result = null;
            return false;
        }

        if (!TryRead<int>(out int length, deref + (PtrSize * 2)))
        {
            result = null;
            return false;
        }

        if (!Game.ReadString(deref + (PtrSize * 2) + 0x4, ReadStringType.UTF16, length * 2, out result))
        {
            result = null;
            return false;
        }

        return true;
    }
    #endregion
}
