namespace ASLHelper.UnityHelper;

public class StringWatcher : ReadWriteWatcher
{
    public StringWatcher(nint baseAddress, int staticFieldOffset, int[] offsets)
        : base(baseAddress, staticFieldOffset, offsets) { }

    public new string Old
    {
        get => (string)base.Old;
        set => base.Old = value;
    }

    public new string Current
    {
        get => (string)base.Current;
        set => base.Current = value;
    }

    public bool Write(string value)
    {
        throw new NotImplementedException();
    }

    private protected override bool Update_Internal()
    {
        if (!Unity.Instance.TryReadString(out var result, _baseAddress, _offsets))
        {
            if (FailAction == ReadFailAction.DontUpdate)
                return false;

            base.Current = null;
        }
        else
        {
            Current = result;
        }

        return true;
    }

    public override void Reset()
    {
        base.Old = null;
        base.Current = null;
        InitialUpdate = false;
    }
}
