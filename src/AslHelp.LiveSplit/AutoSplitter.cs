extern alias Ls;

using Ls::LiveSplit.ASL;
using Ls::LiveSplit.Model;

using System;
using System.Collections.Generic;
using System.Diagnostics;

using AslHelp.LiveSplit.Extensions;

namespace AslHelp.LiveSplit;

[Obsolete("Do not use ASL-specific features.", true)]
public sealed partial class Autosplitter
{
    private readonly ASLScript _script;

    private Autosplitter(LiveSplitState state, ASLScript script, ScriptActions actions, ASLSettingsBuilder settingsBuilder)
    {
        _script = script;

        State = state;
        Actions = actions;
        Vars = script.Vars;
        // Current = script.State.Data;
        SettingsBuilder = settingsBuilder;
    }

    public LiveSplitState State { get; }

    public ScriptActions Actions { get; }
    public IDictionary<string, object?> Vars { get; }
    // public IDictionary<string, object?>? Current { get; }
    public ASLSettingsBuilder SettingsBuilder { get; }

    public Process? Game
    {
        get => _script.GetGame();
        set => _script.SetGame(value);
    }
}
