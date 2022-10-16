using LiveSplit.ComponentUtil;

namespace AslHelp.MemUtils.Pointers;

public class StringPointer : BasePointer<string>
{
    private readonly int _length;
    private readonly ReadStringType _stringType;

    public StringPointer(int length, ReadStringType stringType, nint baseAddress, params int[] offsets)
        : base(baseAddress, offsets)
    {
        _length = length;
        _stringType = stringType;
    }

    public override bool Write(string value)
    {
        throw new NotImplementedException();
    }

    protected override bool TryUpdate(out string result)
    {
        return Basic.Instance.TryReadString(out result, _length, _stringType, _base, _offsets);
    }

    protected override bool HasChanged(string old, string current)
    {
        return old != current;
    }

    public override string ToString()
    {
        return $"StringPointer({OffsetsToString()})";
    }
}
