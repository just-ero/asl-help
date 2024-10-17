using System.Diagnostics;

using AslHelp.Memory.Ipc;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    // TODO: Use semi-auto property in 9.0.
#pragma warning disable IDE0032 // Use auto property
    private Process? _game;
#pragma warning restore IDE0032

    public Process? Game
    {
        get => _game ??= _asl.Game;
        set
        {
            _game = value;
            _asl.Game = value;

            if (value is null)
            {
                DisposeMemory();
            }
        }
    }

    protected abstract IProcessMemory Memory { get; }

    protected abstract void DisposeMemory();
}
