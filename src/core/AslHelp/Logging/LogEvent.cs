namespace AslHelp.Logging;

/// <summary>
///     A single log entry: its message, severity, and the call site that produced it.
/// </summary>
/// <param name="Level">The severity of the event.</param>
/// <param name="Message">The message text.</param>
/// <param name="CallerMemberName">The member that produced the event.</param>
/// <param name="CallerFilePath">The source file that produced the event.</param>
/// <param name="CallerLineNumber">The source line that produced the event.</param>
public readonly record struct LogEvent(
    LogLevel Level,
    string Message,
    string CallerMemberName,
    string CallerFilePath,
    int CallerLineNumber);
