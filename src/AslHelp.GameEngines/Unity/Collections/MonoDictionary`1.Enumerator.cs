using System.Collections;
using System.Collections.Generic;

namespace AslHelp.GameEngines.Unity.Collections;

internal partial class MonoDictionary<TValue>
{
    private struct Enumerator : IEnumerator<KeyValuePair<string, TValue>>, IEnumerator
    {
        private readonly MonoDictionary<TValue> _dictionary;

        private int _next;

        public Enumerator(MonoDictionary<TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public KeyValuePair<string, TValue> Current { get; private set; }
        readonly object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            int next = _next, touched = _dictionary._touchedSlots;

            if (next < 0)
            {
                return false;
            }

            while (next < touched)
            {
                if ((_dictionary._linkSlots[next].HashCode & HashFlag) != 0)
                {
                    Current = new(_dictionary.GetKey(next), _dictionary._valueSlots[next]);
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
