using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using AslHelp.Shared;

namespace AslHelp.GameEngines.Unity.Collections;

#error Check this again.
internal sealed partial class NetFxDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    where TKey : unmanaged
    where TValue : unmanaged
{
    private readonly int[] _buckets;
    private readonly Entry[] _entries;

    public NetFxDictionary(int count, int[] buckets, Entry[] entries)
    {
        _buckets = buckets;
        _entries = entries;

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

    public unsafe bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
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
        return ref _buckets[(hashCode & 0x7FFFFFFF) % _buckets.Length];
    }

    private unsafe ref TValue FindValue(TKey key)
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
            if (entry.HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
            {
                value = ref entry.Value;
                break;
            }

            i = entry.Next;
        }

        return ref value;
    }
}
