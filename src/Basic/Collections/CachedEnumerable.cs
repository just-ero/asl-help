namespace AslHelp.Collections;

public abstract class CachedEnumerable<TKey, TValue> : IEnumerable<TValue> where TKey : notnull
{
    protected readonly Dictionary<TKey, TValue> _cache = new();

    public int Count => _cache.Count;

    public abstract IEnumerator<TValue> GetEnumerator();
    protected abstract TKey GetKey(TValue value);

    protected virtual void OnSearch(TKey key) { }
    protected virtual void OnFound(TValue value) { }
    protected virtual void OnNotFound(TKey key) { }

    protected virtual bool CompareKeys(TKey searchedKey, TKey itemKey)
    {
        return searchedKey.Equals(itemKey);
    }

    protected virtual string KeyNotFoundMessage(TKey key)
    {
        return $"The given key '{key}' was not present in the cached collection.";
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException(KeyNotFoundMessage(key));
            }
        }
        protected set => _cache[key] = value;
    }

    public bool TryGetValue(TKey key, out TValue value)
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
                value = item;
                TKey itemKey = GetKey(value);

                _cache[itemKey] = value;

                if (CompareKeys(key, itemKey))
                {
                    OnFound(value);

                    return true;
                }
            }
        }

        OnNotFound(key);

        value = default;
        return false;
    }

    public void Clear()
    {
        lock (_cache)
        {
            _cache.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
