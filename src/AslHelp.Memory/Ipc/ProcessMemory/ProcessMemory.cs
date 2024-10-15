using System.Diagnostics;

namespace AslHelp.Memory.Ipc;

public partial class ProcessMemory : IProcessMemory
{
    private readonly Process _process;

    public ProcessMemory(Process process)
    {
        _process = process;
    }
}
