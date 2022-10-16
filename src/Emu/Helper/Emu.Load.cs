using AslHelp.Emulators.Managers;
using AslHelp.Tasks;

public partial class Emu
{
    protected override EmuMemManager MakeManager()
    {
        EmuMemManager mgr = Game.ProcessName.ToLower() switch
        {
            "psxfin" => new pSX(),
            "epsxe" => new ePSXe(),
            "xebra" => new Xebra(),
            "retroarch" => new RetroArch(),
            "pcsx-redux.main" => new PCSXRedux(),
            "snes9x" or "snes9x-x64" => new Snes9x(),
            "duckstation-qt-x64-releaseltcg" or "duckstation-nogui-x64-releaseltcg" => new DuckStation(),
            _ => throw new NotSupportedException("This emulator does not appear to be supported.")
        };

        mgr.WRAM =

        return mgr;
    }
}
