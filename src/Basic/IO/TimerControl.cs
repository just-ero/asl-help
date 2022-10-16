using LiveSplit.Model;

namespace AslHelp.IO;

public class TimerControl
{
    private static readonly TimerModel _model;

    static TimerControl()
    {
        _model = new() { CurrentState = timer.State };
    }

    public void Start(Func<bool> func = null)
    {
        if (func is null || func())
        {
            _model.Start();
        }
    }

    public void Split(Func<bool> func = null)
    {
        if (func is null || func())
        {
            _model.Split();
        }
    }

    public void Reset(Func<bool> func = null)
    {
        if (func is null || func())
        {
            _model.Reset();
        }
    }

    public void Pause(Func<bool> func = null)
    {
        if (func is null || func())
        {
            _model.Reset();
        }
    }

    public void Unpause(Func<bool> func = null)
    {
        if (func is null || func())
        {
            _model.UndoAllPauses();
        }
    }

    public void Skip(Func<bool> func = null)
    {
        if (func is null || func())
        {
            _model.SkipSplit();
        }
    }

    public void Undo(Func<bool> func = null)
    {
        if (func is null || func())
        {
            _model.UndoSplit();
        }
    }
}
