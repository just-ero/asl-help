using AslHelp.Logging;
using LiveSplit.Model;
using LiveSplit.View;
using System.Diagnostics;
using System.Windows.Forms;

namespace AslHelp.LiveSplit;

/// <summary>
///     Provides access to the running LiveSplit instance's timer state and controls.
/// </summary>
[DebuggerDisplay("phase = {State.CurrentPhase}")]
public sealed class LiveSplitContext
{
    internal LiveSplitContext(LiveSplitState state)
    {
        State = state;
    }

    /// <summary>
    ///     Gets the LiveSplit timer state.
    /// </summary>
    public LiveSplitState State { get; }

    /// <summary>
    ///     Attempts to locate the running LiveSplit instance via its open timer form.
    /// </summary>
    /// <param name="logger">The logger to write attach diagnostics to.</param>
    /// <returns>
    ///     The timer-side context; otherwise, an <see cref="AttachError"/>.
    /// </returns>
    internal static Result<LiveSplitContext> Find(Logger logger)
    {
        using (logger.BeginScopeTrace($"Searching {Application.OpenForms.Count} open form(s) for '{nameof(TimerForm)}'..."))
        {
            if (Application.OpenForms[nameof(TimerForm)] is not TimerForm timerForm)
            {
                return AttachError.TimerFormNotFound();
            }

            logger.LogDebug("Found LiveSplit's timer form.");
            return new LiveSplitContext(timerForm.CurrentState);
        }
    }
}
