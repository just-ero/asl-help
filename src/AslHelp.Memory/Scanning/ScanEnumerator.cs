// Inspired by Reloaded.Memory.Sigscan, by Sewer56.
// Reloaded.Memory.Sigscan is licensed under LGPL-3.0.
// See: https://github.com/Reloaded-Project/Reloaded.Memory.SigScan

using System.Collections;
using System.Collections.Generic;

namespace AslHelp.Memory.Scanning;

internal sealed unsafe class ScanEnumerator : IEnumerable<uint>, IEnumerator<uint>
{
    private const int UNROLLS = 8;

    private readonly byte[] _data;
    private readonly ulong[] _values;
    private readonly ulong[]? _masks;

    private readonly int _length;
    private readonly int _end;

    private uint _cursor;

    public ScanEnumerator(byte[] data, ScanPattern pattern)
    {
        _data = data;
        _values = pattern.Values;
        _masks = pattern.Masks;

        _length = pattern.Values.Length;
        _end = data.Length - _length - UNROLLS;
    }

    public uint Current { get; private set; }

    public bool MoveNext()
    {
        int length = _length, end = _end;
        uint cursor = _cursor;

        fixed (byte* pData = _data)
        fixed (ulong* pValues = _values, pMasks = _masks)
        {
            ulong value0 = pValues[0], mask0 = pMasks[0];

            while (cursor < end)
            {
                if ((*(ulong*)(pData + cursor) & mask0) != value0)
                {
                    if ((*(ulong*)(pData + cursor + 1) & mask0) != value0)
                    {
                        if ((*(ulong*)(pData + cursor + 2) & mask0) != value0)
                        {
                            if ((*(ulong*)(pData + cursor + 3) & mask0) != value0)
                            {
                                if ((*(ulong*)(pData + cursor + 4) & mask0) != value0)
                                {
                                    if ((*(ulong*)(pData + cursor + 5) & mask0) != value0)
                                    {
                                        if ((*(ulong*)(pData + cursor + 6) & mask0) != value0)
                                        {
                                            if ((*(ulong*)(pData + cursor + 7) & mask0) != value0)
                                            {
                                                cursor += 8;
                                                goto Next;
                                            }
                                            else
                                            {
                                                cursor += 7;
                                            }
                                        }
                                        else
                                        {
                                            cursor += 6;
                                        }
                                    }
                                    else
                                    {
                                        cursor += 5;
                                    }
                                }
                                else
                                {
                                    cursor += 4;
                                }
                            }
                            else
                            {
                                cursor += 3;
                            }
                        }
                        else
                        {
                            cursor += 2;
                        }
                    }
                    else
                    {
                        cursor += 1;
                    }
                }

                if (length <= 1 || RemainingValuesMatch(cursor, length, pData, pValues, pMasks))
                {
                    Current = cursor;
                    _cursor = cursor + 1;

                    return true;
                }

                cursor += 1;

            Next:
                ;
            }

            return false;
        }
    }

    private static bool RemainingValuesMatch(uint cursor, int length, byte* pData, ulong* pValues, ulong* pMasks)
    {
        for (int i = 1; i < length; i++)
        {
            cursor += sizeof(ulong);

            if ((*(ulong*)(pData + cursor) & pMasks[i]) != pValues[i])
            {
                return false;
            }
        }

        return true;
    }

    public void Reset()
    {
        _cursor = 0;
    }

    public void Dispose() { }

    object IEnumerator.Current => Current;

    IEnumerator<uint> IEnumerable<uint>.GetEnumerator()
    {
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this;
    }
}
