using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using AslHelp.GameEngines.Unity.Memory;
using AslHelp.Shared;
using AslHelp.Shared.Extensions;

namespace AslHelp.GameEngines.Unity.Collections;

internal sealed partial class MonoDictionary<TValue>(
    IUnityReader memory,
    int[] table,
    Link[] linkSlots,
    nint[] keySlots,
    TValue[] valueSlots,
    int touchedSlots,
    int count) : IReadOnlyDictionary<string, TValue>
    where TValue : unmanaged
{
    private const int NoSlot = -1;
    private const int HashFlag = int.MinValue;

    private readonly int[] _table = table;
    private readonly Link[] _linkSlots = linkSlots;

    private readonly nint[] _keySlots = keySlots;
    private readonly TValue[] _valueSlots = valueSlots;

    private readonly int _touchedSlots = touchedSlots;

    private readonly IUnityReader _memory = memory;
    private readonly string?[] _keyCache = new string?[keySlots.Length];

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
            ThrowHelper.ThrowIfNull(key);

            ref TValue value = ref FindValue(key);
            if (Unsafe.AsPointer(ref value) == null)
            {
                string msg = $"The given key '{key}' was not present in the dictionary.";
                ThrowHelper.ThrowKeyNotFoundException(msg);
            }

            return value;
        }
    }

    public unsafe bool ContainsKey(string key)
    {
        ThrowHelper.ThrowIfNull(key);

        return Unsafe.AsPointer(ref FindValue(key)) != null;
    }

    public unsafe bool TryGetValue(string key, out TValue value)
    {
        ThrowHelper.ThrowIfNull(key);

        ref TValue valRef = ref FindValue(key);
        if (Unsafe.AsPointer(ref valRef) == null)
        {
            value = default;
            return false;
        }

        value = valRef;
        return true;
    }

    public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private int GetKeyHashCode(string key)
    {
        return key.GetHashCode() | HashFlag;
    }

    private ref int GetSlot(int hashCode)
    {
        return ref _table[(hashCode & int.MaxValue) % _table.Length];
    }

    private unsafe ref TValue FindValue(string key)
    {
        ref TValue value = ref Unsafe.AsRef<TValue>(null);

        Link[] linkSlots = _linkSlots;

        int hashCode = GetKeyHashCode(key);
        int i = GetSlot(hashCode) - 1;

        while (i != NoSlot)
        {
            Link link = linkSlots[i];
            if (link.HashCode == hashCode
                && GetKey(i) == key)
            {
                value = ref _valueSlots[i];
                break;
            }

            i = link.Next;
        }

        return ref value;
    }

    /// <remarks>
    ///     Same implementation as <see cref="UnityMemory.ReadString(nint, int[])"/>.<br/>
    ///     Can't call that method since it expects the address of the pointer to the string.<br/>
    ///     Might need to introduce methods which accept the raw starting address.
    /// </remarks>
    private string GetKey(int index)
    {
        if (_keyCache[index] is string key)
        {
            return key;
        }

        nint deref = _keySlots[index];
        int length = _memory.Read<int>(deref + (_memory.PointerSize * 2));

        char[]? rented = null;
        Span<char> buffer = length <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(length));

        _memory.ReadArray(buffer[..length], deref + (_memory.PointerSize * 2) + sizeof(int));
        key = buffer[..length].ToString();

        ArrayPool<char>.Shared.ReturnIfNotNull(rented);
        _keyCache[index] = key;

        return key;
    }
}
