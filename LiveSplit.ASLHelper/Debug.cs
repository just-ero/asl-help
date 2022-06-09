using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ASLHelper
{
    internal static class Debug
    {
        public static void Log()
        {
            LiveSplit.Options.Log.Info("");
        }

        public static void Log(object output)
        {
            LiveSplit.Options.Log.Info($"[ASL Helper] {output}");
        }

        public static void Warn(object output)
        {
            LiveSplit.Options.Log.Info($"[ASL Helper Warning] {output}");
        }

        public static void Throw(Exception ex)
        {
            Log("Aborting due to error!" + Environment.NewLine + ex);
        }

        public static IEnumerable<string> Trace()
        {
            var frames = new StackTrace().GetFrames();

            for (int i = frames.Length - 1; i >= 0; i--)
                yield return frames[i].GetMethod().Name;
        }

        public static bool TraceIncludes(params string[] methodNames)
        {
            return Trace().Any(t => methodNames.Any(m => m.Equals(t, StringComparison.OrdinalIgnoreCase)));
        }
    }
}