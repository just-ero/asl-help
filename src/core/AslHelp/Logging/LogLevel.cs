namespace AslHelp.Logging;

/// <summary>
///     The severity of a <see cref="LogEvent"/>, ordered from most to least verbose.
/// </summary>
public enum LogLevel
{
    /// <summary>
    ///     The most verbose level, for fine-grained diagnostic detail.
    /// </summary>
    Trace,

    /// <summary>
    ///     Detail useful while debugging.
    /// </summary>
    Debug,

    /// <summary>
    ///     The normal flow of the application.
    /// </summary>
    Information,

    /// <summary>
    ///     An abnormal or unexpected event that does not stop the application.
    /// </summary>
    Warning,

    /// <summary>
    ///     A failure in the current operation.
    /// </summary>
    Error,

    /// <summary>
    ///     A failure that requires immediate attention.
    /// </summary>
    Critical,

    /// <summary>
    ///     Disables logging; no event at this level is emitted.
    /// </summary>
    Off
}
