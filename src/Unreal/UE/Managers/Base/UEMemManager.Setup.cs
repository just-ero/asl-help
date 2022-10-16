using AslHelp.MemUtils.Exceptions;
using AslHelp.MemUtils.SigScan;

namespace AslHelp.UE.Managers;

public abstract partial class UEMemManager
{
    private static readonly Signature _gWorldTrg =
        Unreal.Instance.Is64Bit
        ? new(3, "48 8B 05 ???????? 48 3B C? 48 0F 44 C? 48 89 05 ???????? E8")
        : new(1, "A1 ???????? 33 C9 3B C6 0F 44 C1 A3");

    private static readonly Signature _gEngineTrg =
        Unreal.Instance.Is64Bit
        ? new(7, "A8 01 75 ?? 48 C7 05")
        : new(6, "A8 01 75 ?? C7 05");

    private static readonly Signature _gSyncLoadCountTrg =
        Unreal.Instance.Is64Bit
        ? new(21, "33 C0 0F 57 C0 F2 0F 11 05")
        : new(23, "0F 57 C0 C7 05 ???????? 00 00 00 00 F2 0F 11 05");

    private protected nint _fNamePool = -1;
    private protected nint _gUObjectArray = -1;

    private bool Scan(int major, int minor)
    {
        _fNamePool = _game.Scan(_fNamesTrg);
        if (_fNamePool == default)
        {
            if (_game.MainModule.Symbols.TryGetValue("FNamePool", out DebugSymbol symbol))
            {
                _fNamePool = symbol.Address;
            }
            else
            {
                throw new FatalNotFoundException("FNamePool could not be found.");
            }
        }

        Debug.Info($"FNamePool: 0x{_fNamePool.ToString("X")}");

        _gUObjectArray = _game.Scan(_uObjectsTrg);
        if (_gUObjectArray == default)
        {
            if (_game.MainModule.Symbols.TryGetValue("GUObjectArray", out DebugSymbol symbol))
            {
                _gUObjectArray = symbol.Address;
            }
            else
            {
                throw new FatalNotFoundException("GUObjectArray could not be found.");
            }
        }

        Debug.Info($"GUObjectArray: 0x{_gUObjectArray.ToString("X")}");

        GWorld = _game.ScanRel(_gWorldTrg);
        if (GWorld == default)
        {
            if (_game.MainModule.Symbols.TryGetValue("GWorld", out DebugSymbol symbol))
            {
                GWorld = symbol.Address;
            }
        }

        Debug.Info($"GWorld: 0x{GWorld.ToString("X")}");

        GEngine = _game.ScanRel(_gEngineTrg);
        if (GEngine == default)
        {
            if (_game.MainModule.Symbols.TryGetValue("GEngine", out DebugSymbol symbol))
            {
                GEngine = symbol.Address;
            }
        }

        Debug.Info($"GEngine: 0x{GEngine.ToString("X")}");

        if ((major, minor) is (4, >= 20) or (5, _))
        {
            GSyncLoadCount = _game.ScanRel(_gSyncLoadCountTrg);
            if (GSyncLoadCount == default)
            {
                if (_game.MainModule.Symbols.TryGetValue("GSyncLoadCount", out DebugSymbol symbol))
                {
                    GSyncLoadCount = symbol.Address;
                }
            }

            Debug.Info($"GSyncLoadCount: 0x{GSyncLoadCount.ToString("X")}");
        }

        return true;
    }

    protected abstract ScanTarget _fNamesTrg { get; }
    protected abstract ScanTarget _uObjectsTrg { get; }
}
