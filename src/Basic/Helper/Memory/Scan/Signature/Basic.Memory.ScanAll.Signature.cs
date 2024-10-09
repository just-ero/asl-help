using AslHelp.MemUtils.SigScan;

public partial class Basic
{
    #region Main Module
    public IEnumerable<nint> ScanAll(Signature signature, int alignment = 1)
    {
        return ScanAll(signature, MainModule, alignment);
    }

    public IEnumerable<nint> ScanAll(Signature signature, int size, int alignment = 1)
    {
        return ScanAll(signature, MainModule, size, alignment);
    }
    #endregion

    #region Module Name
    public IEnumerable<nint> ScanAll(Signature signature, string moduleName, int alignment = 1)
    {
        return ScanAll(signature, Modules[moduleName], alignment);
    }

    public IEnumerable<nint> ScanAll(Signature signature, string moduleName, int size, int alignment = 1)
    {
        return ScanAll(signature, Modules[moduleName], size, alignment);
    }
    #endregion

    #region Module
    public IEnumerable<nint> ScanAll(Signature signature, Module module, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[ScanAll] Module could not be found.");
            return Enumerable.Empty<nint>();
        }

        return ScanAll(signature, module.Base, module.MemorySize, alignment);
    }

    public IEnumerable<nint> ScanAll(Signature signature, Module module, int size, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[ScanAll] Module could not be found.");
            return Enumerable.Empty<nint>();
        }

        return ScanAll(signature, module.Base, size, alignment);
    }
    #endregion

    #region Address
    public IEnumerable<nint> ScanAll(Signature signature, nint startAddress, nint endAddress, int alignment = 1)
    {
        int size = (int)(endAddress - startAddress);
        return ScanAll(signature, startAddress, size, alignment);
    }

    public IEnumerable<nint> ScanAll(Signature signature, nint startAddress, int size, int alignment = 1)
    {
        byte[] memory = new byte[size];
        if (!TryReadArray_Internal<byte>(memory, startAddress))
        {
            yield break;
        }

        foreach (int scan in new ScanEnumerator(memory, signature, alignment))
        {
            yield return startAddress + scan + signature.Offset;
        }
    }
    #endregion
}
