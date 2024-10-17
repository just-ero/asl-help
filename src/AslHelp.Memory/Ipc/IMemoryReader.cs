extern alias Ls;

using System;
using System.Diagnostics.CodeAnalysis;

using Ls::LiveSplit.ComponentUtil;

namespace AslHelp.Memory.Ipc;

public interface IMemoryReader
{
    nint Deref(int baseOffset, params int[] offsets);
    nint Deref(string moduleName, int baseOffset, params int[] offsets);
    nint Deref(Module module, int baseOffset, params int[] offsets);
    nint Deref(nint baseAddress, params int[] offsets);

    bool TryDeref(out nint result, int baseOffset, params int[] offsets);
    bool TryDeref(out nint result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryDeref(out nint result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryDeref(out nint result, nint baseAddress, params int[] offsets);

    T Read<T>(int baseOffset, params int[] offsets) where T : unmanaged;
    T Read<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    T Read<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    T Read<T>(nint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryRead<T>(out T result, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryRead<T>(out T result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryRead<T>(out T result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryRead<T>(out T result, nint baseAddress, params int[] offsets) where T : unmanaged;

    T[] ReadArray<T>(int length, int baseOffset, params int[] offsets) where T : unmanaged;
    T[] ReadArray<T>(int length, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    T[] ReadArray<T>(int length, Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    T[] ReadArray<T>(int length, nint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, int length, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, int length, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, int length, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, int length, nint baseAddress, params int[] offsets) where T : unmanaged;

    void ReadArray<T>(Span<T> buffer, int baseOffset, params int[] offsets) where T : unmanaged;
    void ReadArray<T>(Span<T> buffer, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    void ReadArray<T>(Span<T> buffer, Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    void ReadArray<T>(Span<T> buffer, nint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryReadArray<T>(Span<T> buffer, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>(Span<T> buffer, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>(Span<T> buffer, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>(Span<T> buffer, nint baseAddress, params int[] offsets) where T : unmanaged;

    string ReadString(int maxLength, ReadStringType stringType, int baseOffset, params int[] offsets);
    string ReadString(int maxLength, ReadStringType stringType, string moduleName, int baseOffset, params int[] offsets);
    string ReadString(int maxLength, ReadStringType stringType, Module module, int baseOffset, params int[] offsets);
    string ReadString(int maxLength, ReadStringType stringType, nint baseAddress, params int[] offsets);

    bool TryReadString([NotNullWhen(true)] out string? result, int maxLength, ReadStringType stringType, int baseOffset, params int[] offsets);
    bool TryReadString([NotNullWhen(true)] out string? result, int maxLength, ReadStringType stringType, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadString([NotNullWhen(true)] out string? result, int maxLength, ReadStringType stringType, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadString([NotNullWhen(true)] out string? result, int maxLength, ReadStringType stringType, nint baseAddress, params int[] offsets);
}
