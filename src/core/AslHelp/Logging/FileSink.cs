using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AslHelp.Logging;

/// <summary>
///     A sink that writes formatted events to a file on a background thread, trimming the oldest
///     lines once the file grows past <c>maximumLines</c>.
/// </summary>
public sealed class FileSink : ILogSink, IDisposable
{
    private readonly LogFormatter _formatter;

    private readonly BlockingCollection<string> _queue = [];
    private readonly Task _writerTask;

    private readonly int _maximumLines;
    private readonly int _linesToErase;
    private int _currentLines;

    private bool _disposed;

    /// <summary>
    ///     Creates a sink that appends to <paramref name="fileName"/>, seeding its line count from
    ///     the existing file, and starts its background writer.
    /// </summary>
    /// <param name="fileName">The path of the log file; created if it does not exist.</param>
    /// <param name="minimumLevel">The lowest severity to write.</param>
    /// <param name="maximumLines">The line count at which the file is trimmed.</param>
    /// <param name="linesToErase">The number of oldest lines removed per trim.</param>
    /// <param name="formatter">The formatter to use, or <see langword="null"/> for the default.</param>
    /// <exception cref="ArgumentOutOfRangeException">A line count is less than one, or <paramref name="maximumLines"/> is below <paramref name="linesToErase"/>.</exception>
    public FileSink(
        string fileName,
        LogLevel minimumLevel,
        int maximumLines = 4096,
        int linesToErase = 512,
        LogFormatter? formatter = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maximumLines, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(linesToErase, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(maximumLines, linesToErase);

        _formatter = formatter ?? LogFormatter.Default;

        _maximumLines = maximumLines;
        _linesToErase = linesToErase;

        FileName = Path.GetFullPath(fileName);
        MinimumLevel = minimumLevel;

        if (File.Exists(FileName))
        {
            using var reader = new StreamReader(FileName);
            while (reader.ReadLine() is not null)
            {
                _currentLines++;
            }
        }
        else
        {
            File.Create(FileName).Dispose();
        }

        _writerTask = Task.Run(EmitLoop);
    }

    /// <summary>
    ///     Gets the full path of the log file.
    /// </summary>
    public string FileName { get; }

    /// <inheritdoc/>
    public LogLevel MinimumLevel { get; }

    /// <summary>
    ///     Formats <paramref name="e"/> and queues it for the background writer.
    /// </summary>
    /// <param name="e">The event to write.</param>
    /// <param name="indentLevel">The number of enclosing scopes to indent by.</param>
    /// <exception cref="ObjectDisposedException">The sink has been disposed.</exception>
    public void Emit(in LogEvent e, int indentLevel)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var message = _formatter(in e, indentLevel);

        try
        {
            _queue.Add(message);
        }
        catch (InvalidOperationException) { }
    }

#pragma warning disable CA1031 // Do not catch general exception types
#pragma warning disable CA2000 // Dispose objects before losing scope
    private void EmitLoop()
    {
        StreamWriter? writer = null;
        try
        {
            foreach (var line in _queue.GetConsumingEnumerable())
            {
                try
                {
                    writer ??= OpenWriter();

                    if (_currentLines >= _maximumLines)
                    {
                        writer.Dispose();
                        writer = null;

                        EraseLines();

                        writer = OpenWriter();
                    }

                    writer.WriteLine(line);
                    _currentLines++;
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Failed to write log entry to file: {ex}");

                    try
                    {
                        writer?.Dispose();
                    }
                    catch (Exception disposeEx)
                    {
                        Trace.TraceError($"Failed to dispose log file writer: {disposeEx}");
                    }

                    writer = null;
                }
            }
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Unexpected error in log writer loop: {ex}");
        }
        finally
        {
            try
            {
                writer?.Dispose();
            }
            catch (Exception disposeEx)
            {
                Trace.TraceError($"Failed to dispose log file writer in finalizer: {disposeEx}");
            }
        }
    }
#pragma warning restore CA1031, CA2000

    private StreamWriter OpenWriter()
    {
        return new StreamWriter(FileName, append: true) { AutoFlush = true };
    }

    private void EraseLines()
    {
        // Sibling temp keeps the swap on the same volume so File.Replace is atomic.
        string tempFile = FileName + ".tmp";

        int kept = 0;
        using (StreamReader reader = new(FileName))
        using (StreamWriter writer = new(tempFile, append: false))
        {
            int skipped = 0;
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                if (skipped < _linesToErase)
                {
                    skipped++;
                    continue;
                }

                writer.WriteLine(line);
                kept++;
            }
        }

        File.Replace(tempFile, FileName, destinationBackupFileName: null);
        _currentLines = kept;
    }

    /// <summary>
    ///     Stops accepting events and waits briefly for the background writer to drain the queue.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _queue.CompleteAdding();

        try
        {
            _writerTask.Wait(TimeSpan.FromSeconds(2));
        }
        catch (AggregateException ex) when (ex.InnerExceptions is [OperationCanceledException]) { }

        _queue.Dispose();
    }
}
