extern alias Ls;

using AslHelp.Memory;

using Ls::LiveSplit.ComponentUtil;

public partial class Basic
{
    public string? ReadString(int maxLength, ReadStringType stringType, int baseOffset, params int[] offsets)
    {
        Memory.TryReadString(out string? result, maxLength, stringType, baseOffset, offsets);
        return result;
    }

    public string? ReadString(int maxLength, ReadStringType stringType, string moduleName, int baseOffset, params int[] offsets)
    {
        Memory.TryReadString(out string? result, maxLength, stringType, moduleName, baseOffset, offsets);
        return result;
    }

    public string? ReadString(int maxLength, ReadStringType stringType, Module module, int baseOffset, params int[] offsets)
    {
        Memory.TryReadString(out string? result, maxLength, stringType, module, baseOffset, offsets);
        return result;
    }

    public string? ReadString(int maxLength, ReadStringType stringType, nint baseAddress, params int[] offsets)
    {
        Memory.TryReadString(out string? result, maxLength, stringType, baseAddress, offsets);
        return result;
    }
}
