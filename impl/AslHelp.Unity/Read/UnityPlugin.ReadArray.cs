using AslHelp.Memory;

public partial class Unity
{
    public T[] ReadArray<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(out T[]? result, baseOffset, offsets);
        return result ?? [];
    }

    public T[] ReadArray<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(out T[]? result, moduleName, baseOffset, offsets);
        return result ?? [];
    }

    public T[] ReadArray<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(out T[]? result, module, baseOffset, offsets);
        return result ?? [];
    }

    public T[] ReadArray<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(out T[]? result, baseAddress, offsets);
        return result ?? [];
    }
}
