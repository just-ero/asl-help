using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Shared;

namespace AslHelp.Collections;

/// <summary>
///     The <see cref="KeyedCollection{TKey, TValue}"/> class
///     provides an abstract interface for enumerable collections with an internal cache.<br/>
///     The cache is populated during enumeration and can be accessed using a key corresponding to the value.
/// </summary>
/// <typeparam name="TKey">The type of the keys for the <see cref="KeyedCollection{TKey, TValue}"/>.</typeparam>
/// <typeparam name="TValue">The type of the values in the <see cref="KeyedCollection{TKey, TValue}"/>.</typeparam>
public abstract class KeyedCollection<TKey, TValue> : IReadOnlyKeyedCollection<TKey, TValue>
    where TKey : notnull
{
    private readonly IEqualityComparer<TKey> _comparer;
    private readonly Dictionary<TKey, TValue> _cache;

    protected KeyedCollection()
        : this(EqualityComparer<TKey>.Default) { }

    protected KeyedCollection(IEqualityComparer<TKey> comparer)
    {
        _comparer = comparer;
        _cache = new(comparer);
    }

    public int Count => _cache.Count;

    public IReadOnlyCollection<TKey> Keys => _cache.Keys;
    public IReadOnlyCollection<TValue> Values => _cache.Values;

    public TValue this[TKey key]
    {
        get
        {
            if (!TryGetValue(key, out TValue? value))
            {
                string msg = KeyNotFoundMessage(key);
                ThrowHelper.ThrowKeyNotFoundException(msg);
            }

            return value;
        }
    }

    public bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out _);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_cache.TryGetValue(key, out value))
        {
            return true;
        }

        lock (_cache)
        {
            OnSearch(key);

            foreach (TValue item in this)
            {
                if (_comparer.Equals(key, GetKey(item)))
                {
                    value = item;
                    _cache.Add(key, value);

                    OnFound(value);

                    return true;
                }
            }
        }

        value = default;

        OnNotFound(key);

        return false;
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public abstract IEnumerator<TValue> GetEnumerator();
    protected abstract TKey GetKey(TValue value);

    protected virtual void OnSearch(TKey key) { }
    protected virtual void OnFound(TValue value) { }
    protected virtual void OnNotFound(TKey key) { }

    protected virtual string KeyNotFoundMessage(TKey key)
    {
        return $"The given key '{key}' was not present in the cached collection.";
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
