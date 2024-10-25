using System.Collections.Generic;

using AslHelp.Memory;

public partial class Unity
{
    public ISet<T> ReadSet<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadSet(out ISet<T>? result, baseOffset, offsets);
        return result ?? new HashSet<T>();
    }

    public ISet<T> ReadSet<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadSet(out ISet<T>? result, moduleName, baseOffset, offsets);
        return result ?? new HashSet<T>();
    }

    public ISet<T> ReadSet<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadSet(out ISet<T>? result, module, baseOffset, offsets);
        return result ?? new HashSet<T>();
    }

    public ISet<T> ReadSet<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadSet(out ISet<T>? result, baseAddress, offsets);
        return result ?? new HashSet<T>();
    }

    public ISet<string?> ReadSet(int baseOffset, params int[] offsets)
    {
        Memory.TryReadSet(out ISet<string?>? result, baseOffset, offsets);
        return result ?? new HashSet<string?>();
    }

    public ISet<string?> ReadSet(string moduleName, int baseOffset, params int[] offsets)
    {
        Memory.TryReadSet(out ISet<string?>? result, moduleName, baseOffset, offsets);
        return result ?? new HashSet<string?>();
    }

    public ISet<string?> ReadSet(Module module, int baseOffset, params int[] offsets)
    {
        Memory.TryReadSet(out ISet<string?>? result, module, baseOffset, offsets);
        return result ?? new HashSet<string?>();
    }

    public ISet<string?> ReadSet(nint baseAddress, params int[] offsets)
    {
        Memory.TryReadSet(out ISet<string?>? result, baseAddress, offsets);
        return result ?? new HashSet<string?>();
    }
}
