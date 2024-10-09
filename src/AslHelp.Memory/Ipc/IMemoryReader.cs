using System;
using System.Diagnostics.CodeAnalysis;

namespace AslHelp.Memory.Ipc;

public interface IMemoryReader
{
    nuint Deref(int baseOffset, params int[] offsets);
    nuint Deref(string moduleName, int baseOffset, params int[] offsets);
    nuint Deref(Module module, int baseOffset, params int[] offsets);
    nuint Deref(nuint baseAddress, params int[] offsets);

    bool TryDeref(out nuint result, int baseOffset, params int[] offsets);
    bool TryDeref(out nuint result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryDeref(out nuint result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryDeref(out nuint result, nuint baseAddress, params int[] offsets);

    T Read<T>(int baseOffset, params int[] offsets) where T : unmanaged;
    T Read<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    T Read<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    T Read<T>(nuint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryRead<T>(out T result, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryRead<T>(out T result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryRead<T>(out T result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryRead<T>(out T result, nuint baseAddress, params int[] offsets) where T : unmanaged;

    T[] ReadSpan<T>(nuint length, int baseOffset, params int[] offsets) where T : unmanaged;
    T[] ReadSpan<T>(nuint length, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    T[] ReadSpan<T>(nuint length, Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    T[] ReadSpan<T>(nuint length, nuint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryReadSpan<T>([NotNullWhen(true)] out T[]? result, nuint length, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSpan<T>([NotNullWhen(true)] out T[]? result, nuint length, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSpan<T>([NotNullWhen(true)] out T[]? result, nuint length, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSpan<T>([NotNullWhen(true)] out T[]? result, nuint length, nuint baseAddress, params int[] offsets) where T : unmanaged;

    void ReadSpan<T>(Span<T> buffer, int baseOffset, params int[] offsets) where T : unmanaged;
    void ReadSpan<T>(Span<T> buffer, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    void ReadSpan<T>(Span<T> buffer, Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    void ReadSpan<T>(Span<T> buffer, nuint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryReadSpan<T>(Span<T> buffer, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSpan<T>(Span<T> buffer, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSpan<T>(Span<T> buffer, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSpan<T>(Span<T> buffer, nuint baseAddress, params int[] offsets) where T : unmanaged;

    string ReadString(int maxLength, StringType stringType, int baseOffset, params int[] offsets);
    string ReadString(int maxLength, StringType stringType, string moduleName, int baseOffset, params int[] offsets);
    string ReadString(int maxLength, StringType stringType, Module module, int baseOffset, params int[] offsets);
    string ReadString(int maxLength, StringType stringType, nuint baseAddress, params int[] offsets);

    bool TryReadString([NotNullWhen(true)] out string? result, int maxLength, StringType stringType, int baseOffset, params int[] offsets);
    bool TryReadString([NotNullWhen(true)] out string? result, int maxLength, StringType stringType, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadString([NotNullWhen(true)] out string? result, int maxLength, StringType stringType, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadString([NotNullWhen(true)] out string? result, int maxLength, StringType stringType, nuint baseAddress, params int[] offsets);
}
