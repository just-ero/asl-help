using AslHelp.LiveSplit;
using AslHelp.LiveSplit.Diagnostics;
using AslHelp.Shared.Logging;

public delegate void Log(object? output);
public delegate void LogFormat(string format, params object?[] args);

public partial class Basic
{
    protected override void InitializePlugin()
    {
        _logger.Add(new TraceLogger());
    }

    protected override void GenerateCode(Autosplitter asl)
    {
        asl.Vars["Helper"] = this;
        AslDebug.Info("Set helper to vars.Helper.");

        asl.Vars["Log"] = (Log)log;
        AslDebug.Info("Created `vars.Log(object? output)`.");

        asl.Vars["LogFormat"] = (LogFormat)logFormat;
        AslDebug.Info("Created `vars.LogFormat(string format, params object?[] args)`.");

        asl.Actions.Exit.Prepend($"vars.Helper.{nameof(Dispose)}();");
        asl.Actions.Shutdown.Prepend($"vars.Helper.{nameof(Dispose)}();");

        void log(object? output)
        {
            string msg = $"[{GameName}] {output}";
            _logger.Log(msg);
        }

        void logFormat(string format, params object?[] args)
        {
            string msg = $"[{GameName}] {string.Format(format, args)}";
            _logger.Log(msg);
        }
    }
}
