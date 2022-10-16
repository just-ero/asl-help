namespace AslHelp.Emulators.Managers;

public abstract partial class EmuMemManager
{
    public nint WRAM { get; private set; }

    public abstract bool VerifyWRAM();
    public abstract string Name { get; }
    public abstract string Core { get; }
}
