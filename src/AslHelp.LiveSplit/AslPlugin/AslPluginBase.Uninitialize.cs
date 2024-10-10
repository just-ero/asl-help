using System;
using System.Linq;

using AslHelp.LiveSplit.Diagnostics;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    protected abstract void DisposePlugin(bool closing);

    public void Dispose()
    {
        using (AslDebug.Indent("Disposing..."))
        {
            bool closing = AslDebug.StackTraceMethodNames.Any(name =>
                name is "TimerForm.TimerForm_FormClosing"
                or "TimerForm.OpenLayoutFromFile"
                or "TimerForm.LoadDefaultLayout");

            AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
            AppDomain.CurrentDomain.FirstChanceException -= FirstChanceHandler;

            DisposePlugin(closing);
            FreeMemory();
        }
    }

    private static void FreeMemory()
    {
        long memoryBefore = GC.GetTotalMemory(false);
        GC.Collect();
        long memoryAfter = GC.GetTotalMemory(true);

        AslDebug.Info($"Freed {(memoryBefore - memoryAfter) / 1000:n0} KB of memory.");
    }
}
