using LiveSplit.ComponentUtil;

public partial class Basic
{
    #region ReadString
    public string ReadString(int length, ReadStringType stringType, int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, length, stringType, MainModule, baseOffset, offsets);
        return result;
    }

    public string ReadString(int length, ReadStringType stringType, string module, int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, length, stringType, Modules[module], baseOffset, offsets);
        return result;
    }

    public string ReadString(int length, ReadStringType stringType, Module module, int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, length, stringType, module, baseOffset, offsets);
        return result;
    }

    public string ReadString(int length, ReadStringType stringType, nint baseAddress, params int[] offsets)
    {
        TryReadString(out string result, length, stringType, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadString
    public bool TryReadString(out string result, int length, ReadStringType stringType, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, length, stringType, MainModule, baseOffset, offsets);
    }

    public bool TryReadString(out string result, int length, ReadStringType stringType, string module, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, length, stringType, Modules[module], baseOffset, offsets);
    }

    public bool TryReadString(out string result, int length, ReadStringType stringType, Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            Debug.Warn("[ReadString] Module could not be found.");

            result = default;
            return false;
        }

        return TryReadString(out result, length, stringType, module.Base + baseOffset, offsets);
    }

    public bool TryReadString(out string result, int length, ReadStringType stringType, nint baseAddress, params int[] offsets)
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            result = null;
            return false;
        }

        if (!Game.ReadString(deref, stringType, length, out result))
        {
            result = null;
            return false;
        }

        return true;
    }
    #endregion
}
