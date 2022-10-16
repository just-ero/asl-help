namespace AslHelp.IO.Logging;

public class DebugLogger : Logger
{
    public override void Start()
    {
        throw new NotImplementedException();
    }

    public override void Log()
    {
        LiveSplit.Options.Log.Info("");
    }

    public override void Log(object output)
    {
        LiveSplit.Options.Log.Info(output.ToString());
    }

    public override void Stop()
    {
        throw new NotImplementedException();
    }
}
