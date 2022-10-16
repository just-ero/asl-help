public partial class Basic
{
    #region Scan
    public nint ScanPages(int offset, params string[] pattern)
    {
        return ScanPages(false, offset, pattern);
    }

    public nint ScanPages(bool allPages, int offset, params string[] pattern)
    {
        return ScanPagesAll(allPages, offset, pattern).FirstOrDefault();
    }
    #endregion

    #region ScanAll
    public IEnumerable<nint> ScanPagesAll(int offset, params string[] pattern)
    {
        return ScanPagesAll(false, offset, pattern);
    }

    public IEnumerable<nint> ScanPagesAll(bool allPages, int offset, params string[] pattern)
    {
        Signature signature = new(offset, pattern);
        foreach (nint scan in ScanPagesAll(allPages, signature))
        {
            yield return scan;
        }
    }
    #endregion

    #region ScanRel
    public nint ScanPagesRel(int offset, params string[] pattern)
    {
        return ScanPagesRel(false, offset, pattern);
    }

    public nint ScanPagesRel(bool allPages, int offset, params string[] pattern)
    {
        return ScanPagesAllRel(allPages, offset, pattern).FirstOrDefault();
    }
    #endregion

    #region ScanAllRel
    public IEnumerable<nint> ScanPagesAllRel(int offset, params string[] pattern)
    {
        return ScanPagesAllRel(false, offset, pattern);
    }

    public IEnumerable<nint> ScanPagesAllRel(bool allPages, int offset, params string[] pattern)
    {
        foreach (nint scan in ScanPagesAll(allPages, offset, pattern))
        {
            yield return Is64Bit ? scan + 0x4 + Read<int>(scan) : Read<nint>(scan);
        }
    }
    #endregion
}
