using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using AslHelp.Shared;

namespace AslHelp.Unity.Collections;

internal sealed partial class Net35Dictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    where TKey : unmanaged
    where TValue : unmanaged
{
    private readonly int[] _table;
    private readonly Link[] _linkSlots;

    private readonly TKey[] _keySlots;
    private readonly TValue[] _valueSlots;

    public Net35Dictionary(int count, int[] table, Link[] linkSlots, TKey[] keySlots, TValue[] valueSlots)
    {
        _table = table;
        _linkSlots = linkSlots;
        _keySlots = keySlots;
        _valueSlots = valueSlots;

        Count = count;
    }

    public int Count { get; }

    public IEnumerable<TKey> Keys
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

    public unsafe TValue this[TKey key]
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

    public unsafe bool ContainsKey(TKey key)
    {
        return Unsafe.AsPointer(ref FindValue(key)) != null;
    }

    public unsafe bool TryGetValue(TKey key, out TValue value)
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

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private ref int GetBucket(uint hashCode)
    {
        return ref _table[(hashCode & 0x7FFFFFFF) % _table.Length];
    }

    private unsafe ref TValue FindValue(TKey key)
    {
        ref TValue value = ref Unsafe.AsRef<TValue>(null);

        Link[] linkSlots = _linkSlots;

        uint hashCode = (uint)key.GetHashCode();
        int i = GetBucket(hashCode) - 1;

        while (i >= 0)
        {
            if ((uint)i > (uint)linkSlots.Length)
            {
                break;
            }

            if (linkSlots[i].HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(_keySlots[i], key))
            {
                value = ref _valueSlots[i];
                break;
            }

            i = linkSlots[i].Next;
        }

        return ref value;
    }
}
