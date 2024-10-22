using System.Diagnostics;

using AslHelp.Memory.Ipc;

namespace AslHelp.GameEngines.Unity.Memory;

public partial class UnityMemory : ProcessMemory, IUnityReader
{
    private readonly RuntimeVersion _version;

    public UnityMemory(Process process, RuntimeVersion version)
        : base(process)
    {
        _version = version;
    }
}
