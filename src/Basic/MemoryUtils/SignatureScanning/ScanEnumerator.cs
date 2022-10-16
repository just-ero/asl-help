namespace AslHelp.MemUtils.SigScan;

internal class ScanEnumerator
    : IEnumerator<int>,
    IEnumerable<int>
{
    private readonly byte[] _memory;
    private readonly byte[] _values;
    private readonly byte[] _masks;

    private readonly int _alignment;
    private readonly int _length;
    private readonly int _end;

    private int _next;

    public ScanEnumerator(byte[] memory, Signature signature, int alignment)
    {
        _memory = memory;
        (_length, _values, _masks) = signature.Bytes;
        _alignment = alignment;
        _end = memory.Length - _length;
    }

    public int Current { get; private set; }
    object IEnumerator.Current => Current;

    public unsafe bool MoveNext()
    {
        (int align, int length, int end, int next) = (_alignment, _length, _end, _next);

        fixed (byte* pMemory = _memory, pValues = _values, pMasks = _masks)
        {
            for (; next < end; next += align)
            {
                for (int i = 0; i < length; i++)
                {
                    (byte b, byte v, byte m) = (pMemory[next + i], pValues[i], pMasks[i]);

                    if (((b ^ v) & m) != 0)
                    {
                        goto Next;
                    }
                }

                Current = next;
                _next = next + _alignment;
                return true;

            Next:
                ;
            }
        }

        return false;
    }

    public void Reset()
    {
        _next = 0;
    }

    public IEnumerator<int> GetEnumerator()
    {
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this;
    }

    public void Dispose() { }
}
