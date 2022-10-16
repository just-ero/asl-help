public partial class Basic
{
    #region Main Module
    public nint ScanRel(Signature signature, int alignment = 1)
    {
        return ScanRel(signature, MainModule, alignment);
    }

    public nint ScanRel(Signature signature, int size, int alignment = 1)
    {
        return ScanRel(signature, MainModule, size, alignment);
    }
    #endregion

    #region Module Name
    public nint ScanRel(Signature signature, string moduleName, int alignment = 1)
    {
        return ScanRel(signature, Modules[moduleName], alignment);
    }

    public nint ScanRel(Signature signature, string moduleName, int size, int alignment = 1)
    {
        return ScanRel(signature, Modules[moduleName], size, alignment);
    }
    #endregion

    #region Module
    public nint ScanRel(Signature signature, Module module, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[ScanRel] Module could not be found.");
            return default;
        }

        return ScanRel(signature, module.Base, module.MemorySize, alignment);
    }

    public nint ScanRel(Signature signature, Module module, int size, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[ScanRel] Module could not be found.");
            return default;
        }

        return ScanRel(signature, module.Base, size, alignment);
    }
    #endregion

    #region Address
    public nint ScanRel(Signature signature, nint startAddress, nint endAddress, int alignment = 1)
    {
        int size = (int)(endAddress - startAddress);
        return ScanRel(signature, startAddress, size, alignment);
    }

    public nint ScanRel(Signature signature, nint startAddress, int size, int alignment = 1)
    {
        return ScanAllRel(signature, startAddress, size, alignment).FirstOrDefault();
    }
    #endregion
}
