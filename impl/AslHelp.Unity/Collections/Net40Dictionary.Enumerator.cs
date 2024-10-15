using System.Collections;
using System.Collections.Generic;

namespace AslHelp.Unity.Collections;

internal partial class Net40Dictionary<TKey, TValue>
{
    public struct Entry
    {
        public int HashCode;
        public int Next;
        public TKey Key;
        public TValue Value;
    }

    private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator
    {
        private readonly Net40Dictionary<TKey, TValue> _dictionary;

        private int _next;

        public Enumerator(Net40Dictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public KeyValuePair<TKey, TValue> Current { get; private set; }
        readonly object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            uint next = (uint)_next, count = (uint)_dictionary.Count;

            // Use unsigned comparison, since we set `index` to `_dictionary.Count + 1` when the enumeration ends.
            // `_dictionary.Count + 1` could be negative if `_dictionary.Count` is `int.MaxValue`.
            while (next < count)
            {
                ref Entry entry = ref _dictionary._entries[next++];
                if (entry.Next >= -1)
                {
                    Current = new(entry.Key, entry.Value);
                    _next = (int)next;

                    return true;
                }
            }

            _next = (int)count + 1;
            Current = default;

            return false;
        }

        public void Reset()
        {
            _next = 0;
            Current = default;
        }

        public readonly void Dispose()
        { }
    }
}
