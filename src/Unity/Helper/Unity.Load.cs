using AslHelp.MemUtils.Exceptions;
using AslHelp.Mono.Managers;

public partial class Unity
{
    private const string MONO_V1 = "mono.dll";
    private const string MONO_V2 = "mono-2.0-bdwgc.dll";
    // private const string IL2CPP = "GameAssembly.dll";

    protected override UnityMemManager MakeManager()
    {
        if (MonoModule is null)
        {
            string msg = "Mono module was not found.";
            throw new FatalNotFoundException(msg);
        }

        if (UnityVersion is null)
        {
            string msg = "Unity version was not found. Consider setting it manually.";
            throw new FatalNotFoundException(msg);
        }

        return MonoModule.Name switch
        {
            MONO_V1 => new MonoV1Manager("v1"),
            MONO_V2 => (UnityVersion.Major, UnityVersion.Minor) switch
            {
                (2021, >= 2) or ( > 2021, _) => new MonoV3Manager("v3"),
                _ => new MonoV2Manager("v2")
            },
            var _ when CustomIL2CPPModules.Contains(MonoModule.Name) => new Il2CppManager(UnityVersion.Major switch
            {
                _ when !Is64Bit => "base",
                < 2019 => "base",
                > 2019 when Il2CppVersion >= 27 => "2020",
                _ => "2019"
            }),
            _ => throw new NotSupportedException("This version of Unity does not appear to be supported.")
        };
    }
}
