namespace AslHelp.Data;

internal static class Memory
{
    public static Func<string, nint, nint> OnFound { get; } = (_, addr) => Basic.Instance.FromAssemblyAddress(addr);
}
