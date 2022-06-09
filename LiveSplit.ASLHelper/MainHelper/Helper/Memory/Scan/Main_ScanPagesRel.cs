using LiveSplit.ComponentUtil;
using System;
using System.Linq;

namespace ASLHelper
{
    public partial class Main
    {
        #region String Pattern
        public IntPtr ScanPagesRel(int offset, params string[] pattern)
        {
            var target = new SigScanTarget(offset, pattern);
            return ScanPagesRel(target);
        }
        #endregion

        #region SigScanTarget
        public IntPtr ScanPagesRel(SigScanTarget target, bool allPages = false)
        {
            target.OnFound = Data.s_OnFound;
            return ScanPagesAll(target, allPages).FirstOrDefault();
        }
        #endregion
    }
}