namespace AslHelp.Memory.Native.Enums;

/// <summary>
///     Provides memory page type constants.
/// </summary>
/// <remarks>
///     For further information, see:
///     <i><see href="https://docs.microsoft.com/windows/win32/api/winnt/ns-winnt-memory_basic_information#members">MEMORY_BASIC_INFORMATION structure (winnt.h)</see></i>
/// </remarks>
public enum MemoryRangeType : uint
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
    Image = 0x1000000
}
