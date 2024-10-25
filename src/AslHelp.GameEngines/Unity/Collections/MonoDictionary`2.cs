using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using AslHelp.Shared;

namespace AslHelp.GameEngines.Unity.Collections;

internal sealed partial class MonoDictionary<TKey, TValue>(
    int[] table,
    Link[] linkSlots,
    TKey[] keySlots,
    TValue[] valueSlots,
    int touchedSlots,
    int count) : IReadOnlyDictionary<TKey, TValue>
    where TKey : unmanaged
    where TValue : unmanaged
{
    private const int NoSlot = -1;
    private const int HashFlag = int.MinValue;

    private readonly int[] _table = table;
    private readonly Link[] _linkSlots = linkSlots;

    private readonly TKey[] _keySlots = keySlots;
    private readonly TValue[] _valueSlots = valueSlots;

    private readonly int _touchedSlots = touchedSlots;

    public int Count { get; } = count;

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

    public unsafe bool ContainsKey(TKey key)
    {
        ThrowHelper.ThrowIfNull(key);

        return Unsafe.AsPointer(ref FindValue(key)) != null;
    }

    public unsafe bool TryGetValue(TKey key, out TValue value)
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

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private int GetKeyHashCode(TKey key)
    {
        return key.GetHashCode() | HashFlag;
    }

    private ref int GetSlot(int hashCode)
    {
        return ref _table[(hashCode & int.MaxValue) % _table.Length];
    }

    private unsafe ref TValue FindValue(TKey key)
    {
        ref TValue value = ref Unsafe.AsRef<TValue>(null);

        Link[] linkSlots = _linkSlots;

        int hashCode = GetKeyHashCode(key);
        int i = GetSlot(hashCode) - 1;

        while (i != NoSlot)
        {
            Link link = linkSlots[i];
            if (link.HashCode == hashCode
                && EqualityComparer<TKey>.Default.Equals(_keySlots[i], key))
            {
                value = ref _valueSlots[i];
                break;
            }

            i = link.Next;
        }

        return ref value;
    }
}
