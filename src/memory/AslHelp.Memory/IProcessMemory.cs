using System;
using System.Collections.Generic;

namespace AslHelp.Memory;

/// <summary>
///     Reads a process's virtual address space and enumerates its committed memory pages.
/// </summary>
public interface IProcessMemory
{
    /// <summary>
    ///     Reads bytes from <paramref name="address"/> into <paramref name="buffer"/>, filling it
    ///     completely.
    /// </summary>
    /// <param name="address">The address to read from.</param>
    /// <param name="buffer">The destination buffer; its length is the number of bytes to read.</param>
    /// <returns>
    ///     A successful <see cref="Result"/> when the whole buffer was read; otherwise, a failed
    ///     result carrying the error.
    /// </returns>
    Result Read(nint address, Span<byte> buffer);

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
