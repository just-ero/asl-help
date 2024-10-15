using System;
using System.Windows.Forms;

namespace AslHelp.LiveSplit;

[Obsolete("Do not use ASL-specific features.", true)]
public abstract partial class AslPluginBase
{
    protected Autosplitter _asl;

    protected AslPluginBase(bool generateCode)
    {
        try
        {
            Initialize(generateCode);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "An error occurred while initializing asl-help:" + "\n" + ex,
                "LiveSplit | asl-help");

            throw;
        }
    }

    // TODO: Use semi-auto property in 9.0.
#pragma warning disable IDE0032 // Use auto property
    private string? _gameName;
#pragma warning restore IDE0032
    public string GameName
    {
        get => _gameName ?? Game?.ProcessName ?? "Autosplitter";
        set => _gameName = value;
    }
}
