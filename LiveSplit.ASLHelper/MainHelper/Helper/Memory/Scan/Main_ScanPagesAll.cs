namespace ASLHelper;

public partial class Main
{
    #region String Pattern
    public IEnumerable<nint> ScanPagesAll(int offset, params string[] pattern)
    {
        var target = new SigScanTarget(offset, pattern);
        return ScanPagesAll(target);
    }
    #endregion

    #region SigScanTarget
    public IEnumerable<nint> ScanPagesAll(SigScanTarget target, bool allPages = false)
    {
        if (Game == null)
        {
            Debug.Warn("[Scan] Game process was null!");
            yield break;
        }

        foreach (var page in Game.MemoryPages(allPages))
        {
            var scanner = new SignatureScanner(Game, page.BaseAddress, (int)page.RegionSize);

            foreach (var scan in scanner.ScanAll(target))
                yield return scan;
        }
    }
    #endregion
}
