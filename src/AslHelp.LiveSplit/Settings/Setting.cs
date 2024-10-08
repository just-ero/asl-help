namespace AslHelp.LiveSplit.Settings;

internal sealed record Setting(
    string? Id,
    bool State,
    string? Label,
    string? Parent,
    string? ToolTip);
