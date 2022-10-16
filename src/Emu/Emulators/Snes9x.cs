namespace AslHelp.Emulators.Managers;

internal class Snes9x : EmuMemManager
{
    public Snes9x()
        : base()
    {
        if (_emu.Is64Bit)
        {
            _target.Add(4, "49 8B 94 C0 ???????? F7 C1");
            _target.OnFound = (_, addr) => _emu.MainModule.Base + _emu.Read<int>(addr);
        }
        else
        {
            _target.Add(1, "E8 ???????? A2 ???????? 0F B6");
            _target.OnFound = (_, addr) => _emu.Read<nint>(addr + 0x4 + _emu.Read<int>(addr) + 0x17);
        }
    }

    public override string Name => "Snes9x";
    public override string Core => null;

    protected override Task<nint> GetWramAsync()
    {
        nint wram = _emu.Read<nint>(_emu.Scan(_target));
        return Task.FromResult<nint>(wram);
    }

    public override bool VerifyWRAM()
    {
        return true;
    }
}
