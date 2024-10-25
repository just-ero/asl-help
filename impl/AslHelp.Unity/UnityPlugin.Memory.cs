using System.Diagnostics;

using AslHelp.GameEngines.Unity.Memory;
using AslHelp.Memory.Ipc;

public partial class Unity
{
    private new UnityMemory Memory => (UnityMemory)base.Memory;

    protected override IProcessMemory InitializeMemory(Process process)
    {
        return new UnityMemory(process);
    }
}
