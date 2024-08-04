public partial class Basic
{
    #region Main Module
    public IEnumerable<nint> ScanAllRel(int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAllRel(signature, MainModule);
    }

    public IEnumerable<nint> ScanAllRel(int offset, params byte[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAllRel(signature, MainModule);
    }
    #endregion

    #region Module Name
    public IEnumerable<nint> ScanAllRel(string moduleName, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAllRel(signature, Modules[moduleName]);
    }

    public IEnumerable<nint> ScanAllRel(string moduleName, int offset, params byte[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAllRel(signature, Modules[moduleName]);
    }
    #endregion

    #region Module
    public IEnumerable<nint> ScanAllRel(Module module, int offset, params string[] pattern)
    {
        if (module is null)
        {
            Debug.Warn("[ScanAllRel] Module could not be found.");
            return Enumerable.Empty<nint>();
        }

        Signature signature = new(offset, pattern);
        return ScanAllRel(signature, module.Base, module.MemorySize);
    }

    public IEnumerable<nint> ScanAllRel(Module module, int offset, params byte[] pattern)
    {
        if (module is null)
        {
            Debug.Warn("[ScanAllRel] Module could not be found.");
            return Enumerable.Empty<nint>();
        }

        Signature signature = new(offset, pattern);
        return ScanAllRel(signature, module.Base, module.MemorySize);
    }
    #endregion

    #region Address
    public IEnumerable<nint> ScanAllRel(nint startAddress, nint endAddress, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAllRel(signature, startAddress, endAddress);
    }

    public IEnumerable<nint> ScanAllRel(nint startAddress, int size, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        return ScanAllRel(signature, startAddress, size);
    }
    #endregion
}
