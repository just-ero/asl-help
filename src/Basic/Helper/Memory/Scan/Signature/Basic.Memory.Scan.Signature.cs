public partial class Basic
{
    #region Main Module
    public nint Scan(Signature signature, int alignment = 1)
    {
        return Scan(signature, MainModule, alignment);
    }

    public nint Scan(Signature signature, int size, int alignment = 1)
    {
        return Scan(signature, MainModule, size, alignment);
    }
    #endregion

    #region Module Name
    public nint Scan(Signature signature, string moduleName, int alignment = 1)
    {
        return Scan(signature, Modules[moduleName], alignment);
    }

    public nint Scan(Signature signature, string moduleName, int size, int alignment = 1)
    {
        return Scan(signature, Modules[moduleName], size, alignment);
    }
    #endregion

    #region Module
    public nint Scan(Signature signature, Module module, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Module could not be found.");
            return default;
        }

        return Scan(signature, module.Base, module.MemorySize, alignment);
    }

    public nint Scan(Signature signature, Module module, int size, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Module could not be found.");
            return default;
        }

        return Scan(signature, module.Base, size, alignment);
    }
    #endregion

    #region Address
    public nint Scan(Signature signature, nint startAddress, nint endAddress, int alignment = 1)
    {
        int size = (int)(endAddress - startAddress);
        return Scan(signature, startAddress, size, alignment);
    }

    public nint Scan(Signature signature, nint startAddress, int size, int alignment = 1)
    {
        return ScanAll(signature, startAddress, size, alignment).FirstOrDefault();
    }
    #endregion
}
