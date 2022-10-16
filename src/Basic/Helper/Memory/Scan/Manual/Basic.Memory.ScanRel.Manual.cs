public partial class Basic
{
    #region Main Module
    public nint ScanRel(int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanRel(signature, MainModule);
    }

    public nint ScanRel(int offset, params byte[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanRel(signature, MainModule);
    }
    #endregion

    #region Module Name
    public nint ScanRel(string moduleName, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanRel(signature, Modules[moduleName]);
    }

    public nint ScanRel(string moduleName, int offset, params byte[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanRel(signature, Modules[moduleName]);
    }
    #endregion

    #region Module
    public nint ScanRel(Module module, int offset, params string[] pattern)
    {
        if (module is null)
        {
            Debug.Warn("[ScanRel] Module could not be found.");
            return default;
        }

        Signature signature = new(offset, pattern);
        return ScanRel(signature, module.Base, module.MemorySize);
    }

    public nint ScanRel(Module module, int offset, params byte[] pattern)
    {
        if (module is null)
        {
            Debug.Warn("[ScanRel] Module could not be found.");
            return default;
        }

        Signature signature = new(offset, pattern);
        return ScanRel(signature, module.Base, module.MemorySize);
    }
    #endregion

    #region Address
    public nint ScanRel(nint startAddress, nint endAddress, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanRel(signature, startAddress, endAddress);
    }

    public nint ScanRel(nint startAddress, int size, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanRel(signature, startAddress, size);
    }
    #endregion
}
