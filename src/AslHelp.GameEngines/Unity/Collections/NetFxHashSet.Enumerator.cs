using System;
using System.Collections;
using System.Collections.Generic;

namespace AslHelp.GameEngines.Unity.Collections;

internal partial class NetFxHashSet
{
    public struct Slot
    {
        public int HashCode;
        public int Next;
        public nint Value;
    }

    private struct ElementCount
    {
        public int UniqueCount;
        public int UnfoundCount;
    }

    private struct Enumerator : IEnumerator<string?>, IEnumerator
    {
        private readonly NetFxHashSet _set;

        private int _next;

        public Enumerator(NetFxHashSet set)
        {
            _set = set;
        }

        public string? Current { get; private set; }
        readonly object? IEnumerator.Current => Current;

        public bool MoveNext()
        {
            int next = _next;

            while (next < _set._lastIndex)
            {
                NetFxHashSet<nint>.Slot slot = _set._slots[next];
                if (slot.HashCode >= 0)
                {
                    Current = _set.GetSlotValue(next, slot);
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
