using System.Text;

namespace AslHelp.Logging;

/// <summary>
///     Renders a <see cref="LogEvent"/> into the line of text a sink writes.
/// </summary>
/// <param name="e">The event to render.</param>
/// <param name="indentLevel">The number of enclosing scopes to indent by.</param>
/// <returns>
///     The formatted line.
/// </returns>
public delegate string LogFormatter(
    in LogEvent e,
    int indentLevel);

/// <summary>
///     Provides the built-in <see cref="LogFormatter"/>.
/// </summary>
public static class LogFormatterExtensions
{
    extension(LogFormatter)
    {
        /// <summary>
        ///     Renders the event as <c>[level] {indent}{message}</c>, with a four-space indent per
        ///     scope level.
        /// </summary>
        /// <param name="e">The event to render.</param>
        /// <param name="indentLevel">The number of enclosing scopes to indent by.</param>
        /// <returns>
        ///     The formatted line.
        /// </returns>
        public static string Default(in LogEvent e, int indentLevel)
        {
            var level = e.Level switch
            {
                LogLevel.Trace => "trce",
                LogLevel.Debug => "dbug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warn",
                LogLevel.Error => "fail",
                LogLevel.Critical => "crit",
                _ => "    "
            };

            return new StringBuilder()
                .Append('[').Append(level).Append("] ")
                .Append(' ', indentLevel * 4)
                .Append(e.Message)
                .ToString();
        }
    }
}
