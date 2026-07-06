using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AslHelp.Logging;

/// <summary>
///     Routes log events to a set of <see cref="Sinks"/> and tracks per-call-context scope
///     nesting for indentation.
/// </summary>
public sealed class Logger : IDisposable
{
    private const int Levels = (int)LogLevel.Off + 1;

    private readonly AsyncLocal<IndentFrame?> _indent = new();
    private readonly bool _ownsSinks;

    private bool _disposed;

    /// <summary>
    ///     Creates a logger that owns its sinks and disposes them with it.
    /// </summary>
    public Logger()
        : this([], ownsSinks: true) { }

    private Logger(ICollection<ILogSink> sinks, bool ownsSinks)
    {
        _ownsSinks = ownsSinks;

        Sinks = sinks;
    }

    /// <summary>
    ///     Gets the sinks events are routed to.
    /// </summary>
    public ICollection<ILogSink> Sinks { get; }

    /// <summary>
    ///     Returns whether any sink would write an event at <paramref name="level"/>.
    /// </summary>
    /// <param name="level">The level to test.</param>
    /// <returns>
    ///     <see langword="true"/> if at least one sink accepts the level.
    /// </returns>
    public bool IsEnabled(LogLevel level)
    {
        if (level == LogLevel.Off)
        {
            return false;
        }

        foreach (var sink in Sinks)
        {
            if (level >= sink.MinimumLevel && sink.MinimumLevel != LogLevel.Off)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Opens a nesting scope at <paramref name="level"/>; disposing the returned scope closes
    ///     it. Subsequent events indent by one level per sink that writes <paramref name="level"/>.
    /// </summary>
    /// <param name="level">The level the scope nests for.</param>
    /// <returns>
    ///     A scope that restores the previous nesting when disposed.
    /// </returns>
    public IndentScope BeginScope(LogLevel level)
    {
        var current = _indent.Value;
        var next = IndentFrame.Push(current, level);

        _indent.Value = next;
        return new IndentScope(this, current);
    }

    /// <summary>
    ///     Routes <paramref name="e"/> to every sink that accepts its level, indented by the
    ///     current scope depth as seen by each sink.
    /// </summary>
    /// <param name="e">The event to route.</param>
    public void Log(in LogEvent e)
    {
        if (e.Level == LogLevel.Off)
        {
            return;
        }

        var frame = _indent.Value;
        foreach (var sink in Sinks)
        {
            if (e.Level < sink.MinimumLevel || sink.MinimumLevel == LogLevel.Off)
            {
                continue;
            }

            var depth = frame is null ? 0 : frame.Counts[(int)sink.MinimumLevel];
            sink.Emit(e, depth);
        }
    }

    internal void PopFrame(object? parentFrame)
    {
        _indent.Value = (IndentFrame?)parentFrame;
    }

    /// <summary>
    ///     Disposes the sinks the logger owns.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_ownsSinks)
        {
            foreach (var sink in Sinks.OfType<IDisposable>())
            {
                sink.Dispose();
            }
        }
    }

    internal sealed class IndentFrame
    {
        private IndentFrame(int[] counts)
        {
            Counts = counts;
        }

        public int[] Counts { get; }

        public static IndentFrame Push(IndentFrame? current, LogLevel level)
        {
            var counts = new int[Levels];
            if (current is not null)
            {
                Array.Copy(current.Counts, counts, Levels);
            }

            var top = (int)level;
            for (var frame = 0; frame <= top; frame++)
            {
                counts[frame]++;
            }

            return new IndentFrame(counts);
        }
    }
}
