using AslHelp.MemUtils.Exceptions;
using AslHelp.UE.Managers;

public partial class Unreal
{
    protected override UEMemManager MakeManager()
    {
        if (MainModule is null)
        {
            string msg = "Game's main module was not found.";
            throw new FatalNotFoundException(msg);
        }

        if (UEVersion is null)
        {
            string msg = "Unreal Engine version was not found. Consider setting it manually.";
            throw new FatalNotFoundException(msg);
        }

        return (UEVersion.Major, UEVersion.Minor) switch
        {
            (4, < 8) => new UE4_0Manager(UEVersion.Major, UEVersion.Minor),
            (4, < 11) => new UE4_8Manager(UEVersion.Major, UEVersion.Minor),
            (4, < 20) => new UE4_11Manager(UEVersion.Major, UEVersion.Minor),
            (4, < 23) => new UE4_20Manager(UEVersion.Major, UEVersion.Minor),
            (4, < 25) => new UE4_23Manager(UEVersion.Major, UEVersion.Minor),
            (4, _) or (5, _) => new UE4_25Manager(UEVersion.Major, UEVersion.Minor),
            _ => throw new NotSupportedException($"Unreal Engine version {UEVersion.Major}.{UEVersion.Minor} is not supported yet.")
        };
    }
}
