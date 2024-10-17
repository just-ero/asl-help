using System;

using AslHelp.Memory;

public partial class Basic
{
    public T[] ReadArray<T>(int length, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(out T[]? result, length, baseOffset, offsets);
        return result ?? [];
    }

    public T[] ReadArray<T>(int length, string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(out T[]? result, length, moduleName, baseOffset, offsets);
        return result ?? [];
    }

    public T[] ReadArray<T>(int length, Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(out T[]? result, length, module, baseOffset, offsets);
        return result ?? [];
    }

    public T[] ReadArray<T>(int length, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(out T[]? result, length, baseAddress, offsets);
        return result ?? [];
    }

    public void ReadArray<T>(Span<T> buffer, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(buffer, baseOffset, offsets);
    }

    public void ReadArray<T>(Span<T> buffer, string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(buffer, moduleName, baseOffset, offsets);
    }

    public void ReadArray<T>(Span<T> buffer, Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(buffer, module, baseOffset, offsets);
    }

    public void ReadArray<T>(Span<T> buffer, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryReadArray(buffer, baseAddress, offsets);
    }
}
