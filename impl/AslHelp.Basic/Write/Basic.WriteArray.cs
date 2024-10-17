using System;

using AslHelp.Memory;

public partial class Basic
{
    public void WriteArray<T>(ReadOnlySpan<T> values, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryWriteArray(values, baseOffset, offsets);
    }

    public void WriteArray<T>(ReadOnlySpan<T> values, string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryWriteArray(values, moduleName, baseOffset, offsets);
    }

    public void WriteArray<T>(ReadOnlySpan<T> values, Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryWriteArray(values, module, baseOffset, offsets);
    }

    public void WriteArray<T>(ReadOnlySpan<T> values, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        Memory.TryWriteArray(values, baseAddress, offsets);
    }
}
