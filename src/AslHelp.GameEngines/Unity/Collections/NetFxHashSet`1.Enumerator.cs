using System;
using System.Collections;
using System.Collections.Generic;

namespace AslHelp.GameEngines.Unity.Collections;

internal partial class NetFxHashSet<T>
{
    public struct Slot
    {
        public int HashCode;
        public int Next;
        public T Value;
    }

    private struct ElementCount
    {
        public int UniqueCount;
        public int UnfoundCount;
    }

    private struct Enumerator : IEnumerator<T>, IEnumerator
    {
        private readonly NetFxHashSet<T> _set;

        private int _next;

        public Enumerator(NetFxHashSet<T> set)
        {
            _set = set;
        }

        public T Current { get; private set; }
        readonly object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            int next = _next;

            while (next < _set._lastIndex)
            {
                if (_set._slots[next].HashCode >= 0)
                {
                    Current = _set._slots[next].Value;
                    _next = next + 1;

                    return true;
                }

                next++;
            }

            _next = _set._lastIndex + 1;
            Current = default;

            return false;
        }

        void IEnumerator.Reset()
        {
            _next = 0;
            Current = default;
        }

        readonly void IDisposable.Dispose() { }
    }
}
