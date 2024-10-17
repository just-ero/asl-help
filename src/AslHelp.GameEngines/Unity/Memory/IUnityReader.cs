using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory;
using AslHelp.Memory.Ipc;

namespace AslHelp.GameEngines.Unity.Memory;

public interface IUnityReader : IProcessMemory
{
    T[] ReadArray<T>(int baseOffset, params int[] offsets) where T : unmanaged;
    T[] ReadArray<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    T[] ReadArray<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    T[] ReadArray<T>(nint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadArray<T>([NotNullWhen(true)] out T[]? result, nint baseAddress, params int[] offsets) where T : unmanaged;

    string[] ReadArray(int baseOffset, params int[] offsets);
    string[] ReadArray(string moduleName, int baseOffset, params int[] offsets);
    string[] ReadArray(Module module, int baseOffset, params int[] offsets);
    string[] ReadArray(nint baseAddress, params int[] offsets);

    bool TryReadArray([NotNullWhen(true)] out string[]? result, int baseOffset, params int[] offsets);
    bool TryReadArray([NotNullWhen(true)] out string[]? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadArray([NotNullWhen(true)] out string[]? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadArray([NotNullWhen(true)] out string[]? result, nint baseAddress, params int[] offsets);

    List<T> ReadList<T>(int baseOffset, params int[] offsets) where T : unmanaged;
    List<T> ReadList<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    List<T> ReadList<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    List<T> ReadList<T>(nint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryReadList<T>([NotNullWhen(true)] out List<T>? result, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadList<T>([NotNullWhen(true)] out List<T>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadList<T>([NotNullWhen(true)] out List<T>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadList<T>([NotNullWhen(true)] out List<T>? result, nint baseAddress, params int[] offsets) where T : unmanaged;

    List<string> ReadList(int baseOffset, params int[] offsets);
    List<string> ReadList(string moduleName, int baseOffset, params int[] offsets);
    List<string> ReadList(Module module, int baseOffset, params int[] offsets);
    List<string> ReadList(nint baseAddress, params int[] offsets);

    bool TryReadList([NotNullWhen(true)] out List<string>? result, int baseOffset, params int[] offsets);
    bool TryReadList([NotNullWhen(true)] out List<string>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadList([NotNullWhen(true)] out List<string>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadList([NotNullWhen(true)] out List<string>? result, nint baseAddress, params int[] offsets);

    HashSet<T> ReadSet<T>(int baseOffset, params int[] offsets) where T : unmanaged;
    HashSet<T> ReadSet<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    HashSet<T> ReadSet<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged;
    HashSet<T> ReadSet<T>(nint baseAddress, params int[] offsets) where T : unmanaged;

    bool TryReadSet<T>([NotNullWhen(true)] out HashSet<T>? result, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSet<T>([NotNullWhen(true)] out HashSet<T>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSet<T>([NotNullWhen(true)] out HashSet<T>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where T : unmanaged;
    bool TryReadSet<T>([NotNullWhen(true)] out HashSet<T>? result, nint baseAddress, params int[] offsets) where T : unmanaged;

    HashSet<string> ReadSet(int baseOffset, params int[] offsets);
    HashSet<string> ReadSet(string moduleName, int baseOffset, params int[] offsets);
    HashSet<string> ReadSet(Module module, int baseOffset, params int[] offsets);
    HashSet<string> ReadSet(nint baseAddress, params int[] offsets);

    bool TryReadSet([NotNullWhen(true)] out HashSet<string>? result, int baseOffset, params int[] offsets);
    bool TryReadSet([NotNullWhen(true)] out HashSet<string>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadSet([NotNullWhen(true)] out HashSet<string>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadSet([NotNullWhen(true)] out HashSet<string>? result, nint baseAddress, params int[] offsets);

    IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(string moduleName, int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(Module module, int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    IReadOnlyDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(nint baseAddress, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;

    bool TryReadDictionary<TKey, TValue>([NotNullWhen(true)] out IReadOnlyDictionary<TKey, TValue>? result, int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    bool TryReadDictionary<TKey, TValue>([NotNullWhen(true)] out IReadOnlyDictionary<TKey, TValue>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    bool TryReadDictionary<TKey, TValue>([NotNullWhen(true)] out IReadOnlyDictionary<TKey, TValue>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;
    bool TryReadDictionary<TKey, TValue>([NotNullWhen(true)] out IReadOnlyDictionary<TKey, TValue>? result, nint baseAddress, params int[] offsets) where TKey : unmanaged where TValue : unmanaged;

    IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(int baseOffset, params int[] offsets) where TValue : unmanaged;
    IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(string moduleName, int baseOffset, params int[] offsets) where TValue : unmanaged;
    IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(Module module, int baseOffset, params int[] offsets) where TValue : unmanaged;
    IReadOnlyDictionary<string, TValue> ReadDictionary<TValue>(nint baseAddress, params int[] offsets) where TValue : unmanaged;

    bool TryReadDictionary<TValue>([NotNullWhen(true)] out IReadOnlyDictionary<string, TValue>? result, int baseOffset, params int[] offsets) where TValue : unmanaged;
    bool TryReadDictionary<TValue>([NotNullWhen(true)] out IReadOnlyDictionary<string, TValue>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets) where TValue : unmanaged;
    bool TryReadDictionary<TValue>([NotNullWhen(true)] out IReadOnlyDictionary<string, TValue>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets) where TValue : unmanaged;
    bool TryReadDictionary<TValue>([NotNullWhen(true)] out IReadOnlyDictionary<string, TValue>? result, nint baseAddress, params int[] offsets) where TValue : unmanaged;

    IReadOnlyDictionary<string, string> ReadDictionary(int baseOffset, params int[] offsets);
    IReadOnlyDictionary<string, string> ReadDictionary(string moduleName, int baseOffset, params int[] offsets);
    IReadOnlyDictionary<string, string> ReadDictionary(Module module, int baseOffset, params int[] offsets);
    IReadOnlyDictionary<string, string> ReadDictionary(nint baseAddress, params int[] offsets);

    bool TryReadDictionary([NotNullWhen(true)] out IReadOnlyDictionary<string, string>? result, int baseOffset, params int[] offsets);
    bool TryReadDictionary([NotNullWhen(true)] out IReadOnlyDictionary<string, string>? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadDictionary([NotNullWhen(true)] out IReadOnlyDictionary<string, string>? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadDictionary([NotNullWhen(true)] out IReadOnlyDictionary<string, string>? result, nint baseAddress, params int[] offsets);

    string ReadString(int baseOffset, params int[] offsets);
    string ReadString(string moduleName, int baseOffset, params int[] offsets);
    string ReadString(Module module, int baseOffset, params int[] offsets);
    string ReadString(nint baseAddress, params int[] offsets);

    bool TryReadString([NotNullWhen(true)] out string? result, int baseOffset, params int[] offsets);
    bool TryReadString([NotNullWhen(true)] out string? result, [NotNullWhen(true)] string? moduleName, int baseOffset, params int[] offsets);
    bool TryReadString([NotNullWhen(true)] out string? result, [NotNullWhen(true)] Module? module, int baseOffset, params int[] offsets);
    bool TryReadString([NotNullWhen(true)] out string? result, nint baseAddress, params int[] offsets);
}
