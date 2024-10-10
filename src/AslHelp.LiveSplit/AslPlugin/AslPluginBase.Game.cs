using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    [field: AllowNull]
    public Process? Game
    {
        get
        {
            if (field is null)
            {
                field = _asl.Game;

                if (field is not null)
                {
                    InitializeMemory(field);
                }
            }
            else if (field.HasExited)
            {
                DisposeMemory();
            }

            return field;
        }
        set
        {
            field = value;
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
