namespace ASLHelper;

public partial class Main
{
    #region String Pattern
    public nint ScanPagesRel(int offset, params string[] pattern)
    {
        var target = new SigScanTarget(offset, pattern);
        return ScanPagesRel(target);
    }
    #endregion

    #region SigScanTarget
    public nint ScanPagesRel(SigScanTarget target, bool allPages = false)
    {
        target.OnFound = Data.s_OnFound;
        return ScanPagesAll(target, allPages).FirstOrDefault();
    }
    #endregion
}
