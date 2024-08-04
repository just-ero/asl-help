public partial class Basic
{
    #region Main Module
    public IEnumerable<nint> ScanAllRel(Signature signature, int alignment = 1)
    {
        return ScanAllRel(signature, MainModule, alignment);
    }

    public IEnumerable<nint> ScanAllRel(Signature signature, int size, int alignment = 1)
    {
        return ScanAllRel(signature, MainModule, size, alignment);
    }
    #endregion

    #region Module Name
    public IEnumerable<nint> ScanAllRel(Signature signature, string moduleName, int alignment = 1)
    {
        return ScanAllRel(signature, Modules[moduleName], alignment);
    }

    public IEnumerable<nint> ScanAllRel(Signature signature, string moduleName, int size, int alignment = 1)
    {
        return ScanAllRel(signature, Modules[moduleName], size, alignment);
    }
    #endregion

    #region Module
    public IEnumerable<nint> ScanAllRel(Signature signature, Module module, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[ScanAllRel] Module could not be found.");
            return Enumerable.Empty<nint>();
        }

        return ScanAllRel(signature, module.Base, module.MemorySize, alignment);
    }

    public IEnumerable<nint> ScanAllRel(Signature signature, Module module, int size, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[ScanAllRel] Module could not be found.");
            return Enumerable.Empty<nint>();
        }

        return ScanAllRel(signature, module.Base, size, alignment);
    }
    #endregion

    #region Address
    public IEnumerable<nint> ScanAllRel(Signature signature, nint startAddress, nint endAddress, int alignment = 1)
    {
        int size = (int)(endAddress - startAddress);
        return ScanAllRel(signature, startAddress, size, alignment);
    }

    public IEnumerable<nint> ScanAllRel(Signature signature, nint startAddress, int size, int alignment = 1)
    {
        foreach (nint scan in ScanAll(signature, startAddress, size, alignment))
        {
            yield return FromAssemblyAddress(scan);
        }
    }
    #endregion
}
