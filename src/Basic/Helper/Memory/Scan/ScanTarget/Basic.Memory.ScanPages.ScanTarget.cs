using AslHelp.MemUtils;
using AslHelp.MemUtils.SigScan;

public partial class Basic
{
    #region Scan
    public nint ScanPages(ScanTarget target, int alignment = 1)
    {
        return ScanPages(false, target, alignment);
    }

    public nint ScanPages(bool allPages, ScanTarget target, int alignment = 1)
    {
        return ScanPagesAll(allPages, target, alignment).FirstOrDefault();
    }
    #endregion

    #region ScanAll
    public IEnumerable<nint> ScanPagesAll(ScanTarget target, int alignment = 1)
    {
        return ScanPagesAll(false, target, alignment);
    }

    public IEnumerable<nint> ScanPagesAll(bool allPages, ScanTarget target, int alignment = 1)
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

            foreach (nint scan in ScanAll(target, page.Base, page.RegionSize, alignment))
            {
                yield return scan;
            }
        }
    }
    #endregion
}
