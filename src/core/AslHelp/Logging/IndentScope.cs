using System;

namespace AslHelp.Logging;

/// <summary>
///     The handle returned by <see cref="Logger.BeginScope"/>; disposing it restores the logger's
///     previous nesting frame.
/// </summary>
/// <param name="logger">The logger whose nesting this scope restores.</param>
/// <param name="parentFrame">The frame to restore on dispose.</param>
public readonly struct IndentScope(Logger logger, object? parentFrame) : IDisposable
{
    /// <summary>
    ///     Restores the logger's nesting to the frame in effect before this scope opened.
    /// </summary>
    public void Dispose()
    {
        logger?.PopFrame(parentFrame);
    }
}
