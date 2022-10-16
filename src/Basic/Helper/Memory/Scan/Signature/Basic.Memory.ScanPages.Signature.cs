using AslHelp.MemUtils;

public partial class Basic
{
    #region Scan
    public nint ScanPages(Signature signature, int alignment = 1)
    {
        return ScanPages(false, signature, alignment);
    }

    public nint ScanPages(bool allPages, Signature signature, int alignment = 1)
    {
        return ScanPagesAll(allPages, signature, alignment).FirstOrDefault();
    }
    #endregion

    #region ScanAll
    public IEnumerable<nint> ScanPagesAll(Signature signature, int alignment = 1)
    {
        return ScanPagesAll(false, signature, alignment);
    }

    public IEnumerable<nint> ScanPagesAll(bool allPages, Signature signature, int alignment = 1)
    {
        foreach (MemoryPage page in Pages)
        {
            if (!allPages && page.Protect.HasFlag(MemPageProtect.PAGE_GUARD))
            {
                continue;
            }

            if (!allPages && page.Type != MemPageType.MEM_PRIVATE)
            {
                continue;
            }

            foreach (nint scan in ScanAll(signature, page.Base, page.RegionSize, alignment))
            {
                yield return scan;
            }
        }
    }
    #endregion

    #region ScanRel
    public nint ScanPagesRel(Signature signature, int alignment = 1)
    {
        return ScanPagesRel(false, signature, alignment);
    }

    public nint ScanPagesRel(bool allPages, Signature signature, int alignment = 1)
    {
        return ScanPagesAllRel(allPages, signature, alignment).FirstOrDefault();
    }
    #endregion

    #region ScanAllRel
    public IEnumerable<nint> ScanPagesAllRel(Signature signature, int alignment = 1)
    {
        return ScanPagesAllRel(false, signature, alignment);
    }

    public IEnumerable<nint> ScanPagesAllRel(bool allPages, Signature signature, int alignment = 1)
    {
        foreach (nint scan in ScanPagesAll(allPages, signature, alignment))
        {
            yield return Is64Bit ? scan + 0x4 + Read<int>(scan) : Read<nint>(scan);
        }
    }
    #endregion
}
