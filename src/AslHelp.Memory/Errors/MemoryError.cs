using AslHelp.Memory.Native;
using AslHelp.Shared.Results.Errors;

namespace AslHelp.Memory.Errors;

internal sealed record MemoryError : ResultError
{
    private MemoryError(string message)
        : base(message) { }

    public static MemoryError Other(string message)
    {
        return new(message);
    }

    public static MemoryError FromLastWin32Error()
    {
        return new(WinInteropWrapper.GetLastWin32ErrorMessage());
    }

    public static MemoryError ModuleNotFound(string moduleName)
    {
        return new($"Module '{moduleName}' not found.");
    }
}
