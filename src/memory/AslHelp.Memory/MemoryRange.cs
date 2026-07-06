namespace AslHelp.Memory;

/// <summary>
///
/// </summary>
/// <param name="Base"></param>
/// <param name="Size"></param>
public readonly record struct MemoryRange(
    nint Base,
    int Size);
