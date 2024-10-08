using System;
using System.Diagnostics;

using AslHelp.LiveSplit.Control;
using AslHelp.LiveSplit.Settings;
using AslHelp.Memory;

namespace AslHelp.LiveSplit.Abstractions;

[Obsolete("Do not use ASL-specific features.", true)]
public interface IAslPlugin<TPlugin>
    where TPlugin : class, IAslPlugin<TPlugin>
{
    TPlugin Initialize(string? gameName = default, bool generateCode = default);

    TPlugin Exit();
    TPlugin Shutdown();

    bool Initialized { get; }

    Process? Game { get; set; }

    SettingsBuilder? Settings { get; }
    TimerController? Timer { get; }
    TextComponentController? Texts { get; }

    TPlugin LogToFile(string fileName, int maxLines = 4096, int linesToErase = 512);
    FileReader CreateFileReader(string fileName);

    TPlugin AlertRealTime(string? message = default);
    TPlugin AlertGameTime(string? message = default);
    TPlugin AlertLoadless(string? message = default);

    string GetMD5Hash();
    string GetMD5Hash(string moduleName);
    string GetMD5Hash(Module module);

    string GetSHA1Hash();
    string GetSHA1Hash(string moduleName);
    string GetSHA1Hash(Module module);

    string GetSHA256Hash();
    string GetSHA256Hash(string moduleName);
    string GetSHA256Hash(Module module);

    string GetSHA512Hash();
    string GetSHA512Hash(string moduleName);
    string GetSHA512Hash(Module module);
}
