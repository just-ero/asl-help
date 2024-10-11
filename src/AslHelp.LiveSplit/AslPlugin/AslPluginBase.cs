using System;

namespace AslHelp.LiveSplit;

[Obsolete("Do not use ASL-specific features.", true)]
public abstract partial class AslPluginBase
{
    protected Autosplitter _asl;

    protected AslPluginBase(bool generateCode)
    {
        Initialize(generateCode);
    }

    // TODO: Use semi-auto property in RC2.
#pragma warning disable IDE0032 // Use auto property
    private string? _gameName;
#pragma warning restore IDE0032
    public string GameName
    {
        get => _gameName ?? Game?.ProcessName ?? "Autosplitter";
        set => _gameName = value;
    }
}
