public partial class Basic
{
    #region Main Module
    public IEnumerable<nint> ScanAll(int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAll(signature, MainModule);
    }

    public IEnumerable<nint> ScanAll(int offset, params byte[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAll(signature, MainModule);
    }
    #endregion

    #region Module Name
    public IEnumerable<nint> ScanAll(string moduleName, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAll(signature, Modules[moduleName]);
    }

    public IEnumerable<nint> ScanAll(string moduleName, int offset, params byte[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAll(signature, Modules[moduleName]);
    }
    #endregion

    #region Module
    public IEnumerable<nint> ScanAll(Module module, int offset, params string[] pattern)
    {
        if (module is null)
        {
            Debug.Warn("[ScanAll] Module could not be found.");
            return Enumerable.Empty<nint>();
        }

        Signature signature = new(offset, pattern);
        return ScanAll(signature, module.Base, module.MemorySize);
    }

    public IEnumerable<nint> ScanAll(Module module, int offset, params byte[] pattern)
    {
        if (module is null)
        {
            Debug.Warn("[ScanAll] Module could not be found.");
            return Enumerable.Empty<nint>();
        }

        Signature signature = new(offset, pattern);
        return ScanAll(signature, module.Base, module.MemorySize);
    }
    #endregion

    #region Address
    public IEnumerable<nint> ScanAll(nint startAddress, nint endAddress, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAll(signature, startAddress, endAddress);
    }

    public IEnumerable<nint> ScanAll(nint startAddress, int size, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAll(signature, startAddress, size);
    }
    #endregion
}
