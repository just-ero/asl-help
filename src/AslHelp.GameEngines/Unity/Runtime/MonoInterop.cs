using AslHelp.Memory;
using AslHelp.Memory.Ipc;
using AslHelp.Memory.NativeStructs;

namespace AslHelp.GameEngines.Unity.Runtime;

public abstract class MonoInterop
{
    private readonly IProcessMemory _memory;
    private readonly NativeStructCollection _structs;

    protected MonoInterop(IProcessMemory memory, string runtime, string version)
    {
        _memory = memory;
        _structs = NativeStructCollection.Parse(("AslHelp.GameEngines.Unity.Runtime.Structs", runtime, version), memory.Is64Bit)
            .Unwrap();
    }

    public static MonoInterop Initialize(IProcessMemory memory)
    {
        if (memory.Modules.TryGetValue("mono.dll", out var module))
        {

        }
        else if (memory.Modules.TryGetValue("mono-2.0-bdwgc.dll", out module))
        {

        }
        else if (memory.Modules.TryGetValue("GameAssembly.dll", out module))
        {

        }
        else
        {

        }
    }

    public static MonoInterop Initialize(IProcessMemory memory, Module module, MonoRuntimeVersion version)
    {
        if (version == MonoRuntimeVersion.Mono10)
        {
            return new MonoInterop10(memory, module);
        }
        else if (version == MonoRuntimeVersion.Mono11)
        {
            return new MonoInterop11(memory, module);
        }
        else if (version == MonoRuntimeVersion.Mono20)
        {
            return new MonoInterop20(memory, module);
        }
        else if (version == MonoRuntimeVersion.Mono21)
        {
            return new MonoInterop21(memory, module);
        }
        else if (version == MonoRuntimeVersion.Il2Cpp240)
        {
            return new Il2CppInterop240(memory, module);
        }
        else
        {
            return null;
        }
    }
}
