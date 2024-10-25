using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AslHelp.Shared;

namespace AslHelp.GameEngines.Unity.Collections;

internal sealed partial class NetFxHashSet<T>(
    int[] buckets,
    NetFxHashSet<T>.Slot[] slots,
    int count,
    int lastIndex) : ISet<T>, IReadOnlyCollection<T>
    where T : unmanaged
{
    private const int Lower31BitMask = 0x7FFFFFFF;

    private readonly int[] _buckets = buckets;
    private readonly Slot[] _slots = slots;

    private readonly int _lastIndex = lastIndex;

    public int Count { get; } = count;
    public bool IsReadOnly { get; } = true;

    public bool Contains(T item)
    {
        return InternalIndexOf(item) >= 0;
    }

    public void CopyTo(T[] array)
    {
        CopyTo(array, 0, Count);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        CopyTo(array, arrayIndex, Count);
    }

    public void CopyTo(T[] array, int arrayIndex, int count)
    {
        ThrowHelper.ThrowIfNotInRange(arrayIndex, 0, array.Length);
        ThrowHelper.ThrowIfNotInRange(count, 0, Count);

        int numCopied = 0;
        for (int i = 0; i < _lastIndex && numCopied < Count; i++)
        {
            if (_slots[i].HashCode >= 0)
            {
                array[arrayIndex + numCopied] = _slots[i].Value;
                numCopied++;
            }
        }
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        ThrowHelper.ThrowIfNull(other);

        if (Count == 0)
        {
            return true;
        }

        if (other is HashSet<T> otherSet && otherSet.Comparer == EqualityComparer<T>.Default)
        {
            if (Count > otherSet.Count)
            {
                return false;
            }

            return IsSubsetOfHasSetWithSameEC(otherSet);
        }

        ElementCount result = CheckUniqueAndUnfoundElements(other, false);
        return result.UniqueCount == Count && result.UnfoundCount >= 0;
    }

    private bool IsSubsetOfHasSetWithSameEC(HashSet<T> other)
    {
        foreach (T item in this)
        {
            if (!other.Contains(item))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        ThrowHelper.ThrowIfNull(other);

        if (other is ICollection<T> { Count: > 0 })
        {
            return Count == 0;
        }

        if (other is HashSet<T> otherSet && otherSet.Comparer == EqualityComparer<T>.Default)
        {
            if (Count >= otherSet.Count)
            {
                return false;
            }

            return IsSubsetOfHasSetWithSameEC(otherSet);
        }

        ElementCount result = CheckUniqueAndUnfoundElements(other, false);
        return result.UniqueCount == Count && result.UnfoundCount > 0;
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        ThrowHelper.ThrowIfNull(other);

        if (other is ICollection<T> { Count: 0 })
        {
            return true;
        }

        if (other is HashSet<T> otherSet && otherSet.Comparer == EqualityComparer<T>.Default)
        {
            if (Count < otherSet.Count)
            {
                return false;
            }
        }

        return ContainsAllElements(other);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        ThrowHelper.ThrowIfNull(other);

        if (Count == 0)
        {
            return false;
        }

        if (other is ICollection<T> { Count: 0 })
        {
            return true;
        }

        if (other is HashSet<T> otherSet && otherSet.Comparer == EqualityComparer<T>.Default)
        {
            if (Count <= otherSet.Count)
            {
                return false;
            }

            return ContainsAllElements(otherSet);
        }

        ElementCount result = CheckUniqueAndUnfoundElements(other, false);
        return result.UniqueCount < Count && result.UnfoundCount == 0;
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        ThrowHelper.ThrowIfNull(other);

        if (Count == 0)
        {
            return false;
        }

        foreach (T element in other)
        {
            if (Contains(element))
            {
                return true;
            }
        }

        return false;
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        ThrowHelper.ThrowIfNull(other);

        if (other is HashSet<T> otherSet && otherSet.Comparer == EqualityComparer<T>.Default)
        {
            if (Count != otherSet.Count)
            {
                return false;
            }

            return ContainsAllElements(otherSet);
        }

        if (other is ICollection<T> otherCollection && Count == 0 && otherCollection.Count > 0)
        {
            return false;
        }

        ElementCount result = CheckUniqueAndUnfoundElements(other, true);
        return result.UniqueCount == Count && result.UnfoundCount == 0;
    }

    private unsafe ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
    {
        ElementCount result = default;

        if (Count == 0)
        {
            result.UniqueCount = 0;
            result.UnfoundCount = other.Any() ? 1 : 0;

            return result;
        }

        foreach (T item in other)
        {
            int index = InternalIndexOf(item);
            if (index >= 0)
            {
                result.UniqueCount++;
            }
            else
            {
                result.UnfoundCount++;
                if (returnIfUnfound)
                {
                    break;
                }
            }
        }

        return result;
    }

    private int InternalIndexOf(T item)
    {
        int hashCode = InternalGetHashCode(item);
        for (int i = _buckets[hashCode % _buckets.Length] - 1; i >= 0; i = _slots[i].Next)
        {
            if (_slots[i].HashCode == hashCode && EqualityComparer<T>.Default.Equals(_slots[i].Value, item))
            {
                return i;
            }
        }

        return -1;
    }

    private int InternalGetHashCode(T item)
    {
        return EqualityComparer<T>.Default.GetHashCode(item) & Lower31BitMask;
    }

    private bool ContainsAllElements(IEnumerable<T> other)
    {
        foreach (T item in other)
        {
            if (!Contains(item))
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    bool ISet<T>.Add(T item)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);

        return false;
    }

    bool ICollection<T>.Remove(T item)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);

        return false;
    }

    void ICollection<T>.Clear()
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }

    void ISet<T>.ExceptWith(IEnumerable<T> other)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }

    void ISet<T>.IntersectWith(IEnumerable<T> other)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }

    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }

    void ISet<T>.UnionWith(IEnumerable<T> other)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    void ICollection<T>.Add(T item)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }
}
