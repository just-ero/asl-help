using System;
using System.Diagnostics.CodeAnalysis;

namespace AslHelp.Memory.Ipc;

public interface IMemoryWriter
{
    void Write<T>(T value, int baseOffset, params int[] offsets) where T : unmanaged;
    void Write<T>(T value, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    void Write<T>(T value, Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    void Write<T>(T value, nuint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryWrite<T>(T value, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryWrite<T>(T value, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryWrite<T>(T value, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryWrite<T>(T value, nuint baseAddress, params int[] offsets) where T : unmanaged;

    void WriteArray<T>(ReadOnlySpan<T> values, int baseOffset, params int[] offsets) where T : unmanaged;
    void WriteArray<T>(ReadOnlySpan<T> values, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    void WriteArray<T>(ReadOnlySpan<T> values, Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    void WriteArray<T>(ReadOnlySpan<T> values, nuint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryWriteArray<T>(ReadOnlySpan<T> values, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryWriteArray<T>(ReadOnlySpan<T> values, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryWriteArray<T>(ReadOnlySpan<T> values, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryWriteArray<T>(ReadOnlySpan<T> values, nuint baseAddress, params int[] offsets) where T : unmanaged;
}
