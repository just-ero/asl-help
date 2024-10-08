using System;
using System.Linq;

using AslHelp.LiveSplit.Diagnostics;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    protected abstract void ExitPlugin();
    protected abstract void ShutdownPlugin(bool closing);

    public AslPluginBase Exit()
    {
        EnsureInitialized();

        using (AslDebug.Indent("Disposing..."))
        {
            ExitPlugin();
            FreeMemory();
        }

        return this;
    }

    public AslPluginBase Shutdown()
    {
        EnsureInitialized();

        using (AslDebug.Indent("Disposing..."))
        {
            AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
            AppDomain.CurrentDomain.FirstChanceException -= FirstChanceHandler;

            bool closing = AslDebug.StackTraceMethodNames.Any(name =>
                name is "TimerForm.TimerForm_FormClosing"
                or "TimerForm.OpenLayoutFromFile"
                or "TimerForm.LoadDefaultLayout");

            ShutdownPlugin(closing);
            FreeMemory();
        }

        return this;
    }

    private static void FreeMemory()
    {
        long memoryBefore = GC.GetTotalMemory(false);
        GC.Collect();
        long memoryAfter = GC.GetTotalMemory(true);

        AslDebug.Info($"Freed {(memoryBefore - memoryAfter) / 1000:n0} KB of memory.");
    }
}
