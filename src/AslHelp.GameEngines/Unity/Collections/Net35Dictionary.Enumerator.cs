using System.Collections;
using System.Collections.Generic;

namespace AslHelp.Unity.Collections;

internal partial class Net35Dictionary<TKey, TValue>
{
    private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator
    {
        private readonly Net35Dictionary<TKey, TValue> _dictionary;

        private int _next;

        public Enumerator(Net35Dictionary<TKey, TValue> dictionary)
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
                ref Link link = ref _dictionary._linkSlots[next++];
                if (((uint)link.HashCode & 0x80000000) != 0)
                {
                    Current = new(_dictionary._keySlots[next], _dictionary._valueSlots[next]);
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
