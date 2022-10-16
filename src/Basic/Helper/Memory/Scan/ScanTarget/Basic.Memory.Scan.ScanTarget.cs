using AslHelp.MemUtils.SigScan;

public partial class Basic
{
    #region Main Module
    public nint Scan(ScanTarget target, int alignment = 1)
    {
        return Scan(target, MainModule, alignment);
    }

    public nint Scan(ScanTarget target, int size, int alignment = 1)
    {
        return Scan(target, MainModule, size, alignment);
    }
    #endregion

    #region Module Name
    public nint Scan(ScanTarget target, string moduleName, int alignment = 1)
    {
        return Scan(target, Modules[moduleName], alignment);
    }

    public nint Scan(ScanTarget target, string moduleName, int size, int alignment = 1)
    {
        return Scan(target, Modules[moduleName], size, alignment);
    }
    #endregion

    #region Module
    public nint Scan(ScanTarget target, Module module, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Module could not be found.");
            return default;
        }

        return Scan(target, module.Base, module.MemorySize, alignment);
    }

    public nint Scan(ScanTarget target, Module module, int size, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Module could not be found.");
            return default;
        }

        return Scan(target, module.Base, size, alignment);
    }
    #endregion

    #region Address
    public nint Scan(ScanTarget target, nint startAddress, nint endAddress, int alignment = 1)
    {
        int size = (int)(endAddress - startAddress);
        return Scan(target, startAddress, size, alignment);
    }

    public nint Scan(ScanTarget target, nint startAddress, int size, int alignment = 1)
    {
        return ScanAll(target, startAddress, size, alignment).FirstOrDefault();
    }
    #endregion
}
