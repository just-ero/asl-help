using AslHelp.MemUtils.SigScan;

public partial class Basic
{
    #region Main Module
    public nint ScanRel(ScanTarget target, int alignment = 1)
    {
        return ScanRel(target, MainModule, alignment);
    }

    public nint ScanRel(ScanTarget target, int size, int alignment = 1)
    {
        return ScanRel(target, MainModule, size, alignment);
    }
    #endregion

    #region Module Name
    public nint ScanRel(ScanTarget target, string moduleName, int alignment = 1)
    {
        return ScanRel(target, Modules[moduleName], alignment);
    }

    public nint ScanRel(ScanTarget target, string moduleName, int size, int alignment = 1)
    {
        return ScanRel(target, Modules[moduleName], size, alignment);
    }
    #endregion

    #region Module
    public nint ScanRel(ScanTarget target, Module module, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Module could not be found.");
            return default;
        }

        return ScanRel(target, module.Base, module.MemorySize, alignment);
    }

    public nint ScanRel(ScanTarget target, Module module, int size, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Module could not be found.");
            return default;
        }

        return ScanRel(target, module.Base, size, alignment);
    }
    #endregion

    #region Address
    public nint ScanRel(ScanTarget target, nint startAddress, nint endAddress, int alignment = 1)
    {
        int size = (int)(endAddress - startAddress);
        return ScanRel(target, startAddress, size, alignment);
    }

    public nint ScanRel(ScanTarget target, nint startAddress, int size, int alignment = 1)
    {
        return ScanAllRel(target, startAddress, size, alignment).FirstOrDefault();
    }
    #endregion
}
