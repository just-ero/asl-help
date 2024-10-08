using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using AslHelp.Shared.Extensions;
using AslHelp.LiveSplit.Diagnostics;

using LiveSplit.ASL;
using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.View;

namespace AslHelp.LiveSplit;

public sealed partial class Autosplitter
{
    public static bool TryInitialize([NotNullWhen(true)] out Autosplitter? autosplitter)
    {
        if (!TryGetTimerData(out LiveSplitState? state))
        {
            AslDebug.Error($"An instance of type '{nameof(TimerForm)}' could not be found within the open forms of the application.");

            autosplitter = default;
            return false;
        }

        if (!TryGetScriptData(state, out ASLComponent? component, out ASLScript? script, out ASLScript.Methods? methods))
        {
            AslDebug.Error($"The assembly or component containing the executing script could not be found.");

            autosplitter = default;
            return false;
        }

        ScriptActions actions = ParseActions(component, methods);

        ASLSettings? settings = script.GetFieldValue<ASLSettings>("_settings");
        if (settings?.Builder is not ASLSettingsBuilder builder)
        {
            AslDebug.Error($"The '{nameof(ASLScript)}' did not contain a valid instance of '{nameof(ASLSettingsBuilder)}'.");

            autosplitter = default;
            return false;
        }

        autosplitter = new(state, script, actions, builder);
        return true;
    }

    private static bool TryGetTimerData([NotNullWhen(true)] out LiveSplitState? state)
    {
        if (Application.OpenForms[nameof(TimerForm)] is not TimerForm timerForm)
        {
            state = default;
            return false;
        }

        state = timerForm.CurrentState;
        return true;
    }

    private static bool TryGetScriptData(
        LiveSplitState state,
        [NotNullWhen(true)] out ASLComponent? component,
        [NotNullWhen(true)] out ASLScript? script,
        [NotNullWhen(true)] out ASLScript.Methods? methods)
    {
        component = default;
        script = default;
        methods = default;

        Assembly? scriptAssembly = ReflectionExtensions.AssemblyTrace
            .FirstOrDefault(asm => asm.GetType("CompiledScript") is not null);

        if (scriptAssembly is null)
        {
            return false;
        }

        IEnumerable<IComponent?> components = state.Layout.Components.Prepend(state.Run.AutoSplitter?.Component);

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

        return component is not null
            && script is not null
            && methods is not null;
    }
}
