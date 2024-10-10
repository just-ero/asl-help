extern alias Ls;

using Ls::LiveSplit.ASL;
using Ls::LiveSplit.Model;
using Ls::LiveSplit.UI.Components;
using Ls::LiveSplit.View;

using AslHelp.Shared.Results.Errors;

namespace AslHelp.LiveSplit;

internal sealed record LiveSplitInitializationError : ResultError
{
    private LiveSplitInitializationError(string message)
        : base(message) { }

    public static LiveSplitInitializationError Other(string message)
    {
        return new(message);
    }

    public static LiveSplitInitializationError TimerFormNotFound
        => new($"An instance of type '{nameof(TimerForm)}' could not be found within the open forms of the application.");

    public static LiveSplitInitializationError LiveSplitStateInvalid
        => new($"The '{nameof(TimerForm)}' did not contain a valid instance of '{nameof(LiveSplitState)}'.");

    public static LiveSplitInitializationError ScriptAssemblyNotFound
        => new($"The compiled assembly for the executing script could not be found.");

    public static LiveSplitInitializationError ScriptComponentNotFound
        => new($"The '{nameof(ASLComponent)}' containing the executing script could not be found.");

    public static LiveSplitInitializationError ScriptSettingsInvalid
        => new($"The '{nameof(ASLScript)}' did not contain a valid instance of '{nameof(ASLSettingsBuilder)}'.");
}
