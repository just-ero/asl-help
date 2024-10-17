using System;
using System.Diagnostics;

using AslHelp.Memory.Ipc;

[Obsolete("Do not use ASL-specific features.", true)]
public sealed partial class Unity : Basic
{
    protected override IProcessMemory InitializeMemory(Process process)
    {
        return base.InitializeMemory(process);
    }
}
