using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AslHelp.Collections;

public interface IReadOnlyKeyedCollection<TKey, TValue> : IReadOnlyCollection<TValue>
{
    TValue this[TKey key] { get; }

    IReadOnlyCollection<TKey> Keys { get; }
    IReadOnlyCollection<TValue> Values { get; }

    bool ContainsKey(TKey key);
    bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
}
