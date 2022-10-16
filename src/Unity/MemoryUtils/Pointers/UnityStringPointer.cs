namespace AslHelp.MemUtils.Pointers;

public class UnityStringPointer : BasePointer<string>
{
    public UnityStringPointer(nint baseAddress, int[] offsets)
        : base(baseAddress, offsets) { }

    public override bool Write(string value)
    {
        throw new NotImplementedException();
    }

    protected override bool TryUpdate(out string result)
    {
        return Unity.Instance.TryReadString(out result, _base, _offsets);
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
