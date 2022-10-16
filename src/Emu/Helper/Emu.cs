using AslHelp;
using AslHelp.Emulators.Managers;

public partial class Emu : HelperBase<EmuMemManager>
{
    internal static new Emu Instance { get; private set; }

    public Emu()
        : this(true) { }

    public Emu(bool generateCode)
        : base(generateCode)
    {
        Instance = this;
    }

    public uint WRAMLoadTimeout { get; set; } = 3000;

    protected override Task<bool> LoadAsync()
    {
        return Task.FromResult<bool>(true);
    }
}
