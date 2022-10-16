using AslHelp.Data;
using AslHelp.MemUtils;
using AslHelp.MemUtils.SigScan;

namespace AslHelp.Emulators.Managers;

public abstract partial class EmuMemManager
{
    protected readonly Emu _emu = Emu.Instance;
    protected readonly ScanTarget _target = new() { OnFound = Memory.OnFound };

    protected (bool Check, int Value) _exactPageSize;
    protected (bool Check, MemPageType Value) _exactPageType;

    internal EmuMemManager()
    {
        _exactPageSize = (false, default);
        _exactPageType = (false, default);
    }

    internal EmuMemManager(int exactPageSize)
    {
        _exactPageSize = (true, exactPageSize);
        _exactPageType = (false, default);
    }

    internal EmuMemManager(MemPageType exactPageType)
    {
        _exactPageSize = (false, default);
        _exactPageType = (true, exactPageType);
    }

    internal EmuMemManager(int exactPageSize, MemPageType exactPageType)
    {
        _exactPageSize = (true, exactPageSize);
        _exactPageType = (true, exactPageType);
    }

    protected abstract Task<nint> GetWramAsync();

    protected IEnumerable<MemoryPage> GetFilteredPages(bool allPages = true)
    {
        (bool checkPageSize, int pageSize) = _exactPageSize;
        (bool checkPageType, MemPageType pageType) = _exactPageType;

        foreach (MemoryPage page in _emu.Pages)
        {
            if (checkPageSize && page.RegionSize != pageSize)
            {
                continue;
            }

            if (!allPages && page.Protect.HasFlag(MemPageProtect.PAGE_GUARD))
            {
                continue;
            }

            if (checkPageType)
            {
                if (page.Type != pageType)
                {
                    continue;
                }
            }
            else if (!allPages && page.Type != MemPageType.MEM_PRIVATE)
            {
                continue;
            }

            yield return page;
        }
    }

    internal async Task<bool> TryFindWram()
    {
        if (Core is null)
        {
            Debug.Info($"Searching for WRAM for {Name}...");
        }
        else
        {
            Debug.Info($"Searching for WRAM for {Name} ({Core})...");
        }

        int wramLoadTimeout = (int)_emu.WRAMLoadTimeout;

        while (true)
        {
            if (_emu.LoadCanceled || _emu.Game is null || _emu.Game.HasExited)
            {
                break;
            }

            WRAM = await GetWramAsync();

            if (WRAM != 0)
            {
                Debug.Info($"  => Found at 0x" + WRAM.ToString("X") + ".");
                Debug.Info();

                return true;
            }

            Debug.Info($"  => Not found. Retrying in {wramLoadTimeout}ms...");
            await Task.Delay(wramLoadTimeout, _emu.CancelToken);
        }

        Debug.Warn($"  => Could not determine WRAM address for emulator.");
        return false;
    }
}
