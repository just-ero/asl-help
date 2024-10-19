using System.Collections;
using System.Collections.Generic;

namespace AslHelp.GameEngines.Unity.Collections;

#error Use Mono impl.
internal partial class MonoDictionary<TKey, TValue>
{
    private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator
    {
        private readonly MonoDictionary<TKey, TValue> _dictionary;

        private int _next;

        public Enumerator(MonoDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public KeyValuePair<TKey, TValue> Current { get; private set; }
        readonly object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            int next = _next, count = _dictionary.Count;

            if (next < 0)
            {
                return false;
            }

            while (next < count)
            {
                if ((_dictionary._linkSlots[next].HashCode & HashFlag) != 0)
                {
                    Current = new(_dictionary._keySlots[next], _dictionary._valueSlots[next]);
                    _next = next + 1;

                    return true;
                }

                next++;
            }

            _next = NoSlot;
            Current = default;

            return false;
        }

        public void Reset()
        {
            _next = 0;
            Current = default;
        }

        public readonly void Dispose() { }
    }
}
