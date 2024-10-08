using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AslHelp.Collections;

public interface IKeyedCollection<TKey, TValue> : ICollection<TValue>, IReadOnlyCollection<TValue>
{
    TValue this[TKey key] { get; set; }

    ICollection<TKey> Keys { get; }
    ICollection<TValue> Values { get; }

    void Add(TKey key, TValue value);
    bool ContainsKey(TKey key);
    bool Remove(TKey key);
    bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
}
