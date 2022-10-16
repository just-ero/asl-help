namespace AslHelp.Emulators.Managers;

internal class ePSXe : EmuMemManager
{
    public ePSXe()
        : base()
    {
        _target.Add(5, "C1 E1 10 8D 89");
    }

    public override string Name => "ePSXe";
    public override string Core => null;

    protected override async Task<nint> GetWramAsync()
    {
        await Task.Delay(3000, _emu.CancelToken);

        return _emu.Scan(_target);
    }

    public override bool VerifyWRAM()
    {
        return true;
    }
}
