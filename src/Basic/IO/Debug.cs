using AslHelp.IO.Logging;

namespace AslHelp.IO;

internal static class Debug
{
    private static readonly DebugLogger _logger = new();

    public static void Info()
    {
        _logger.Log("[asl-help]");
    }

    public static void Info(object output)
    {
        _logger.Log($"[asl-help] {output}");
    }

    public static void Warn(object output)
    {
        Info($"[WARNING] {output}");
    }

    public static void Error(object output)
    {
        Info($"[ERROR] {output}");
    }

    public static void Throw(Exception ex)
    {
        Info("Aborting due to error." + Environment.NewLine + ex);
    }

    public static IEnumerable<string> Trace
    {
        get
        {
            StackFrame[] frames = new StackTrace().GetFrames();

            for (int i = 0; i < frames.Length; i++)
            {
                MethodBase method = frames[i].GetMethod();
                Type decl = method.DeclaringType;
                string ret = decl is null ? method.Name : $"{decl.Name}.{method.Name}";

                yield return ret;
            }
        }
    }

    public static bool TraceIncludes(params string[] methodNames)
    {
        return Trace.Any(t => methodNames.Any(m => m.Equals(t, StringComparison.OrdinalIgnoreCase)));
    }
}
