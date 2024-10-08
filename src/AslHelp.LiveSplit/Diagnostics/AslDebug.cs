using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using AslHelp.Diagnostics.Logging;

namespace AslHelp.LiveSplit.Diagnostics;

[Obsolete("Do not use ASL-specific features.", true)]
public static class AslDebug
{
    private static readonly TraceLogger _logger = new();
    private static int _indentLevel;

    public static void Info(object? output)
    {
        _logger.Log($"[asl-help] [Info]{Prefix}{output}");
    }

    public static void Warn(object? output)
    {
        _logger.Log($"[asl-help] [Warn]{Prefix}{output}");
    }

    public static void Error(object? output)
    {
        _logger.Log($"[asl-help] [Error]{Prefix}{output}");
    }

    public static IEnumerable<string> StackTraceMethodNames
    {
        get
        {
            return new StackTrace(false)
                .GetFrames()
                .Select(frame =>
                {
                    MethodBase? method = frame.GetMethod();
                    Type? decl = method?.DeclaringType;

                    return decl is null
                        ? $"{method?.Name}"
                        : $"{decl.FullName}.{method?.Name}";
                });
        }
    }

    public static IDisposable Indent()
    {
        return new Indenter();
    }

    public static IDisposable Indent(string info)
    {
        Info(info);
        return new Indenter();
    }

    private static string Prefix
    {
        get
        {
            if (_indentLevel > 0)
            {
                return $"{new string(' ', _indentLevel * 2)} => ";
            }
            else
            {
                return " ";
            }
        }
    }

    private sealed class Indenter : IDisposable
    {
        public Indenter()
        {
            _indentLevel++;
        }

        public void Dispose()
        {
            _indentLevel--;
        }
    }
}
