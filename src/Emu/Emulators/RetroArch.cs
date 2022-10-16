using AslHelp.MemUtils;

namespace AslHelp.Emulators.Managers;

internal class RetroArch : EmuMemManager
{
    private readonly nint _base;
    private IEnumerable<string> _lowerModuleNames => _emu.Modules.Select(m => m.Name.ToLower());

    public RetroArch()
        : base(MemPageType.MEM_MAPPED)
    {
        if (Core is null)
        {
            Debug.Warn("[RetroArch] Could not determine core module. Unable to load WRAM address.");

            _base = default;
            return;
        }

        _base = _emu.Modules[Core].Base;

        _exactPageSize = Core switch
        {
            "mednafen_psx_hw_libretro.dll" or "mednafen_psx_libretro.dll" => (true, 0x200000),
            "pcsx_rearmed_libretro.dll" => (true, 0x210000),
            _ => (false, default)
        };
    }

    public override string Name => "RetroArch";
    public override string Core => _lowerModuleNames.FirstOrDefault(name => name.EndsWith("libretro.dll"));

    protected override Task<nint> GetWramAsync()
    {
        nint wram = Core switch
        {
            null => 0,
            _ => GetFilteredPages().FirstOrDefault().Base
        };

        return Task.FromResult<nint>(wram);
    }

    public override bool VerifyWRAM()
    {
        if (Core is null)
        {
            return false;
        }

        return _emu.TryRead<byte>(out _, _base);
    }
}
