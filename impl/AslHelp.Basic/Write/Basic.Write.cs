using AslHelp.Memory;

public partial class Basic
{
    public void Write<T>(T value, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryWrite(value, baseOffset, offsets);
    }

    public void Write<T>(T value, string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryWrite(value, moduleName, baseOffset, offsets);
    }

    public void Write<T>(T value, Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryWrite(value, module, baseOffset, offsets);
    }

    public void Write<T>(T value, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryWrite(value, baseAddress, offsets);
    }
}
