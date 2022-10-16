using AslHelp;
using AslHelp.UE.Managers;

public sealed partial class Unreal : HelperBase<UEMemManager>
{
    internal static new Unreal Instance { get; private set; }

    public Unreal()
        : this(true) { }

    public Unreal(bool generateCode)
        : base(generateCode)
    {
        Instance = this;
    }

    protected override Task<bool> LoadAsync()
    {
        return Task.FromResult<bool>(true);
    }

    public override void Dispose(bool removeTexts)
    {
        _ueVersion = default;

        base.Dispose(removeTexts);
    }
}
