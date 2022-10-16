namespace AslHelp.MemUtils.Pointers;

public class UEStringPointer : BasePointer<string>
{
    private readonly int _length;

    public UEStringPointer(int length, nint baseAddress, params int[] offsets)
        : base(baseAddress, offsets)
    {
        _length = length;
    }

    public override bool Write(string value)
    {
        throw new NotImplementedException();
    }

    protected override bool TryUpdate(out string result)
    {
        return Unreal.Instance.TryReadString(out result, _length, _base, _offsets);
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
