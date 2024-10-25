using System.Collections.Generic;

using AslHelp.Memory;

public partial class Unity
{
    public IReadOnlyList<T> ReadList<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadList(out IReadOnlyList<T>? result, baseOffset, offsets);
        return result ?? [];
    }

    public IReadOnlyList<T> ReadList<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadList(out IReadOnlyList<T>? result, moduleName, baseOffset, offsets);
        return result ?? [];
    }

    public IReadOnlyList<T> ReadList<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadList(out IReadOnlyList<T>? result, module, baseOffset, offsets);
        return result ?? [];
    }

    public IReadOnlyList<T> ReadList<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadList(out IReadOnlyList<T>? result, baseAddress, offsets);
        return result ?? [];
    }

    public IReadOnlyList<string?> ReadList(int baseOffset, params int[] offsets)
    {
        Memory.TryReadList(out IReadOnlyList<string?>? result, baseOffset, offsets);
        return result ?? [];
    }

    public IReadOnlyList<string?> ReadList(string moduleName, int baseOffset, params int[] offsets)
    {
        Memory.TryReadList(out IReadOnlyList<string?>? result, moduleName, baseOffset, offsets);
        return result ?? [];
    }

    public IReadOnlyList<string?> ReadList(Module module, int baseOffset, params int[] offsets)
    {
        Memory.TryReadList(out IReadOnlyList<string?>? result, module, baseOffset, offsets);
        return result ?? [];
    }

    public IReadOnlyList<string?> ReadList(nint baseAddress, params int[] offsets)
    {
        Memory.TryReadList(out IReadOnlyList<string?>? result, baseAddress, offsets);
        return result ?? [];
    }
}
