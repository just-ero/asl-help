using System.Diagnostics;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    // TODO: Use semi-auto property in 9.0.
#pragma warning disable IDE0032 // Use auto property
    private Process? _game;
#pragma warning restore IDE0032
    public Process? Game
    {
        get
        {
            if (_game is null)
            {
                _game = _asl.Game;

                if (_game is not null)
                {
                    InitializeMemory(_game);
                }
            }

            return _game;
        }
        set
        {
            _game = value;
            _asl.Game = value;

            if (value is null)
            {
                DisposeMemory();
            }
            else
            {
                InitializeMemory(value);
            }
        }
    }

    protected abstract void InitializeMemory(Process game);
    protected abstract void DisposeMemory();
}
