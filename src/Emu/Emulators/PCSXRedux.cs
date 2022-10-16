namespace AslHelp.Emulators.Managers;

internal class PCSXRedux : EmuMemManager
{
    private readonly int _initialPageCount;

    public PCSXRedux()
        : base(0x801000)
    {
        _initialPageCount = GetFilteredPages().Count();
    }

    public override string Name => "PCSX-Redux";
    public override string Core => null;

    protected override Task<nint> GetWramAsync()
    {
        nint wram = GetFilteredPages().LastOrDefault().Base + (_emu.Is64Bit ? 0x40 : 0x20);
        return Task.FromResult<nint>(wram);
    }

    public override bool VerifyWRAM()
    {
        return GetFilteredPages().Count() == _initialPageCount;
    }
}
