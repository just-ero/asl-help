using AslHelp.Memory;

public partial class Unity
{
    public string? ReadString(int baseOffset, params int[] offsets)
    {
        Memory.TryReadString(out string? result, baseOffset, offsets);
        return result;
    }

    public string? ReadString(string moduleName, int baseOffset, params int[] offsets)
    {
        Memory.TryReadString(out string? result, moduleName, baseOffset, offsets);
        return result;
    }

    public string? ReadString(Module module, int baseOffset, params int[] offsets)
    {
        Memory.TryReadString(out string? result, module, baseOffset, offsets);
        return result;
    }

    public string? ReadString(nint baseAddress, params int[] offsets)
    {
        Memory.TryReadString(out string? result, baseAddress, offsets);
        return result;
    }
}
