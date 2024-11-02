using System.Diagnostics;

using AslHelp.Memory.Ipc;

namespace AslHelp.GameEngines.Unity.Memory;

public partial class UnityMemory : ProcessMemory, IUnityReader
{
    private readonly DotnetRuntimeVersion _version;

    public UnityMemory(Process process, DotnetRuntimeVersion version)
        : base(process)
    {
        _version = version;
    }
}
