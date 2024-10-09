using AslHelp.Shared.Results.Errors;

namespace AslHelp.Memory.Errors;

internal sealed record ScanPatternError : ResultError
{
    private ScanPatternError(string message)
        : base(message) { }

    public static ScanPatternError Other(string message)
    {
        return new(message);
    }

    public static ScanPatternError EmptyPattern
        => new("The provided pattern was empty.");

    public static ScanPatternError InvalidFormat
        => new("The provided pattern was in an unexpected format. All bytes must be fully specified.");
}
