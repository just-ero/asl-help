using AslHelp.Memory;

public partial class Basic
{
    public T Read<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryRead(out T result, baseOffset, offsets);
        return result;
    }

    public T Read<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryRead(out T result, moduleName, baseOffset, offsets);
        return result;
    }

    public T Read<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryRead(out T result, module, baseOffset, offsets);
        return result;
    }

    public T Read<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryRead(out T result, baseAddress, offsets);
        return result;
    }
}
