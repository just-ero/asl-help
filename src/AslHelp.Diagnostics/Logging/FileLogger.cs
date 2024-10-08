using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AslHelp.Shared;

namespace AslHelp.Diagnostics.Logging;

/// <summary>
///     Provides an <see cref="ILogger"/> that writes to a specified file.
/// </summary>
public sealed class FileLogger : ILogger
{
    private readonly StreamWriter _writer;

    private readonly Queue<string> _queuedLines = new();
    private CancellationTokenSource _cancelSource = new();
    private readonly ManualResetEvent _resetEvent = new(false);

    private int _lineNumber;
    private readonly int _maximumLines;
    private readonly int _linesToErase;

    private bool _isRunning;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileLogger"/> class
    ///     with the specified file path, the maximum amount of lines to write to the file
    ///     and the amount of lines to erase when the maximum amount of lines is reached.
    /// </summary>
    /// <param name="fileName">The path of the file to write to.</param>
    /// <param name="maximumLines">The maximum amount of lines to write to the file.</param>
    /// <param name="linesToErase">The amount of lines to erase when the maximum amount of lines is reached.</param>
    public FileLogger(string fileName, int maximumLines, int linesToErase)
    {
        ThrowHelper.ThrowIfLessThan(
            maximumLines, 1,
            "The file logger must allow at least 1 line.");

        ThrowHelper.ThrowIfLessThan(
            linesToErase, 1,
            "File logger must erase at least 1 line.");

        ThrowHelper.ThrowIfLargerThan(
            linesToErase, maximumLines,
            "The file logger's maximum lines must be greater or equal to the amount of lines to erase.");

        _writer = new(fileName, true);
        _maximumLines = maximumLines;
        _linesToErase = linesToErase;

        FilePath = Path.GetFullPath(fileName);
    }

    /// <summary>
    ///     Gets the path of the file to write to.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    ///     Starts the <see cref="FileLogger"/>.
    /// </summary>
    public void Start()
    {
        if (_isRunning)
        {
            return;
        }

        // We simply fire and forget and wait for new lines to be added to the queue.
        _ = Task.Run(() =>
        {
            _isRunning = true;
            _cancelSource = new();
            _lineNumber = 0;

            if (!File.Exists(FilePath))
            {
                File.Create(FilePath).Dispose();
            }
            else
            {
                using StreamReader reader = new(FilePath);

                while (!_cancelSource.IsCancellationRequested && reader.ReadLine() is not null)
                {
                    _lineNumber++;
                }
            }

            string output;

            while (true)
            {
                _ = _resetEvent.WaitOne();
                if (_cancelSource.IsCancellationRequested)
                {
                    break;
                }

                lock (_queuedLines)
                {
                    if (!_queuedLines.Any())
                    {
                        _ = _resetEvent.Reset();
                        continue;
                    }

                    output = _queuedLines.Dequeue();
                }

                WriteLine(output);
            }
        }, _cancelSource.Token);
    }

    /// <summary>
    ///     Writes an empty line to the log.
    /// </summary>
    public void Log()
    {
        if (!_isRunning)
        {
            const string Msg = "Logger is not running.";
            ThrowHelper.ThrowInvalidOperationException(Msg);
        }

        lock (_queuedLines)
        {
            _queuedLines.Enqueue("");
            _ = _resetEvent.Set();
        }
    }

    /// <summary>
    ///     Writes the string representation of the specified value to the log.
    /// </summary>
    /// <param name="output">The value to log.</param>
    public void Log(object? output)
    {
        if (!_isRunning)
        {
            const string Msg = "Logger is not running.";
            ThrowHelper.ThrowInvalidOperationException(Msg);
        }

        lock (_queuedLines)
        {
            _queuedLines.Enqueue($"[{DateTime.Now:dd-MMM-yy HH:mm:ss.fff}] :: {output}");
            _ = _resetEvent.Set();
        }
    }

    /// <summary>
    ///     Terminates the <see cref="FileLogger"/>.
    /// </summary>
    public void Stop()
    {
        if (!_isRunning)
        {
            return;
        }

        _cancelSource.Cancel();
        _ = _resetEvent.Set();
        _queuedLines.Clear();
    }

    private void WriteLine(string output)
    {
        if (_lineNumber >= _maximumLines)
        {
            FlushLines();
        }

        try
        {
            _writer.WriteLine(output);
            _lineNumber++;
        }
        catch { }
    }

    private void FlushLines()
    {
        string tempFile = $"{FilePath}-temp";
        var lines = File.ReadAllLines(FilePath).Skip(_linesToErase);

        File.WriteAllLines(tempFile, lines);

        try
        {
            File.Copy(tempFile, FilePath, true);
            _lineNumber -= _linesToErase;
        }
        catch { }
        finally
        {
            try
            {
                File.Delete(tempFile);
            }
            catch { }
        }
    }
}
