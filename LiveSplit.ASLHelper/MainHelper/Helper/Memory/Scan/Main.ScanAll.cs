namespace ASLHelper;

public partial class Main
{
    #region String Pattern
    public IEnumerable<nint> ScanAll(params string[] pattern)
    {
        var module = Game?.MainModuleWow64Safe();
        return ScanAll(module, 0, pattern);
    }

    public IEnumerable<nint> ScanAll(int offset, params string[] pattern)
    {
        var module = Game?.MainModuleWow64Safe();
        return ScanAll(module, offset, pattern);
    }

    public IEnumerable<nint> ScanAll(string module, params string[] pattern)
    {
        return ScanAll(GetModule(module), 0, pattern);
    }

    public IEnumerable<nint> ScanAll(string module, int offset, params string[] pattern)
    {
        return ScanAll(GetModule(module), offset, pattern);
    }

    public IEnumerable<nint> ScanAll(ProcessModuleWow64Safe module, params string[] pattern)
    {
        return ScanAll(module, 0, pattern);
    }

    public IEnumerable<nint> ScanAll(ProcessModuleWow64Safe module, int offset, params string[] pattern)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Module could not be found!");
            return Enumerable.Empty<nint>();
        }

        var start = module.BaseAddress;
        var size = module.ModuleMemorySize;

        return ScanAll(start, size, offset, pattern);
    }

    public IEnumerable<nint> ScanAll(nint startAddress, nint endAddress, params string[] pattern)
    {
        var start = startAddress;
        var size = (int)(endAddress - (long)startAddress);

        return ScanAll(start, size, 0, pattern);
    }

    public IEnumerable<nint> ScanAll(nint startAddress, nint endAddress, int offset, params string[] pattern)
    {
        var start = startAddress;
        var size = (int)(endAddress - (long)startAddress);

        return ScanAll(start, size, offset, pattern);
    }

    public IEnumerable<nint> ScanAll(nint startAddress, int size, params string[] pattern)
    {
        return ScanAll(startAddress, size, 0, pattern);
    }

    public IEnumerable<nint> ScanAll(nint startAddress, int size, int offset, params string[] pattern)
    {
        if (Game is null)
        {
            Debug.Warn("[Scan] Game process was null!");
            return Enumerable.Empty<nint>();
        }

        Debug.Log(startAddress.ToString("X"));
        Debug.Log(size.ToString("X"));
        Debug.Log(pattern.First());

        var scanner = new SignatureScanner(Game, startAddress, size);
        var target = new SigScanTarget(offset, pattern);

        return scanner.ScanAll(target);
    }
    #endregion

    #region SigScanTarget
    public IEnumerable<nint> ScanAll(SigScanTarget target)
    {
        var module = Game?.MainModuleWow64Safe();
        return ScanAll(module, target);
    }

    public IEnumerable<nint> ScanAll(string module, SigScanTarget target)
    {
        return ScanAll(GetModule(module), target);
    }

    public IEnumerable<nint> ScanAll(ProcessModuleWow64Safe module, SigScanTarget target)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Module could not be found!");
            return Enumerable.Empty<nint>();
        }

        var start = module.BaseAddress;
        var size = module.ModuleMemorySize;

        return ScanAll(start, size, target);
    }

    public IEnumerable<nint> ScanAll(nint startAddress, nint endAddress, SigScanTarget target)
    {
        var start = startAddress;
        var size = (int)(endAddress - (long)startAddress);

        return ScanAll(start, size, target);
    }

    public IEnumerable<nint> ScanAll(nint startAddress, int size, SigScanTarget target)
    {
        if (Game is null)
        {
            Debug.Warn("[Scan] Game process was null!");
            return Enumerable.Empty<nint>();
        }

        var scanner = new SignatureScanner(Game, startAddress, size);
        return scanner.ScanAll(target);
    }
    #endregion
}
