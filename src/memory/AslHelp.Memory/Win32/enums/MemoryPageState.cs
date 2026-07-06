namespace AslHelp.Memory.Win32;

/// <summary>
///     Provides memory page state constants.
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
public enum MemoryPageState : uint
#pragma warning restore CA1008, CA1027, CA1028
{
    /// <summary>
    ///     Allocates memory charges for the specified reserved memory pages.
    /// </summary>
    Commit = 0x00001000,

    /// <summary>
    ///     Reserves a range of the process' virtual address space.
    /// </summary>
    Reserve = 0x00002000,

    /// <summary>
    ///     Decommits a range of the process' virtual address space.
    /// </summary>
    Decommit = 0x00004000,

    /// <summary>
    ///     Releases a range of the process' virtual address space.
    /// </summary>
    Release = 0x00008000,

    /// <summary>
    ///     Indicates that data in the memory range is no longer of interest.
    /// </summary>
    Reset = 0x00080000,

    /// <summary>
    ///     Allocates memory at the highest possible address.
    /// </summary>
    TopDown = 0x00100000,

    /// <summary>
    ///     Causes the system to track pages that are written to in the allocated region.
    /// </summary>
    WriteWatch = 0x00200000,

    /// <summary>
    ///     Reserves an address range that can be used to map Address Windowing Extensions pages.
    /// </summary>
    Physical = 0x00400000,

    /// <summary>
    ///     Indicates that the data in the specified memory range is of interest to the caller and attempts to reverse the effects of
    ///     <see cref="Reset"/>.
    /// </summary>
    ResetUndo = 0x01000000,

    /// <summary>
    ///     Allocates memory using large page support.
    /// </summary>
    LargePages = 0x20000000,
}
