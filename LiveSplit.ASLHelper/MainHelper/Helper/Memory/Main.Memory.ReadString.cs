namespace ASLHelper;

public partial class Main
{
    #region ReadString
    public string ReadString(int length, ReadStringType stringType, int baseOffset, params int[] offsets)
    {
        _ = TryReadString(out var result, length, stringType, Game?.MainModuleWow64Safe(), baseOffset, offsets);
        return result;
    }

    public string ReadString(int length, ReadStringType stringType, string moduleName, int baseOffset, params int[] offsets)
    {
        _ = TryReadString(out var result, length, stringType, GetModule(moduleName), baseOffset, offsets);
        return result;
    }

    public string ReadString(int length, ReadStringType stringType, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets)
    {
        _ = TryReadString(out var result, length, stringType, module, baseOffset, offsets);
        return result;
    }

    public string ReadString(int length, ReadStringType stringType, nint baseAddress, params int[] offsets)
    {
        _ = TryReadString(out var result, length, stringType, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadString
    public bool TryReadString(out string result, int length, ReadStringType stringType, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, length, stringType, Game?.MainModuleWow64Safe(), baseOffset, offsets);
    }

    public bool TryReadString(out string result, int length, ReadStringType stringType, string moduleName, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, length, stringType, GetModule(moduleName), baseOffset, offsets);
    }

    public bool TryReadString(out string result, int length, ReadStringType stringType, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            Debug.Warn("[Read] Module could not be found!");

            result = default;
            return false;
        }

        return TryReadString(out result, length, stringType, module.BaseAddress + baseOffset, offsets);
    }

    public bool TryReadString(out string result, int length, ReadStringType stringType, nint baseAddress, params int[] offsets)
    {
        if (!TryDeref(out var deref, baseAddress, offsets))
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
