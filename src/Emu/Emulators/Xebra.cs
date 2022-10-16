namespace AslHelp.Emulators.Managers;

internal class Xebra : EmuMemManager
{
    public Xebra()
        : base()
    {
        _target.Add(0x1, "E8 ???????? E9 ???????? 89 C8 C1 F8 10");
        _target.OnFound = (_, addr) => addr + 0x4 + _emu.Read<int>(addr);
    }

    public override string Name => "Xebra";
    public override string Core => null;

    protected override Task<nint> GetWramAsync()
    {
        nint wram = _emu.Read<nint>(_emu.Scan(_target) + 0x16A);
        return Task.FromResult<nint>(wram);
    }

    public override bool VerifyWRAM()
    {
        return true;
    }
}
