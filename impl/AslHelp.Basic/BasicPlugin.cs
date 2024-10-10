using System;

using AslHelp.LiveSplit;
using AslHelp.Memory.Ipc;

[Obsolete("Do not use ASL-specific features.", true)]
public partial class Basic : AslPluginBase, IProcessMemory
{
    public Basic()
        : this(true) { }

    public Basic(bool generateCode)
        : base(generateCode) { }
}
