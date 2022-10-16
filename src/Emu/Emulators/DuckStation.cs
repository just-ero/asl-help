using AslHelp.MemUtils;

namespace AslHelp.Emulators.Managers;

internal class DuckStation : EmuMemManager
{
    public DuckStation()
        : base(0x200000, MemPageType.MEM_MAPPED) { }

    public override string Name => "DuckStation";
    public override string Core => null;

    protected override Task<nint> GetWramAsync()
    {
        nint wram = GetFilteredPages().FirstOrDefault().Base;
        return Task.FromResult<nint>(wram);
    }

    public override bool VerifyWRAM()
    {
        return _emu.TryRead<nint>(out _, WRAM);
    }
}
