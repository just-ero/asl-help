namespace AslHelp.IO.Logging;

public class FileLogger : Logger
{
    private readonly string _filePath;
    private readonly Queue<string> _queuedLines = new();
    private readonly CancellationTokenSource _cancelSource = new();
    private readonly ManualResetEvent _resetEvent = new(false);

    private int _lineNumber;
    private readonly int _maximumLines;
    private readonly int _linesToErase;

    public FileLogger(string path, int maximumLines, int linesToErase)
    {
        _filePath = path;
        _maximumLines = maximumLines;
        _linesToErase = linesToErase;
    }

    public override void Start()
    {
        Task.Run(() =>
        {
            _lineNumber = 0;

            if (!File.Exists(_filePath))
            {
                File.Create(_filePath).Dispose();
            }
            else
            {
                using StreamReader reader = new(_filePath);

                while (!_cancelSource.IsCancellationRequested && reader.ReadLine() is not null)
                {
                    _lineNumber++;
                }
            }

            string output;

            while (true)
            {
                _resetEvent.WaitOne();
                if (_cancelSource.IsCancellationRequested)
                {
                    break;
                }

                lock (_queuedLines)
                {
                    if (!_queuedLines.Any())
                    {
                        _resetEvent.Reset();
                        continue;
                    }

                    output = _queuedLines.Dequeue();
                }

                WriteLine(output);
            }
        }, _cancelSource.Token);
    }

    public override void Log()
    {
        lock (_queuedLines)
        {
            _queuedLines.Enqueue("");
            _resetEvent.Set();
        }
    }

    public override void Log(object output)
    {
        lock (_queuedLines)
        {
            _queuedLines.Enqueue($"[{DateTime.Now:dd-MMM-yy HH:mm:ss.fff}] :: {output}");
            _resetEvent.Set();
        }
    }

    public override void Stop()
    {
        _cancelSource.Cancel();
        _resetEvent.Set();
    }

    private void WriteLine(string output)
    {
        if (_lineNumber >= _maximumLines)
        {
            FlushLines();
        }

        try
        {
            using StreamWriter writer = new(_filePath, true);

            writer.WriteLine(output);
            _lineNumber++;
        }
        catch (Exception ex)
        {
            Debug.Error($"FileLogger was unable to write to the output:\n{ex}");
        }
    }

    private void FlushLines()
    {
        string tempFile = $"{_filePath}-temp";
        string[] lines = File.ReadAllLines(_filePath).Skip(_linesToErase).ToArray();

        File.WriteAllLines(tempFile, lines);

        try
        {
            File.Copy(tempFile, _filePath, true);
            _lineNumber = lines.Length;
        }
        catch (Exception ex)
        {
            Debug.Error($"FileLogger failed replacing log file with temporary log file:\n{ex}");
        }
        finally
        {
            try
            {
                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                Debug.Error($"FileLogger failed deleting temporary log file:\n{ex}");
            }
        }
    }
}
