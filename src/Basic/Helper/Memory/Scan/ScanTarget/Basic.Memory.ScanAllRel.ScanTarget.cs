using AslHelp.MemUtils.SigScan;

public partial class Basic
{
    #region Main Module
    public IEnumerable<nint> ScanAllRel(ScanTarget target, int alignment = 1)
    {
        return ScanAllRel(target, MainModule, alignment);
    }

    public IEnumerable<nint> ScanAllRel(ScanTarget target, int size, int alignment = 1)
    {
        return ScanAllRel(target, MainModule, size, alignment);
    }
    #endregion

    #region Module Name
    public IEnumerable<nint> ScanAllRel(ScanTarget target, string moduleName, int alignment = 1)
    {
        return ScanAllRel(target, Modules[moduleName], alignment);
    }

    public IEnumerable<nint> ScanAllRel(ScanTarget target, string moduleName, int size, int alignment = 1)
    {
        return ScanAllRel(target, Modules[moduleName], size, alignment);
    }
    #endregion

    #region Module
    public IEnumerable<nint> ScanAllRel(ScanTarget target, Module module, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[ScanAll] Module could not be found.");
            return Enumerable.Empty<nint>();
        }

        return ScanAllRel(target, module.Base, module.MemorySize, alignment);
    }

    public IEnumerable<nint> ScanAllRel(ScanTarget target, Module module, int size, int alignment = 1)
    {
        if (module is null)
        {
            Debug.Warn("[ScanAll] Module could not be found.");
            return Enumerable.Empty<nint>();
        }

        return ScanAllRel(target, module.Base, size, alignment);
    }
    #endregion

    #region Address
    public IEnumerable<nint> ScanAllRel(ScanTarget target, nint startAddress, nint endAddress, int alignment = 1)
    {
        int size = (int)(endAddress - startAddress);
        return ScanAllRel(target, startAddress, size, alignment);
    }

    public IEnumerable<nint> ScanAllRel(ScanTarget target, nint startAddress, int size, int alignment = 1)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target), "[ScanAll] ScanTarget was null.");
        }

        byte[] memory = new byte[size];
        if (!TryReadSpan(memory, startAddress))
        {
            yield break;
        }

        foreach (Signature signature in target)
        {
            foreach (nint scan in ScanAllRel(signature, startAddress, size, alignment))
            {
                if (target.OnFound is not null)
                {
                    yield return target.OnFound(signature.Name, scan);
                }
                else
                {
                    yield return scan;
                }
            }
        }
    }
    #endregion
}
