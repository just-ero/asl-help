namespace ASLHelper;

public partial class Main
{
    #region String Pattern
    public nint Scan(int offset, params string[] pattern)
    {
        var module = Game?.MainModuleWow64Safe();
        return Scan(module, offset, pattern);
    }

    public nint Scan(string module, params string[] pattern)
    {
        return Scan(GetModule(module), 0, pattern);
    }

    public nint Scan(string module, int offset, params string[] pattern)
    {
        return Scan(GetModule(module), offset, pattern);
    }

    public nint Scan(ProcessModuleWow64Safe module, params string[] pattern)
    {
        return Scan(module, 0, pattern);
    }

    public nint Scan(ProcessModuleWow64Safe module, int offset, params string[] pattern)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Could not find module!");
            return 0;
        }

        var start = module.BaseAddress;
        var size = module.ModuleMemorySize;

        return Scan(start, size, offset, pattern);
    }

    public nint Scan(nint startAddress, nint endAddress, params string[] pattern)
    {
        var start = startAddress;
        var size = (int)(endAddress - (long)startAddress);

        return Scan(start, size, 0, pattern);
    }

    public nint Scan(nint startAddress, nint endAddress, int offset, params string[] pattern)
    {
        var start = startAddress;
        var size = (int)(endAddress - (long)startAddress);

        return Scan(start, size, offset, pattern);
    }

    public nint Scan(nint startAddress, int size, params string[] pattern)
    {
        return Scan(startAddress, size, 0, pattern);
    }

    public nint Scan(nint startAddress, int size, int offset, params string[] pattern)
    {
        return ScanAll(startAddress, size, offset, pattern).FirstOrDefault();
    }
    #endregion

    #region SigScanTarget
    public nint Scan(SigScanTarget target)
    {
        var module = Game?.MainModuleWow64Safe();
        return Scan(module, target);
    }

    public nint Scan(string module, SigScanTarget target)
    {
        return Scan(GetModule(module), target);
    }

    public nint Scan(ProcessModuleWow64Safe module, SigScanTarget target)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Module could not be found!");
            return 0;
        }

        var start = module.BaseAddress;
        var size = module.ModuleMemorySize;

        return Scan(start, size, target);
    }

    public nint Scan(nint startAddress, nint endAddress, SigScanTarget target)
    {
        var start = startAddress;
        var size = (int)(endAddress - (long)startAddress);

        return Scan(start, size, target);
    }

    public nint Scan(nint startAddress, int size, SigScanTarget target)
    {
        return ScanAll(startAddress, size, target).FirstOrDefault();
    }
    #endregion
}
