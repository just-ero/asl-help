using System.Collections;
using System.Collections.Generic;

namespace AslHelp.GameEngines.Unity.Collections;

internal sealed partial class Net20HashSet<T>
{
    private struct Enumerator : IEnumerator<T>, IEnumerator
    {
        private readonly Net20HashSet<T> _set;

        private int _next;

        public Enumerator(Net20HashSet<T> set)
        {
            _set = set;
        }

        public T Current { get; private set; }
        readonly object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            int next = _next, count = _set.Count;

            if (next < 0)
            {
                return false;
            }

            while (next < count)
            {
                if (_set.GetLinkHashCode(next) != 0)
                {
                    Current = _set._slots[next];
                    _next = next;

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
