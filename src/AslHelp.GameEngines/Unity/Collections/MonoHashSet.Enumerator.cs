using System.Collections;
using System.Collections.Generic;

namespace AslHelp.GameEngines.Unity.Collections;

internal sealed partial class MonoHashSet<T>
{
    private struct Enumerator : IEnumerator<T>, IEnumerator
    {
        private readonly MonoHashSet<T> _set;

        private int _next;

        public Enumerator(MonoHashSet<T> set)
        {
            _set = set;
        }

        public T Current { get; private set; }
        readonly object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            int next = _next, touched = _set._touched;

            if (next < 0)
            {
                return false;
            }

            while (next < touched)
            {
                if (_set.GetLinkHashCode(next) != 0)
                {
                    Current = _set._slots[next];
                    _next = next + 1;

                    return true;
                }

                next++;
            }

            _next = NoSlot;
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
