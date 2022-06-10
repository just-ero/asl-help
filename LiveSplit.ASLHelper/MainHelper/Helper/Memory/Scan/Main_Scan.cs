namespace ASLHelper;

public partial class Main
{
    #region String Pattern
    public IntPtr Scan(int offset, params string[] pattern)
    {
        var module = Game?.MainModuleWow64Safe();
        return Scan(module, offset, pattern);
    }

    public IntPtr Scan(string module, params string[] pattern)
    {
        return Scan(GetModule(module), 0, pattern);
    }

    public IntPtr Scan(string module, int offset, params string[] pattern)
    {
        return Scan(GetModule(module), offset, pattern);
    }

    public IntPtr Scan(ProcessModuleWow64Safe module, params string[] pattern)
    {
        return Scan(module, 0, pattern);
    }

    public IntPtr Scan(ProcessModuleWow64Safe module, int offset, params string[] pattern)
    {
        if (module == null)
        {
            Debug.Warn("[Scan] Could not find module!");
            return IntPtr.Zero;
        }

        var start = module.BaseAddress;
        var size = module.ModuleMemorySize;

        return Scan(start, size, offset, pattern);
    }

    public IntPtr Scan(IntPtr startAddress, IntPtr endAddress, params string[] pattern)
    {
        var start = startAddress;
        var size = (int)((long)endAddress - (long)startAddress);

        return Scan(start, size, 0, pattern);
    }

    public IntPtr Scan(IntPtr startAddress, IntPtr endAddress, int offset, params string[] pattern)
    {
        var start = startAddress;
        var size = (int)((long)endAddress - (long)startAddress);

        return Scan(start, size, offset, pattern);
    }

    public IntPtr Scan(IntPtr startAddress, int size, params string[] pattern)
    {
        return Scan(startAddress, size, 0, pattern);
    }

    public IntPtr Scan(IntPtr startAddress, int size, int offset, params string[] pattern)
    {
        return ScanAll(startAddress, size, offset, pattern).FirstOrDefault();
    }
    #endregion

    #region SigScanTarget
    public IntPtr Scan(SigScanTarget target)
    {
        var module = Game?.MainModuleWow64Safe();
        return Scan(module, target);
    }

    public IntPtr Scan(string module, SigScanTarget target)
    {
        return Scan(GetModule(module), target);
    }

    public IntPtr Scan(ProcessModuleWow64Safe module, SigScanTarget target)
    {
        if (module == null)
        {
            Debug.Warn("[Scan] Module could not be found!");
            return IntPtr.Zero;
        }

        var start = module.BaseAddress;
        var size = module.ModuleMemorySize;

        return Scan(start, size, target);
    }

    public IntPtr Scan(IntPtr startAddress, IntPtr endAddress, SigScanTarget target)
    {
        var start = startAddress;
        var size = (int)((long)endAddress - (long)startAddress);

        return Scan(start, size, target);
    }

    public IntPtr Scan(IntPtr startAddress, int size, SigScanTarget target)
    {
        return ScanAll(startAddress, size, target).FirstOrDefault();
    }
    #endregion
}
