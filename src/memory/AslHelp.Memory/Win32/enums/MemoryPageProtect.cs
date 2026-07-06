namespace AslHelp.Memory.Win32;

/// <summary>
///     Provides memory-protection constants.
/// </summary>
/// <remarks>
///     For further information see:
///     <i><see href="https://docs.microsoft.com/windows/win32/memory/memory-protection-constants#constants">
///         Memory Protection Constants
///     </see></i>
/// </remarks>
#pragma warning disable CA1008 // Enums should have zero value
#pragma warning disable CA1027 // Mark enums with FlagsAttribute
#pragma warning disable CA1028 // Enum storage should be int
public enum MemoryPageProtect : uint
#pragma warning restore CA1008, CA1027, CA1028
{
    /// <summary>
    ///     Disables all access to the committed region of pages.
    /// </summary>
    NoAccess = 0x001,

    /// <summary>
    ///     Enables read-only access to the committed region of pages.
    /// </summary>
    ReadOnly = 0x002,

    /// <summary>
    ///     Enables read-only or read/write access to the committed region of pages.
    /// </summary>
    ReadWrite = 0x004,

    /// <summary>
    ///     Enables read-only or copy-on-write access to a mapped view of a file mapping object.
    /// </summary>
    WriteCopy = 0x008,

    /// <summary>
    ///     Enables execute access to the committed region of pages.
    /// </summary>
    Execute = 0x010,

    /// <summary>
    ///     Enables execute or read-only access to the committed region of pages.
    /// </summary>
    ExecuteRead = 0x020,

    /// <summary>
    ///     Enables execute, read-only, or read/write access to the committed region of pages.
    /// </summary>
    ExecuteReadWrite = 0x040,

    /// <summary>
    ///     Enables execute, read-only, or copy-on-write access to a mapped view of a file mapping object.
    /// </summary>
    ExecuteWriteCopy = 0x080,

    /// <summary>
    ///     Pages in the region become guard pages.
    /// </summary>
    Guard = 0x100,

    /// <summary>
    ///     Sets all pages to be non-cachable.
    /// </summary>
    NoCache = 0x200,

    /// <summary>
    ///     Sets all pages to be write-combined.
    /// </summary>
    WriteCombine = 0x400,
}

