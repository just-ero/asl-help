namespace AslHelp.Diagnostics.Logging;

public interface ILogger
{
    void Start();
    void Stop();
    void Log();
    void Log(object? output);
}
