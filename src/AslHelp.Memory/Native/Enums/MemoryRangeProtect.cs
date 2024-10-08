namespace AslHelp.Memory.Native.Enums;

/// <summary>
///     Provides memory-protection constants.
/// </summary>
/// <remarks>
///     For further information, see:
///     <i><see href="https://docs.microsoft.com/windows/win32/memory/memory-protection-constants#constants">Memory Protection Constants</see></i>
/// </remarks>
public enum MemoryRangeProtect : uint
{
    /// <summary>
    ///     Disables all access to the committed region of pages.
    /// </summary>
    NoAccess = 0x00000001,

    /// <summary>
    ///     Enables read-only access to the committed region of pages.
    /// </summary>
    ReadOnly = 0x00000002,

    /// <summary>
    ///     Enables read-only or read/write access to the committed region of pages.
    /// </summary>
    ReadWrite = 0x00000004,

    /// <summary>
    ///     Enables read-only or copy-on-write access to a mapped view of a file mapping object.
    /// </summary>
    WriteCopy = 0x00000008,

    /// <summary>
    ///     Enables execute access to the committed region of pages.
    /// </summary>
    Execute = 0x00000010,

    /// <summary>
    ///     Enables execute or read-only access to the committed region of pages.
    /// </summary>
    ExecuteRead = 0x00000020,

    /// <summary>
    ///     Enables execute, read-only, or read/write access to the committed region of pages.
    /// </summary>
    ExecuteReadWrite = 0x00000040,

    /// <summary>
    ///     Enables execute, read-only, or copy-on-write access to a mapped view of a file mapping object.
    /// </summary>
    ExecuteWriteCopy = 0x00000080,

    /// <summary>
    ///     Pages in the region become guard pages.
    /// </summary>
    Guard = 0x00000100,

    /// <summary>
    ///     Sets all pages to be non-cachable.
    /// </summary>
    NoCache = 0x00000200,

    /// <summary>
    ///     Sets all pages to be write-combined.
    /// </summary>
    WriteCombine = 0x00000400
}
