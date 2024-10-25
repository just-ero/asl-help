using System.Collections;
using System.Collections.Generic;

using AslHelp.Shared;

namespace AslHelp.GameEngines.Unity.Collections;

internal sealed partial class MonoHashSet<T>(
    int[] table,
    Link[] links,
    T[] slots,
    int touched,
    int count) : ISet<T>, IReadOnlyCollection<T>
    where T : unmanaged
{
    private const int NoSlot = -1;
    private const int HashFlag = int.MinValue;

    private readonly int[] _table = table;
    private readonly Link[] _links = links;

    private readonly T[] _slots = slots;

    private readonly int _touched = touched;

    public int Count { get; } = count;
    public bool IsReadOnly { get; } = true;

    public bool Contains(T item)
    {
        return FindItem(item);
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
        ThrowHelper.ThrowIfNull(array);
        ThrowHelper.ThrowIfNotInRange(arrayIndex, 0, array.Length);
        ThrowHelper.ThrowIfNotInRange(count, 0, Count);

        for (int i = 0; i < _touched; i++)
        {
            if (GetLinkHashCode(i) != 0)
            {
                array[arrayIndex++] = _slots[i];
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

        HashSet<T> otherSet = ToSet(other);
        if (Count > otherSet.Count)
        {
            return false;
        }

        return CheckIsSubsetOf(otherSet);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        ThrowHelper.ThrowIfNull(other);

        if (Count == 0)
        {
            return true;
        }

        HashSet<T> otherSet = ToSet(other);
        if (Count >= otherSet.Count)
        {
            return false;
        }

        return CheckIsSubsetOf(otherSet);
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        ThrowHelper.ThrowIfNull(other);

        HashSet<T> otherSet = ToSet(other);
        if (Count < otherSet.Count)
        {
            return false;
        }

        return CheckIsSupersetOf(otherSet);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        ThrowHelper.ThrowIfNull(other);

        HashSet<T> otherSet = ToSet(other);
        if (Count <= otherSet.Count)
        {
            return false;
        }

        return CheckIsSupersetOf(otherSet);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        ThrowHelper.ThrowIfNull(other);

        foreach (T item in other)
        {
            if (Contains(item))
            {
                return true;
            }
        }

        return false;
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        ThrowHelper.ThrowIfNull(other);

        HashSet<T> otherSet = ToSet(other);
        if (Count != otherSet.Count)
        {
            return false;
        }

        return CheckIsSupersetOf(otherSet);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    public bool Add(T item)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);

        return false;
    }

    public bool Remove(T item)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);

        return false;
    }

    public void Clear()
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }

    public void UnionWith(IEnumerable<T> other)
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
        Add(item);
    }

    private HashSet<T> ToSet(IEnumerable<T> source)
    {
        if (source is not HashSet<T> set
            || set.Comparer != EqualityComparer<T>.Default)
        {
            set = new(source, EqualityComparer<T>.Default);
        }

        return set;
    }

    private bool CheckIsSubsetOf(HashSet<T> other)
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

    private bool CheckIsSupersetOf(HashSet<T> other)
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

    private int GetLinkHashCode(int index)
    {
        return _links[index].HashCode & HashFlag;
    }

    private int GetItemHashCode(T item)
    {
        return item.GetHashCode() | HashFlag;
    }

    private ref int GetSlot(int hashCode)
    {
        return ref _table[(hashCode & int.MaxValue) % _table.Length];
    }

    private bool FindItem(T item)
    {
        int hashCode = GetItemHashCode(item);
        int i = GetSlot(hashCode) - 1;

        while (i != NoSlot)
        {
            Link link = _links[i];
            if (link.HashCode == hashCode
                && EqualityComparer<T>.Default.Equals(_slots[i], item))
            {
                return true;
            }

            i = link.Next;
        }

        return false;
    }
}
