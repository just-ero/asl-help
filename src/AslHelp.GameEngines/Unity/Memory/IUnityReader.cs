using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Memory.Ipc;

namespace AslHelp.GameEngines.Unity.Memory;

public interface IUnityReader : IProcessMemory
{
    T[]? ReadArray<T>(int baseOffset, params int[] offsets) where T : unmanaged;
    T[]? ReadArray<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    T[]? ReadArray<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    T[]? ReadArray<T>(nint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryReadArray<T>(out T[]? result, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>(out T[]? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>(out T[]? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>(out T[]? result, nint baseAddress, params int[] offsets) where T : unmanaged;

    string?[]? ReadArray(int baseOffset, params int[] offsets);
    string?[]? ReadArray(string moduleName, int baseOffset, params int[] offsets);
    string?[]? ReadArray(Module module, int baseOffset, params int[] offsets);
    string?[]? ReadArray(nint baseAddress, params int[] offsets);

    bool TryReadArray(out string?[]? result, int baseOffset, params int[] offsets);
    bool TryReadArray(out string?[]? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadArray(out string?[]? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadArray(out string?[]? result, nint baseAddress, params int[] offsets);

    IReadOnlyList<T>? ReadList<T>(int baseOffset, params int[] offsets) where T : unmanaged;
    IReadOnlyList<T>? ReadList<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    IReadOnlyList<T>? ReadList<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    IReadOnlyList<T>? ReadList<T>(nint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryReadList<T>(out IReadOnlyList<T>? result, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadList<T>(out IReadOnlyList<T>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadList<T>(out IReadOnlyList<T>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadList<T>(out IReadOnlyList<T>? result, nint baseAddress, params int[] offsets) where T : unmanaged;

    IReadOnlyList<string?>? ReadList(int baseOffset, params int[] offsets);
    IReadOnlyList<string?>? ReadList(string moduleName, int baseOffset, params int[] offsets);
    IReadOnlyList<string?>? ReadList(Module module, int baseOffset, params int[] offsets);
    IReadOnlyList<string?>? ReadList(nint baseAddress, params int[] offsets);

    bool TryReadList(out IReadOnlyList<string?>? result, int baseOffset, params int[] offsets);
    bool TryReadList(out IReadOnlyList<string?>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadList(out IReadOnlyList<string?>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadList(out IReadOnlyList<string?>? result, nint baseAddress, params int[] offsets);

    ISet<T>? ReadSet<T>(int baseOffset, params int[] offsets) where T : unmanaged;
    ISet<T>? ReadSet<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    ISet<T>? ReadSet<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    ISet<T>? ReadSet<T>(nint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryReadSet<T>(out ISet<T>? result, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSet<T>(out ISet<T>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSet<T>(out ISet<T>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSet<T>(out ISet<T>? result, nint baseAddress, params int[] offsets) where T : unmanaged;

    ISet<string?>? ReadSet(int baseOffset, params int[] offsets);
    ISet<string?>? ReadSet(string moduleName, int baseOffset, params int[] offsets);
    ISet<string?>? ReadSet(Module module, int baseOffset, params int[] offsets);
    ISet<string?>? ReadSet(nint baseAddress, params int[] offsets);

    bool TryReadSet(out ISet<string?>? result, int baseOffset, params int[] offsets);
    bool TryReadSet(out ISet<string?>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadSet(out ISet<string?>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadSet(out ISet<string?>? result, nint baseAddress, params int[] offsets);

    IReadOnlyDictionary<TKey, TValue>? ReadDictionary<TKey, TValue>(int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    IReadOnlyDictionary<TKey, TValue>? ReadDictionary<TKey, TValue>(string moduleName, int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    IReadOnlyDictionary<TKey, TValue>? ReadDictionary<TKey, TValue>(Module module, int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    IReadOnlyDictionary<TKey, TValue>? ReadDictionary<TKey, TValue>(nint baseAddress, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;

    bool TryReadDictionary<TKey, TValue>(out IReadOnlyDictionary<TKey, TValue>? result, int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    bool TryReadDictionary<TKey, TValue>(out IReadOnlyDictionary<TKey, TValue>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    bool TryReadDictionary<TKey, TValue>(out IReadOnlyDictionary<TKey, TValue>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    bool TryReadDictionary<TKey, TValue>(out IReadOnlyDictionary<TKey, TValue>? result, nint baseAddress, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;

    IReadOnlyDictionary<string, TValue>? ReadDictionary<TValue>(int baseOffset, params int[] offsets) where TValue : unmanaged;
    IReadOnlyDictionary<string, TValue>? ReadDictionary<TValue>(string moduleName, int baseOffset, params int[] offsets) where TValue : unmanaged;
    IReadOnlyDictionary<string, TValue>? ReadDictionary<TValue>(Module module, int baseOffset, params int[] offsets) where TValue : unmanaged;
    IReadOnlyDictionary<string, TValue>? ReadDictionary<TValue>(nint baseAddress, params int[] offsets) where TValue : unmanaged;

    bool TryReadDictionary<TValue>(out IReadOnlyDictionary<string, TValue>? result, int baseOffset, params int[] offsets) where TValue : unmanaged;
    bool TryReadDictionary<TValue>(out IReadOnlyDictionary<string, TValue>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where TValue : unmanaged;
    bool TryReadDictionary<TValue>(out IReadOnlyDictionary<string, TValue>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where TValue : unmanaged;
    bool TryReadDictionary<TValue>(out IReadOnlyDictionary<string, TValue>? result, nint baseAddress, params int[] offsets) where TValue : unmanaged;

    IReadOnlyDictionary<string, string?>? ReadDictionary(int baseOffset, params int[] offsets);
    IReadOnlyDictionary<string, string?>? ReadDictionary(string moduleName, int baseOffset, params int[] offsets);
    IReadOnlyDictionary<string, string?>? ReadDictionary(Module module, int baseOffset, params int[] offsets);
    IReadOnlyDictionary<string, string?>? ReadDictionary(nint baseAddress, params int[] offsets);

    bool TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, int baseOffset, params int[] offsets);
    bool TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadDictionary(out IReadOnlyDictionary<string, string?>? result, nint baseAddress, params int[] offsets);

    string? ReadString(int baseOffset, params int[] offsets);
    string? ReadString(string moduleName, int baseOffset, params int[] offsets);
    string? ReadString(Module module, int baseOffset, params int[] offsets);
    string? ReadString(nint baseAddress, params int[] offsets);

    bool TryReadString(out string? result, int baseOffset, params int[] offsets);
    bool TryReadString(out string? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadString(out string? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadString(out string? result, nint baseAddress, params int[] offsets);
}
