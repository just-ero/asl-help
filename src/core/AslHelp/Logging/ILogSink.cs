namespace AslHelp.Logging;

/// <summary>
///     A destination that writes log events meeting its <see cref="MinimumLevel"/>.
/// </summary>
public interface ILogSink
{
    /// <summary>
    ///     Gets the lowest severity this sink writes; events below it are dropped.
    /// </summary>
    LogLevel MinimumLevel { get; }

    /// <summary>
    ///     Writes <paramref name="e"/>, indented by <paramref name="indentLevel"/> scope levels.
    /// </summary>
    /// <param name="e">The event to write.</param>
    /// <param name="indentLevel">The number of enclosing scopes to indent by.</param>
    void Emit(in LogEvent e, int indentLevel);
}
