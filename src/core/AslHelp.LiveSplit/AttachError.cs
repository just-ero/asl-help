using System.Collections.Generic;

namespace AslHelp.LiveSplit;

/// <summary>
///     The base type for all errors produced when attaching to LiveSplit.
/// </summary>
internal sealed record AttachError : ResultError
{
    private AttachError(string message)
        : base(message) { }

    /// <summary>
    ///     No open LiveSplit timer form was found.
    /// </summary>
    public static AttachError TimerFormNotFound()
    {
        return new(
            "Could not find LiveSplit's timer form. Make sure asl-help is loaded inside a running LiveSplit instance.");
    }

    /// <summary>
    ///     No <c>CompiledScript</c> frame was found on the call stack.
    /// </summary>
    public static AttachError NotCalledFromScript()
    {
        return new(
            "asl-help must be loaded from within an ASL script (no compiled script was found on the call stack).");
    }

    /// <summary>
    ///     The calling script module did not match any loaded ASL component.
    /// </summary>
    /// <param name="candidates">
    ///     The script paths of the components that were inspected.
    /// </param>
    public static AttachError ScriptComponentNotFound(IReadOnlyList<string> candidates)
    {
        return new(
            $"The calling script does not belong to any loaded ASL component. " +
            $"Components inspected: [{string.Join(", ", candidates)}].");
    }

    /// <summary>
    ///     asl-help was loaded from an ASL action other than <c>startup</c>.
    /// </summary>
    public static AttachError OutsideStartup(string action)
    {
        return new(
            $"asl-help was loaded from the '{action}' action. Move the Assembly.Load call into 'startup'.");
    }

    /// <summary>
    ///     A LiveSplit-internal member could not be resolved via reflection (version drift).
    /// </summary>
    public static AttachError LiveSplitInternalsChanged(string member)
    {
        return new(
            $"Could not resolve LiveSplit internals ('{member}'). This LiveSplit version may be unsupported.");
    }
}
