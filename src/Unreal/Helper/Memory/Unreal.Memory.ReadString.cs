using LiveSplit.ComponentUtil;

public partial class Unreal
{
    #region ReadString
    public string ReadString(int length, int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, length, MainModule, baseOffset, offsets);
        return result;
    }

    public string ReadString(int length, string module, int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, length, Modules[module], baseOffset, offsets);
        return result;
    }

    public string ReadString(int length, Module module, int baseOffset, params int[] offsets)
    {
        TryReadString(out string result, length, module, baseOffset, offsets);
        return result;
    }

    public string ReadString(int length, nint baseAddress, params int[] offsets)
    {
        TryReadString(out string result, length, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadString
    public bool TryReadString(out string result, int length, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, length, MainModule, baseOffset, offsets);
    }

    public bool TryReadString(out string result, int length, string module, int baseOffset, params int[] offsets)
    {
        return TryReadString(out result, length, Modules[module], baseOffset, offsets);
    }

    public bool TryReadString(out string result, int length, Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            Debug.Warn("[Unreal.ReadString] Module could not be found.");

            result = default;
            return false;
        }

        return TryReadString(out result, length, module.Base + baseOffset, offsets);
    }

    public bool TryReadString(out string result, int length, nint baseAddress, params int[] offsets)
    {
        if (!TryRead<nint>(out nint deref, baseAddress, offsets))
        {
            result = null;
            return false;
        }

        if (!Game.ReadString(deref, ReadStringType.UTF16, length, out result))
        {
            result = null;
            return false;
        }

        return true;
    }
    #endregion
}
