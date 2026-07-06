using AslHelp.Logging;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AslHelp.LiveSplit.Asl.Attach;

/// <summary>
///     Identifies the compiled ASL script module that asl-help was loaded from.
/// </summary>
internal static class ScriptCaller
{
    private const string CompiledScriptTypeName = "CompiledScript";

    /// <summary>
    ///     Walks the current call stack for the first compiled script frame.
    /// </summary>
    /// <param name="logger">The logger to write diagnostics to.</param>
    /// <returns>
    ///     The <see cref="Module"/> of the calling compiled script; otherwise, an <see cref="AttachError"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Result<Module> FindCallingModule(Logger logger)
    {
        StackTrace trace = new();
        using (logger.BeginScopeTrace($"Searching {trace.FrameCount} stack frames for a compiled script..."))
        {

            for (var i = 0; i < trace.FrameCount; i++)
            {
                var method = trace.GetFrame(i)?.GetMethod();
                logger.LogTrace(
                    $"[{i}] {method?.DeclaringType?.FullName ?? "<global>"}.{method?.Name ?? "<unknown>"}");

                if (method?.DeclaringType is { Name: CompiledScriptTypeName })
                {
                    logger.LogDebug($"Found compiled script frame at [{i}] ('{method.Name}').");
                    return Result.Ok(method.Module);
                }
            }

            return AttachError.NotCalledFromScript();
        }
    }
}
