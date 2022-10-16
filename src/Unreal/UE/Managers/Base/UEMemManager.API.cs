using AslHelp.UE.Models;

namespace AslHelp.UE.Managers;

public abstract partial class UEMemManager
{
    public nint GWorld { get; private set; }
    public nint GEngine { get; private set; }
    public nint GSyncLoadCount { get; private set; }

    public FNameCache FNames { get; } = new();
    public UObjectCache UObjects { get; } = new();

    public UObject this[string name] => UObjects[name];
}
