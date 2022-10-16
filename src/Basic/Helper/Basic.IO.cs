using AslHelp.Data.AutoSplitter;
using AslHelp.IO;
using AslHelp.IO.Logging;
using AslHelp.IO.Texts;

public partial class Basic
{
    protected DebugLogger _dbgLogger = new();
    protected FileLogger _fileLogger;

    public TimerControl Timer { get; } = new();
    public SettingsCreator Settings { get; } = new();
    public TextComponentManager Texts { get; } = new();

    public void StartFileLogger(string filePath, int maximumLinesInFile = 4096, int amountOfLinesToFlush = 512)
    {
        _fileLogger?.Stop();

        if (Actions.Current != "startup")
        {
            string msg = "[IO] The file logger may not be started outside of the 'startup {}' action.";
            throw new InvalidOperationException(msg);
        }

        if (maximumLinesInFile < 1)
        {
            string msg = "[IO] The file logger must allow at least 1 line.";
            throw new ArgumentOutOfRangeException(msg);
        }

        if (amountOfLinesToFlush < 1)
        {
            string msg = "[IO] The file logger must erase at least 1 line.";
            throw new ArgumentOutOfRangeException(msg);
        }

        if (amountOfLinesToFlush > maximumLinesInFile)
        {
            string msg = "[IO] The file logger's maximum lines must be greater than the amount of lines to erase.";
            throw new InvalidOperationException(msg);
        }

        try
        {
            _fileLogger = new(filePath, maximumLinesInFile, amountOfLinesToFlush);
            _fileLogger.Start();
        }
        catch (Exception ex)
        {
            Debug.Warn($"[IO] Was unable to create the file logger:\n{ex}");
        }
    }

    protected void StartBench(string id)
    {
        _dbgLogger.StartBenchmark(id);
        _fileLogger?.StartBenchmark(id);
    }

    protected void StopBench(string id)
    {
        _dbgLogger.StopBenchmark(id);
        _fileLogger?.StopBenchmark(id);
    }

    private void Log(object output)
    {
        _dbgLogger.Log($"[{GameName}] {output}");
        _fileLogger?.Log($"[{GameName}] {output}");
    }
}
