using AslHelp.MemUtils.Exceptions;
using AslHelp.Mono.Managers;

public partial class Unity
{
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

        if (MonoV1Modules.Contains(MainModule.Name))
        {
            return new MonoV1Manager("v1");
        }

        if (MonoV2Modules.Contains(MainModule.Name))
        {
            return (UnityVersion.Major, UnityVersion.Minor) switch
            {
                (2021, >= 2) or ( > 2021, _) => new MonoV3Manager("v3"),
                _ => new MonoV2Manager("v2")
            };
        }

        if (IL2CPPModules.Contains(MonoModule.Name))
        {
            return new Il2CppManager(UnityVersion.Major switch
            {
                _ when !Is64Bit => "base",
                < 2019 => "base",
                > 2019 when Il2CppVersion >= 27 => "2020",
                _ => "2019"
            });
        };

        throw new NotSupportedException("This version of Unity does not appear to be supported.");
    }
}
