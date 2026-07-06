using System;

namespace AslHelp.Memory;

/// <summary>
///     Reads raw bytes from a process's virtual address space.
/// </summary>
public interface IMemoryReader
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
}
