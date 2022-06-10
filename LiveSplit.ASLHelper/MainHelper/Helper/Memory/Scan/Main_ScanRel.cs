namespace ASLHelper;

public partial class Main
{
    #region String Pattern
    public IntPtr ScanRelRel(params string[] pattern)
    {
        var module = Game?.MainModuleWow64Safe();
        return ScanRel(module, 0, pattern);
    }

    public IntPtr ScanRel(int offset, params string[] pattern)
    {
        var module = Game?.MainModuleWow64Safe();
        return ScanRel(module, offset, pattern);
    }

    public IntPtr ScanRel(string moduleName, params string[] pattern)
    {
        var module = GetModule(moduleName);
        return ScanRel(module, 0, pattern);
    }

    public IntPtr ScanRel(string moduleName, int offset, params string[] pattern)
    {
        var module = GetModule(moduleName);
        return ScanRel(module, offset, pattern);
    }

    public IntPtr ScanRel(ProcessModuleWow64Safe module, params string[] pattern)
    {
        return ScanRel(module, 0, pattern);
    }

    public IntPtr ScanRel(ProcessModuleWow64Safe module, int offset, params string[] pattern)
    {
        if (module == null)
        {
            Debug.Warn("[Scan] Module could not be found!");
            return IntPtr.Zero;
        }

        var start = module.BaseAddress;
        var size = module.ModuleMemorySize;

        return ScanRel(start, size, offset, pattern);
    }

    public IntPtr ScanRel(IntPtr startAddress, IntPtr endAddress, params string[] pattern)
    {
        var start = startAddress;
        var size = (int)((long)endAddress - (long)startAddress);

        return ScanRel(start, size, 0, pattern);
    }

    public IntPtr ScanRel(IntPtr startAddress, IntPtr endAddress, int offset, params string[] pattern)
    {
        var start = startAddress;
        var size = (int)((long)endAddress - (long)startAddress);

        return ScanRel(start, size, offset, pattern);
    }

    public IntPtr ScanRel(IntPtr startAddress, int size, params string[] pattern)
    {
        return ScanRel(startAddress, size, 0, pattern);
    }

    public IntPtr ScanRel(IntPtr startAddress, int size, int offset, params string[] pattern)
    {
        var scan = ScanAll(startAddress, size, offset, pattern).FirstOrDefault();
        return scan == IntPtr.Zero ? scan : Game.Is64Bit() ? scan + 0x4 + Game.ReadValue<int>(scan) : Game.ReadPointer(scan);
    }
    #endregion

    #region SigScanTarget
    public IntPtr ScanRelRel(SigScanTarget target)
    {
        var module = Game?.MainModuleWow64Safe();
        return ScanRel(module, target);
    }

    public IntPtr ScanRel(string module, SigScanTarget target)
    {
        return ScanRel(GetModule(module), target);
    }

    public IntPtr ScanRel(ProcessModuleWow64Safe module, SigScanTarget target)
    {
        if (module == null)
        {
            Debug.Warn("[Scan] Module could not be found!");
            return IntPtr.Zero;
        }

        var start = module.BaseAddress;
        var size = module.ModuleMemorySize;

        return ScanRel(start, size, target);
    }

    public IntPtr ScanRel(IntPtr startAddress, IntPtr endAddress, SigScanTarget target)
    {
        var start = startAddress;
        var size = (int)((long)endAddress - (long)startAddress);

        return ScanRel(start, size, target);
    }

    public IntPtr ScanRel(IntPtr startAddress, int size, SigScanTarget target)
    {
        target.OnFound = Data.s_OnFound;
        return ScanAll(startAddress, size, target).FirstOrDefault();
    }
    #endregion
}
