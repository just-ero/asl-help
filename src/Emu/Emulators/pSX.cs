using AslHelp.MemUtils;

namespace AslHelp.Emulators.Managers;

internal class pSX : EmuMemManager
{
    public pSX()
        : base(0x201000, MemPageType.MEM_PRIVATE) { }

    public override string Name => "pSX";
    public override string Core => null;

    protected override Task<nint> GetWramAsync()
    {
        nint wramBase = GetFilteredPages().FirstOrDefault().Base;

        if (wramBase == 0)
        {
            return Task.FromResult<nint>(default);
        }

        return Task.FromResult<nint>(wramBase + 0x20);
    }

    public override bool VerifyWRAM()
    {
        return true;
    }
}
