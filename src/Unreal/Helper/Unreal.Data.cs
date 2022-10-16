using AslHelp.Data.AutoSplitter;
using AslHelp.MemUtils.SigScan;

public partial class Unreal
{
    public bool IsLoading => Read<bool>(Manager.GSyncLoadCount);

    private Version _ueVersion;
    public Version UEVersion
    {
        get
        {
            if (_ueVersion is not null)
            {
                return _ueVersion;
            }

            if (Game is null)
            {
                return null;
            }

            Debug.Info("Retrieving Unreal Engine version...");

            ScanTarget engineVersionTrg = new();

            if (Is64Bit)
            {
                engineVersionTrg.Add(Data.GEngineTrg_64);
                engineVersionTrg.Add(Data.FEngineTrg_64);

                engineVersionTrg.OnFound = Data.EngineTrg_OnFound_64;
            }
            else
            {
                engineVersionTrg.Add(Data.GEngineTrg_32);
                engineVersionTrg.Add(Data.FEngineTrg_32);

                engineVersionTrg.OnFound = Data.EngineTrg_OnFound_32;
            }

            nint engineVersion = Scan(engineVersionTrg);

            if (engineVersion != 0)
            {
                Span<ushort> verNums = stackalloc ushort[2];

                if (TryReadSpan_Internal(verNums, engineVersion))
                {
                    _ueVersion = new(verNums[0], verNums[1]);
                    goto Success;
                }
            }

            // Fall back on file version number (not guaranteed to be exported).

            FileVersionInfo vi = MainModule.VersionInfo;
            _ueVersion = new(vi.FileMajorPart, vi.FileMinorPart);

        Success:
            Debug.Info($"  => Unreal {_ueVersion.Major}.{_ueVersion.Minor}.");
            Debug.Info($"  => Doesn't look right? You can set the helper's `{nameof(UEVersion)}` manually in 'startup {{}}':");
            Debug.Info($"     `vars.Helper.UEVersion = new Version(4, 0);`");

            return _ueVersion;
        }
        set
        {
            if (Actions.Current != "startup")
            {
                string msg = $"{nameof(UEVersion)} may only be set in the 'startup {{}}' action.";
                throw new InvalidOperationException(msg);
            }

            if (value is null)
            {
                string msg = $"{nameof(UEVersion)} may not be null.";
                throw new ArgumentNullException(msg);
            }

            _ueVersion = value;
        }
    }
}
