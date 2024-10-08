using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using AslHelp.Shared;

namespace AslHelp.Collections;

public abstract class OrderedDictionary<TKey, TValue>
    : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IList<TValue>, IReadOnlyList<TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dict;
    private readonly Dictionary<TKey, int> _indices;
    private readonly List<TValue> _items = [];

    protected OrderedDictionary(IEqualityComparer<TKey>? comparer = null)
    {
        comparer ??= EqualityComparer<TKey>.Default;

        _dict = new(comparer);
        _indices = new(comparer);
    }

    public ICollection<TKey> Keys => _dict.Keys;
    public ICollection<TValue> Values => _items;

    public int Count => _items.Count;
    public bool IsReadOnly => false;

    public TValue this[TKey key]
    {
        get
        {
            if (!_dict.TryGetValue(key, out TValue value))
            {
                string msg = $"The given key '{key}' was not present in the dictionary.";
                ThrowHelper.ThrowKeyNotFoundException(msg);
            }

            return value;
        }
        set
        {
            _dict[key] = value;

            if (_indices.TryGetValue(key, out int index))
            {
                _items.RemoveAt(index);
                _items.Insert(index, value);
            }
            else
            {
                _indices[key] = _items.Count;
                _items.Add(value);
            }
        }
    }

    public TValue this[int index]
    {
        get => _items[index];
        set
        {
            _items[index] = value;

            TKey key = GetKeyForItem(value);
            _dict[key] = value;
            _indices[key] = index;
        }
    }

    protected abstract TKey GetKeyForItem(TValue item);

    public void Add(TValue item)
    {
        Add(GetKeyForItem(item), item);
    }

    public void Add(TKey key, TValue value)
    {
        _dict.Add(key, value);
        _indices.Add(key, _items.Count);
        _items.Add(value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Insert(int index, TValue item)
    {
        foreach (var kvp in _indices.Where(kvp => kvp.Value >= index))
        {
            _indices[kvp.Key] = kvp.Value + 1;
        }

        TKey key = GetKeyForItem(item);

        _dict.Add(key, item);
        _indices.Add(key, index);
        _items.Insert(index, item);
    }

    public bool Remove(TValue item)
    {
        return Remove(GetKeyForItem(item));
    }

    public bool Remove(TKey key)
    {
        if (!_indices.TryGetValue(key, out int index))
        {
            return false;
        }

        foreach (var kvp in _indices.Where(kvp => kvp.Value > index))
        {
            _indices[kvp.Key] = kvp.Value - 1;
        }

        _items.RemoveAt(index);
        return _dict.Remove(key) && _indices.Remove(key);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public void RemoveAt(int index)
    {
        TKey key = GetKeyForItem(_items[index]);

        foreach (var kvp in _indices.Where(kvp => kvp.Value > index))
        {
            _indices[kvp.Key] = kvp.Value - 1;
        }

        _dict.Remove(key);
        _indices.Remove(key);
        _items.RemoveAt(index);
    }

    public bool ContainsKey(TKey key)
    {
        return _dict.ContainsKey(key);
    }

    public bool Contains(TValue item)
    {
        return Contains(new KeyValuePair<TKey, TValue>(GetKeyForItem(item), item));
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dict.Contains(item);
    }

    public int IndexOf(TValue item)
    {
        return _indices[GetKeyForItem(item)];
    }

    public bool TryGetValue(TKey key, [UnscopedRef] out TValue value)
    {
        return _dict.TryGetValue(key, out value);
    }

    public void Clear()
    {
        _dict.Clear();
        _indices.Clear();
        _items.Clear();
    }

    public void CopyTo(TValue[] array, int arrayIndex)
    {
        if (arrayIndex < 0)
        {
            const string Msg = "Starting index must be equal to or greater than 0.";
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(arrayIndex), Msg);
        }

        for (int i = 0; i < Count; i++)
        {
            if (arrayIndex >= array.Length)
            {
                const string Msg = $"The number of elements in this collection is greater than the available space in '{nameof(array)}'.";
                ThrowHelper.ThrowArgumentException(nameof(array), Msg);
            }

            array[arrayIndex++] = _items[i];
        }
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (arrayIndex < 0)
        {
            const string Msg = "Starting index must be equal to or greater than 0.";
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(arrayIndex), Msg);
        }

        for (int i = 0; i < Count; i++)
        {
            if (arrayIndex >= array.Length)
            {
                const string Msg = $"The number of elements in this collection is greater than the available space in '{nameof(array)}'.";
                ThrowHelper.ThrowArgumentException(nameof(array), Msg);
            }

            TValue item = _items[i];
            array[arrayIndex++] = new(GetKeyForItem(item), item);
        }
    }

    public IEnumerator<TValue> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return _dict.GetEnumerator();
    }
}
