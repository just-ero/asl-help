namespace ASLHelper;

public partial class Unity
{
    #region ReadString
    public string ReadString(int baseOffset, params int[] offsets)
    {
        _ = TryReadString(out var result, Game?.MainModuleWow64Safe(), baseOffset, offsets);
        return result;
    }

    public string ReadString(string moduleName, int baseOffset, params int[] offsets)
    {
        _ = TryReadString(out var result, GetModule(moduleName), baseOffset, offsets);
        return result;
    }

    public string ReadString(ProcessModuleWow64Safe module, int baseOffset, params int[] offsets)
    {
        _ = TryReadString(out var result, module, baseOffset, offsets);
        return result;
    }

    public string ReadString(nint baseAddress, params int[] offsets)
    {
        _ = TryReadString(out var result, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadString
    public bool TryReadString(out string result, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, Game?.MainModuleWow64Safe(), baseOffset, offsets);
    }

    public bool TryReadString(out string result, string moduleName, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, GetModule(moduleName), baseOffset, offsets);
    }

    public bool TryReadString(out string result, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            Debug.Warn("[Read] Module could not be found!");

            result = default;
            return false;
        }

        return TryReadString(out result, module.BaseAddress + baseOffset, offsets);
    }

    public bool TryReadString(out string result, nint baseAddress, params int[] offsets)
    {
        if (!TryDeref(out var deref, baseAddress, offsets))
        {
            result = null;
            return false;
        }

        if (!TryRead<int>(out int length, deref + (Is64Bit ? 0x10 : 0x8)))
        {
            result = null;
            return false;
        }

        if (!Game.ReadString(deref + (Is64Bit ? 0x14 : 0xC), ReadStringType.UTF16, length * 2, out result))
        {
            result = null;
            return false;
        }

        return true;
    }
    #endregion
}
