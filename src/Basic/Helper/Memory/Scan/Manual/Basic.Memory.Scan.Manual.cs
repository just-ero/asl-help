public partial class Basic
{
    #region Main Module
    public nint Scan(int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return Scan(signature, MainModule);
    }

    public nint Scan(int offset, params byte[] pattern)
    {
        Signature signature = new(offset, pattern);
        return Scan(signature, MainModule);
    }
    #endregion

    #region Module Name
    public nint Scan(string moduleName, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return Scan(signature, Modules[moduleName]);
    }

    public nint Scan(string moduleName, int offset, params byte[] pattern)
    {
        Signature signature = new(offset, pattern);
        return Scan(signature, Modules[moduleName]);
    }
    #endregion

    #region Module
    public nint Scan(Module module, int offset, params string[] pattern)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Module could not be found.");
            return default;
        }

        Signature signature = new(offset, pattern);
        return Scan(signature, module.Base, module.MemorySize);
    }

    public nint Scan(Module module, int offset, params byte[] pattern)
    {
        if (module is null)
        {
            Debug.Warn("[Scan] Module could not be found.");
            return default;
        }

        Signature signature = new(offset, pattern);
        return Scan(signature, module.Base, module.MemorySize);
    }
    #endregion

    #region Address
    public nint Scan(nint startAddress, nint endAddress, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return Scan(signature, startAddress, endAddress);
    }

    public nint Scan(nint startAddress, int size, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return Scan(signature, startAddress, size);
    }
    #endregion
}
