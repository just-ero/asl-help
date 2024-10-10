using System;
using System.Diagnostics.CodeAnalysis;

namespace AslHelp.LiveSplit;

[Obsolete("Do not use ASL-specific features.", true)]
public abstract partial class AslPluginBase
{
    protected Autosplitter _asl;

    protected AslPluginBase(bool generateCode)
    {
        Initialize(generateCode);
    }

    [field: AllowNull]
    public string GameName
    {
        get => field ?? Game?.ProcessName ?? "Autosplitter";
        set;
    }
}
