using System.Diagnostics;

namespace AslHelp.Logging;

/// <summary>
///     A sink that writes formatted events to <see cref="Trace"/> (e.g. the debugger's output
///     window via <c>OutputDebugString</c>).
/// </summary>
public sealed class OdsSink : ILogSink
{
    private readonly LogFormatter _formatter;

    /// <summary>
    ///     Creates a sink writing events at or above <paramref name="minimumLevel"/>.
    /// </summary>
    /// <param name="minimumLevel">The lowest severity to write.</param>
    /// <param name="formatter">The formatter to use, or <see langword="null"/> for the default.</param>
    public OdsSink(LogLevel minimumLevel, LogFormatter? formatter = null)
    {
        MinimumLevel = minimumLevel;
        _formatter = formatter ?? LogFormatter.Default;
    }

    /// <inheritdoc/>
    public LogLevel MinimumLevel { get; }

    /// <inheritdoc/>
    public void Emit(in LogEvent e, int indentLevel)
    {
        Trace.WriteLine(_formatter(in e, indentLevel));
    }
}
