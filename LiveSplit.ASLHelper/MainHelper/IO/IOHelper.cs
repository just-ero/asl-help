namespace ASLHelper.MainHelper;

public class IOHelper
{
    private FileLogger _fileLogger;

    internal IOHelper() { }

    public void Log(object output = null)
    {
        LiveSplit.Options.Log.Info($"[{Main.Instance.GameName}] {output}");
        _fileLogger?.Log(output);
    }

    public void StartFileLogger(string path)
    {
        if (Main.Instance.Game is not null)
        {
            Debug.Warn("[IO] A new file logger may not be started outside of the 'startup {}' action!");
            return;
        }

        _fileLogger = new(path);
    }

    public void Dispose()
    {
        _fileLogger?.Writer.Close();
        _fileLogger?.Writer.Dispose();
    }
}
