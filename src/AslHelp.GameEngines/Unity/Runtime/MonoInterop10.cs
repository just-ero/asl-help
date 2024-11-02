using AslHelp.Memory.Ipc;

namespace AslHelp.GameEngines.Unity.Runtime;

public class MonoInterop10 : MonoInterop
{
    protected readonly nint _loadedAssemblies;

    protected MonoInterop10(IProcessMemory memory, string version)
        : base(memory, "mono", version) { }

    public MonoInterop10(IProcessMemory memory)
        : base(memory, "mono", "1.0")
    {
        _loadedAssemblies = GetLoadedAssemblies();
    }

    protected virtual nint GetLoadedAssemblies()
    {
        if (!_memory.)
    }
}
