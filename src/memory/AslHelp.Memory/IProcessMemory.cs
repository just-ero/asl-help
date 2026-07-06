using System.Collections.Generic;

namespace AslHelp.Memory;

/// <summary>
///     Reads a process's virtual address space and enumerates its committed memory pages.
/// </summary>
public interface IProcessMemory : IMemoryReader
{
    /// <summary>
    ///     Enumerates the memory pages overlapping <c>[start, start + size)</c>, in ascending
    ///     address order.
    /// </summary>
    /// <param name="start">The inclusive start address of the range to walk.</param>
    /// <param name="size">The length of the range, in bytes.</param>
    /// <returns>
    ///     The pages overlapping the range.
    /// </returns>
    IEnumerable<MemoryPage> GetMemoryPages(nint start, nint size);
}
