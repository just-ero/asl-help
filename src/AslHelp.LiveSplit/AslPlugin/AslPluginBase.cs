using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using AslHelp.Shared;
using AslHelp.LiveSplit.Abstractions;

namespace AslHelp.LiveSplit;

[Obsolete("Do not use ASL-specific features.", true)]
public abstract partial class AslPluginBase : IAslPlugin<AslPluginBase>
{
    protected Autosplitter? _asl;
    private string? _gameName;

    protected AslPluginBase() { }

    public string GameName => _gameName ?? Game?.ProcessName ?? "Auto Splitter";

    [MemberNotNull(nameof(_asl))]
    protected void EnsureInitialized(
        [CallerMemberName] string? caller = null)
    {
        if (_asl is null)
        {
            string msg = $"Tried to call {caller}, but ASL is not initialized.";
            ThrowHelper.ThrowInvalidOperationException(msg);
        }
    }
}
