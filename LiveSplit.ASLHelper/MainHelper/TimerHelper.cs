using LiveSplit.Model;

namespace ASLHelper.MainHelper;

public class TimerHelper
{
    public TimerHelper(LiveSplitState state)
    {
        _model = new TimerModel { CurrentState = state };
    }

    private readonly TimerModel _model;

    public void Start(Func<bool> func = null)
    {
        if (func?.Invoke() ?? true)
            _model.Start();
    }

    public void Split(Func<bool> func = null)
    {
        if (func?.Invoke() ?? true)
            _model.Split();
    }

    public void Reset(Func<bool> func = null)
    {
        if (func?.Invoke() ?? true)
            _model.Reset();
    }

    public void Pause(Func<bool> func = null)
    {
        if (func?.Invoke() ?? true)
            _model.Reset();
    }

    public void UnPause(Func<bool> func = null)
    {
        if (func?.Invoke() ?? true)
            _model.UndoAllPauses();
    }

    public void Skip(Func<bool> func = null)
    {
        if (func?.Invoke() ?? true)
            _model.SkipSplit();
    }

    public void Undo(Func<bool> func = null)
    {
        if (func?.Invoke() ?? true)
            _model.UndoSplit();
    }
}
