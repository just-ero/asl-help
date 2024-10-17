using AslHelp.Memory;

public partial class Basic
{
    public nint Deref(int baseOffset, params int[] offsets)
    {
        Memory.TryDeref(out nint result, baseOffset, offsets);
        return result;
    }

    public nint Deref(string moduleName, int baseOffset, params int[] offsets)
    {
        Memory.TryDeref(out nint result, moduleName, baseOffset, offsets);
        return result;
    }

    public nint Deref(Module module, int baseOffset, params int[] offsets)
    {
        Memory.TryDeref(out nint result, module, baseOffset, offsets);
        return result;
    }

    public nint Deref(nint baseAddress, params int[] offsets)
    {
        Memory.TryDeref(out nint result, baseAddress, offsets);
        return result;
    }
}
