using System.Collections;
using System.Collections.Generic;

using AslHelp.Shared;

namespace AslHelp.GameEngines.Unity.Collections;

internal sealed partial class Net20HashSet<T> : ISet<T>, IReadOnlyCollection<T>
    where T : unmanaged
{
    private const int NoSlot = -1;
    private const int HashFlag = int.MinValue;

    private readonly int[] _table;
    private readonly Link[] _links;

    private readonly T[] _slots;

    public Net20HashSet(int count, int[] table, Link[] links, T[] slots)
    {
        _table = table;
        _links = links;
        _slots = slots;

        Count = count;
    }

    public int Count { get; }
    bool ICollection<T>.IsReadOnly { get; } = true;

    public bool Add(T item)
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

    private int GetItemHashCode(T item)
    {
        return item.GetHashCode() | HashFlag;
    }

    private bool SlotsContainsAt(int index, int hash, T item)
    {
        int i = _table[index] - 1;
        while (i > NoSlot)
        {
            Link link = _links[i];
            if (link.HashCode != hash)
            {
                continue;
            }

            if (Equals(_slots[i], item))
            {
                return true;
            }

            i = link.Next;
        }

        return false;
    }

    public bool Contains(T item)
    {
        int hashCode = GetItemHashCode(item);
        int index = (hashCode & int.MaxValue) % _table.Length;

        return SlotsContainsAt(index, hashCode, item);
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

        for (int i = 0; i < count; i++)
        {
            if (GetLinkHashCode(i) != 0)
            {
                array[arrayIndex++] = _slots[i];
            }
        }
    }

    private int GetLinkHashCode(int index)
    {
        return _links[index].HashCode & HashFlag;
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {

    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {

    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {

    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {

    }

    public bool Overlaps(IEnumerable<T> other)
    {

    }

    public bool Remove(T item)
    {
        const string Msg = "The collection is read-only.";
        ThrowHelper.ThrowNotSupportedException(Msg);

        return false;
    }

    public bool SetEquals(IEnumerable<T> other)
    {

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

    void ICollection<T>.Add(T item)
    {
        Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
