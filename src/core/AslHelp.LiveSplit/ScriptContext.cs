using AslHelp.LiveSplit.Settings;
using AslHelp.Logging;
using AslHelp.Reflection;
using LiveSplit.ASL;
using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace AslHelp.LiveSplit.Asl.Attach;

/// <summary>
///     Provides access to the attached ASL script: its path, actions, variables, settings, and the
///     action that loaded asl-help.
/// </summary>
[DebuggerDisplay("{Path,nq} (from {CallingAction,nq})")]
public sealed class ScriptContext
{
    private readonly ASLScript _script;

    private ScriptContext(
        ASLScript script,
        string path,
        string callingAction,
        ScriptActions actions,
        SettingsBuilder settings)
    {
        _script = script;

        Path = path;
        CallingAction = callingAction;
        Actions = actions;
        SettingsBuilder = settings;
    }

    /// <summary>
    ///     Gets the full path of the running script file.
    /// </summary>
    public string Path { get; }

    /// <summary>
    ///     Gets the name of the ASL action that loaded asl-help (always <c>startup</c>).
    /// </summary>
    public string CallingAction { get; }

    /// <summary>
    ///     Gets the actions of the attached script.
    /// </summary>
    public ScriptActions Actions { get; }

    /// <summary>
    ///     Gets the settings builder for the attached script.
    /// </summary>
    public SettingsBuilder SettingsBuilder { get; }

    /// <summary>
    ///     Gets the script's <c>vars</c> dictionary.
    /// </summary>
    public IDictionary<string, object?> Vars => _script.Vars;

    /// <summary>
    ///     Gets the state's <c>current</c> dictionary,
    ///     or <see langword="null"/> before the script has a state.
    /// </summary>
    public IDictionary<string, object?>? Current => _script.State?.Data;

    /// <summary>
    ///     Gets or sets the game process tracked by the script.
    /// </summary>
    public Process? Game
    {
        get => _aslScriptGetGame(_script);
        set => _aslScriptSetGame(_script, value);
    }

    private static readonly UnsafeAccessor.GetField<ASLScript, Process?> _aslScriptGetGame =
        UnsafeAccessor.CreateFieldGetter<ASLScript, Process?>("_game");

    private static readonly UnsafeAccessor.SetField<ASLScript, Process?> _aslScriptSetGame =
        UnsafeAccessor.CreateFieldSetter<ASLScript, Process?>("_game");

    /// <summary>
    ///     Attempts to identify the calling ASL script and build its context.
    /// </summary>
    /// <param name="logger">The logger to write attach diagnostics to.</param>
    /// <param name="state">The LiveSplit timer state to search for ASL components.</param>
    /// <returns>
    ///     The script-side context;
    /// otherwise, an <see cref="AttachError"/>.
    /// </returns>
    internal static Result<ScriptContext> Find(Logger logger, LiveSplitState state)
    {
        if (!ScriptCaller.FindCallingModule(logger)
            .TryUnwrap(out Module? callingModule, out var callerError))
        {
            return Result.Err<ScriptContext>(callerError);
        }

        try
        {
            List<string> inspected = [];
            List<(ASLComponent Component, IReadOnlyDictionary<Module, string> Actions)> candidates = [];

            using (logger.BeginScopeTrace("Inspecting loaded ASL components..."))
            {
                foreach (ASLComponent component in EnumerateComponents(state))
                {
                    var settings = component.GetFieldValue<ComponentSettings>("_settings")!;
                    inspected.Add(settings.ScriptPath);

                    if (component.Script is not { } script)
                    {
                        logger.LogTrace($"'{settings.ScriptPath}': no script loaded; skipping.");
                        continue;
                    }

                    var methods = script.GetFieldValue<ASLScript.Methods>("_methods")!;
                    var actionModules = ScriptActions.GetActionModules(methods);

                    logger.LogTrace($"'{settings.ScriptPath}': {actionModules.Count} compiled action(s).");
                    candidates.Add((component, actionModules));
                }
            }

#pragma warning disable CA2000 // Dispose objects before losing scope (ASLComponent)
            if (!ScriptResolver.TryMatch(callingModule, candidates, out ASLComponent? owner, out string? action))
#pragma warning restore CA2000
            {
                return AttachError.ScriptComponentNotFound(inspected);
            }

            logger.LogDebug($"Calling module belongs to the '{action}' action.");

            if (action != "startup")
            {
                return AttachError.OutsideStartup(action);
            }

            var ownerScript = owner.Script;
            var ownerSettings = owner.GetFieldValue<ComponentSettings>("_settings")!;
            var ownerMethods = ownerScript.GetFieldValue<ASLScript.Methods>("_methods")!;
            var ownerAslSettings = ownerScript.GetFieldValue<ASLSettings>("_settings")!;

            if (ownerAslSettings.Builder is not { } builder)
            {
                return AttachError.LiveSplitInternalsChanged("ASLSettings.Builder");
            }

            ScriptActions actions = ScriptActions.Parse(logger, ownerSettings.ScriptPath, ownerMethods);

            logger.LogDebug($"Resolved script '{ownerSettings.ScriptPath}'.");
            return new ScriptContext(
                ownerScript, ownerSettings.ScriptPath, action, actions, new SettingsBuilder(builder));
        }
        catch (MissingFieldException ex)
        {
            return AttachError.LiveSplitInternalsChanged(ex.Message);
        }
    }

    private static IEnumerable<ASLComponent> EnumerateComponents(LiveSplitState state)
    {
        if (state.Run.AutoSplitter?.Component is ASLComponent autoSplitter)
        {
            yield return autoSplitter;
        }

        foreach (IComponent component in state.Layout.Components)
        {
            if (component is ASLComponent asl)
            {
                yield return asl;
            }
        }
    }
}
