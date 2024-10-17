extern alias Ls;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using AslHelp.Shared.Extensions;
using AslHelp.Shared.Results;

using Ls::LiveSplit.ASL;
using Ls::LiveSplit.Model;
using Ls::LiveSplit.UI.Components;
using Ls::LiveSplit.View;

namespace AslHelp.LiveSplit;

public sealed partial class Autosplitter
{
    public static Result<Autosplitter> Initialize()
    {
        return GetTimerData()
            .AndThen(state =>
            {
                return GetScriptData(state)
                    .Map(scriptData => (State: state, Component: scriptData.Component, Script: scriptData.Script, Methods: scriptData.Methods));
            })
            .AndThen<Autosplitter>(res =>
            {
                ASLSettings? settings = res.Script.GetFieldValue<ASLSettings>("_settings");
                if (settings?.Builder is not ASLSettingsBuilder builder)
                {
                    return LiveSplitInitializationError.ScriptSettingsInvalid;
                }

                ScriptActions actions = ParseActions(res.Component, res.Methods);

                return new Autosplitter(res.State, res.Script, actions, builder);
            });
    }

    private static Result<LiveSplitState> GetTimerData()
    {
        if (Application.OpenForms[nameof(TimerForm)] is not TimerForm timerForm)
        {
            return LiveSplitInitializationError.TimerFormNotFound;
        }

        if (timerForm.CurrentState is not LiveSplitState state)
        {
            return LiveSplitInitializationError.LiveSplitStateInvalid;
        }

        return state;
    }

    private static Result<(ASLComponent Component, ASLScript Script, ASLScript.Methods Methods)> GetScriptData(LiveSplitState state)
    {
        Assembly? scriptAssembly = ReflectionExtensions.AssemblyTrace
            .FirstOrDefault(asm => asm.GetType("CompiledScript") is not null);

        if (scriptAssembly is null)
        {
            return LiveSplitInitializationError.ScriptAssemblyNotFound;
        }

        IEnumerable<IComponent?> components = state.Layout.Components.Prepend(state.Run.AutoSplitter?.Component);

        ASLComponent? component = default;
        ASLScript? script = default;
        ASLScript.Methods? methods = default;

        foreach (ASLComponent c in components.OfType<ASLComponent>())
        {
            script = c.Script;
            methods = script?.GetFieldValue<ASLScript.Methods>("_methods");

            if (methods?.startup is not { IsEmpty: false } startup)
            {
                continue;
            }

            object? cc = startup.GetFieldValue<object>("_compiled_code");
            if (cc?.GetType().Assembly == scriptAssembly)
            {
                component = c;
                break;
            }
        }

        if (component is null || script is null || methods is null)
        {
            return LiveSplitInitializationError.ScriptComponentNotFound;
        }

        return (component, script, methods);
    }
}
