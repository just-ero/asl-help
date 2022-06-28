namespace ASLHelper;

public partial class Main
{
    #region String Pattern
    public nint ScanPages(int offset, params string[] pattern)
    {
        var target = new SigScanTarget(offset, pattern);
        return ScanPages(target);
    }
    #endregion

    #region SigScanTarget
    public nint ScanPages(SigScanTarget target, bool allPages = false)
    {
        return ScanPagesAll(target, allPages).FirstOrDefault();
    }
    #endregion
}
