using ASLHelper.Extensions;
using LiveSplit.Model;

namespace ASLHelper;

public partial class Main
{
    public Main(LiveSplitState state, object settings, object compiledScript)
    {
        State = state;
        Layout = state.Layout;

        UI = new();
        IO = new();
        Timer = new(state);
        Settings = new(settings);

        _form = state.Form;

        _script =
            Layout.Components.Append(state.Run.AutoSplitter?.Component).Cast<dynamic>()
            .FirstOrDefault(c =>
                c is not null && ((object)c).HasProperty("ComponentName")
                && c.ComponentName == "Scriptable Auto Splitter"
                && ((c.Script as object).GetFieldValue("_methods") as IEnumerable<object>)
                   .FirstOrDefault()?.GetFieldValue("_compiled_code").Equals(compiledScript)
            )?.Script;

        Instance = this;

        Debug.Log("Created ASL helper.");
    }

    public Main(LiveSplitState state, object compiledScript)
        : this(state, null, compiledScript) { }

    private protected Process _game;
    public Process Game
    {
        get
        {
            _game?.Refresh();

            if ((_game is null || _game.HasExited) && (_game = _script.GetFieldValue("_game")) is not null)
            {
                Is64Bit = _game.Is64Bit();
                PtrSize = Is64Bit ? 0x8 : 0x4;
            }

            return _game;
        }
        set
        {
            _game = value;
            Is64Bit = _game.Is64Bit();
            PtrSize = Is64Bit ? 0x8 : 0x4;

            _script.SetFieldValue("_game", value);
        }
    }

    private string _gameName;
    public string GameName
    {
        get => _gameName ?? Game?.ProcessName ?? "Auto Splitter";
        set => _gameName = value;
    }

    private protected bool TryGetModule(out ProcessModuleWow64Safe module, params string[] names)
    {
        module = null;

        if (Game is null)
            return false;

        var modules = Game.ModulesWow64Safe();
        if (modules is null || modules.Length == 0)
            return false;

        module = modules.FirstOrDefault(m => names.Any(n => n.Equals(m?.ModuleName ?? "", StringComparison.OrdinalIgnoreCase)));
        return true;
    }

    public ProcessModuleWow64Safe GetModule(params string[] names)
    {
        _ = TryGetModule(out var module, names);
        return module;
    }

    public virtual void Dispose()
    {
        Dispose(true);
    }

    public virtual void Dispose(bool removeTexts)
    {
        GC.SuppressFinalize(this);

        var closing = Debug.TraceIncludes("TimerForm_FormClosing", "OpenLayoutFromFile", "LoadDefaultLayout");
        if (!closing && removeTexts)
            UI.Text.RemoveAll();

        IO.Dispose();
    }
}
