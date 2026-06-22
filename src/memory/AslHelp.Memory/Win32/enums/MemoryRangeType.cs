namespace AslHelp.Memory.Win32;

/// <summary>
///     Provides memory page type constants.
/// </summary>
/// <remarks>
///     For further information see:
///     <i><see href="https://docs.microsoft.com/windows/win32/api/winnt/ns-winnt-memory_basic_information#members">
///         MEMORY_BASIC_INFORMATION structure (winnt.h)
///     </see></i>
/// </remarks>
#pragma warning disable CA1008 // Enums should have zero value
#pragma warning disable CA1027 // Mark enums with FlagsAttribute
#pragma warning disable CA1028 // Enum storage should be int
public enum MemoryRangeType : uint
#pragma warning restore CA1008, CA1027, CA1028
{
    /// <summary>
    ///     Indicates that the memory pages within the region are private.
    /// </summary>
    Private = 0x0020000,

    /// <summary>
    ///     Indicates that the memory pages within the region are mapped into the view of a section.
    /// </summary>
    Mapped = 0x0040000,

    /// <summary>
    ///     Indicates that the memory pages within the region are mapped into the view of an image section.
    /// </summary>
    Image = 0x1000000,
}
