extern alias Ls;

using Ls::LiveSplit.ASL;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;

using AslHelp.LiveSplit.Diagnostics;
using AslHelp.Shared;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    // private bool _initialized;

    protected abstract void InitializePlugin();
    protected abstract void GenerateCode(Autosplitter asl);

    [MemberNotNull(nameof(_asl), nameof(Timer), nameof(Texts), nameof(Settings))]
    private void Initialize(bool generateCode)
    {
        using (AslDebug.Indent("Initializing asl-help..."))
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
            AppDomain.CurrentDomain.FirstChanceException += FirstChanceHandler;

            using (AslDebug.Indent("Initializing timer and script data..."))
            {
                _asl = Autosplitter.Initialize().UnwrapOrElse(err =>
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
                    AppDomain.CurrentDomain.FirstChanceException -= FirstChanceHandler;

                    ThrowHelper.ThrowException(err.ToString());
                    return default;
                });

                AslDebug.Info("Success.");
            }

            if (generateCode)
            {
                using (AslDebug.Indent("Generating code..."))
                {
                    GenerateCode(_asl);
                    AslDebug.Info("Success.");
                }
            }

            InitializePlugin();

            Timer = new(_asl.State);
            Texts = new(_asl.State);
            Settings = new(_asl.SettingsBuilder);

            AslDebug.Info("Success.");
        }

        // _initialized = true;
    }

    private static Assembly? AssemblyResolve(object? sender, ResolveEventArgs e)
    {
        string name = e.Name;

        int i = name.IndexOf(',');
        if (i == -1)
        {
            return default;
        }

        try
        {
            string file = $"Components/{name[..i]}.dll";
            return Assembly.LoadFrom(file);
        }
        catch
        {
            // If the file is not found or could not be loaded, ignore.
            return null;
        }
    }

    private const string CallSite_Target
        = "   at CallSite.Target(Closure , CallSite , Object , ";

    private const string UpdateDelegates_UpdateAndExecuteVoid2
        = "   at System.Dynamic.UpdateDelegates.UpdateAndExecuteVoid2[T0,T1](CallSite site, T0 arg0, T1 arg1)";

    private const string UpdateDelegates_UpdateAndExecute2
        = "   at System.Dynamic.UpdateDelegates.UpdateAndExecute2[T0,T1,TRet](CallSite site, T0 arg0, T1 arg1)";

    private const string CompiledScript_Execute
        = "   at CompiledScript.Execute(LiveSplitState timer, Object old, Object current, Object vars, Process game, Object settings)";

    private static readonly string[] _newLines = ["\r\n", "\n"];
    private static readonly FieldInfo _messageField = typeof(Exception).GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static void FirstChanceHandler(object? sender, FirstChanceExceptionEventArgs e)
    {
        if (e.Exception is not ASLRuntimeException ex)
        {
            return;
        }

        if (ex.Message is not string message)
        {
            return;
        }

        if (ex.InnerException?.StackTrace is not string stackTrace)
        {
            return;
        }

        string[] messageLines = message.Split(_newLines, StringSplitOptions.None);
        string[] stackTraceLines = stackTrace.Split(_newLines, StringSplitOptions.None);

        StringBuilder sb = new(message.Length + stackTrace.Length);

        for (int i = 0; i < messageLines.Length - 2; i++)
        {
            sb.AppendLine(messageLines[i]);
        }

        foreach (string line in stackTraceLines)
        {
            bool lastLine = line.Length switch
            {
                >= 122 => line[28] == '(' && line.StartsWith(CompiledScript_Execute, StringComparison.Ordinal),
                >= 99 => line[66] == '(' && line.StartsWith(UpdateDelegates_UpdateAndExecute2, StringComparison.Ordinal),
                >= 98 => line[65] == '(' && line.StartsWith(UpdateDelegates_UpdateAndExecuteVoid2, StringComparison.Ordinal),
                >= 52 => line[21] == '(' && line.StartsWith(CallSite_Target, StringComparison.Ordinal),
                _ => false
            };

            if (lastLine)
            {
                break;
            }

            sb.AppendLine(line);
        }

        sb.AppendLine();
        sb.Append(messageLines[^1]);

        _messageField.SetValue(ex, sb.ToString());
    }
}
