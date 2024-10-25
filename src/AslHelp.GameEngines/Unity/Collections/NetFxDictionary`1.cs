using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using AslHelp.GameEngines.Unity.Memory;
using AslHelp.Shared;
using AslHelp.Shared.Extensions;

namespace AslHelp.GameEngines.Unity.Collections;

internal sealed partial class NetFxDictionary<TValue>(
    IUnityReader memory,
    int[] buckets,
    NetFxDictionary<TValue>.Entry[] entries,
    int count) : IReadOnlyDictionary<string, TValue>
    where TValue : unmanaged
{
    private readonly int[] _buckets = buckets;
    private readonly Entry[] _entries = entries;

    private readonly IUnityReader _memory = memory;
    private readonly string?[] _keyCache = new string?[entries.Length];

    public int Count { get; } = count;

    public IEnumerable<string> Keys
    {
        get
        {
            Enumerator enumerator = new(this);
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current.Key;
            }
        }
    }

    public IEnumerable<TValue> Values
    {
        get
        {
            Enumerator enumerator = new(this);
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current.Value;
            }
        }
    }

    public unsafe TValue this[string key]
    {
        get
        {
            ref TValue value = ref FindValue(key);
            if (Unsafe.AsPointer(ref value) != null)
            {
                return value;
            }

            string msg = $"The given key '{key}' was not present in the dictionary.";
            ThrowHelper.ThrowKeyNotFoundException(msg);

            return default;
        }
    }

    public unsafe bool ContainsKey(string key)
    {
        return Unsafe.AsPointer(ref FindValue(key)) != null;
    }

    public unsafe bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
    {
        ref TValue valRef = ref FindValue(key);
        if (Unsafe.AsPointer(ref valRef) != null)
        {
            value = valRef;
            return true;
        }

        value = default;
        return false;
    }

    public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private ref int GetBucket(uint hashCode)
    {
        return ref _buckets[(hashCode & int.MaxValue) % _buckets.Length];
    }

    private unsafe ref TValue FindValue(string key)
    {
        ref TValue value = ref Unsafe.AsRef<TValue>(null);

        Entry[] entries = _entries;

        uint hashCode = (uint)key.GetHashCode();
        int i = GetBucket(hashCode) - 1;

        while (i >= 0)
        {
            if ((uint)i >= (uint)entries.Length)
            {
                break;
            }

            ref Entry entry = ref entries[i];
            if (entry.HashCode == hashCode
                && GetKey(i, entry) == key)
            {
                value = ref entry.Value;
                break;
            }

            i = entry.Next;
        }

        return ref value;
    }

    /// <remarks>
    ///     Same implementation as <see cref="UnityMemory.ReadString(nint, int[])"/>.<br/>
    ///     Can't call that method since it expects the address of the pointer to the string.<br/>
    ///     Might need to introduce methods which accept the raw starting address.
    /// </remarks>
    private string GetKey(int index, Entry entry)
    {
        if (_keyCache[index] is string key)
        {
            return key;
        }

        nint deref = _entries[index].Key;
        int length = _memory.Read<int>(deref + (_memory.PointerSize * 2));

        char[]? rented = null;
        Span<char> buffer = length <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(length));

        _memory.ReadArray(buffer[..length], deref + (_memory.PointerSize * 2) + sizeof(int));
        string result = buffer[..length].ToString();

        ArrayPool<char>.Shared.ReturnIfNotNull(rented);
        _keyCache[index] = result;

        return result;
    }
}
