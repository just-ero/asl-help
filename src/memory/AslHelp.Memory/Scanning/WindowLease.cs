using System;
using System.Buffers;

namespace AslHelp.Memory.Scanning;

/// <summary>
///     A leased view over a region window. When backed by a pooled buffer, the first
///     <see cref="Dispose"/> returns it to the pool; further disposes are no-ops.
/// </summary>
internal struct WindowLease : IDisposable
{
    private byte[]? _rented;

    public WindowLease(ReadOnlyMemory<byte> bytes, nint start, byte[]? rented)
    {
        Bytes = bytes;
        Start = start;
        _rented = rented;
    }

    /// <summary>
    ///     An empty lease: no bytes, nothing pooled to return.
    /// </summary>
    public static WindowLease Empty => default;

    /// <summary>
    ///     The virtual address of <c>Bytes.Span[0]</c>.
    /// </summary>
    public nint Start { get; }

    /// <summary>
    ///     The leased bytes.
    /// </summary>
    public ReadOnlyMemory<byte> Bytes { get; }

    /// <summary>
    ///     Returns the pooled buffer, if any, to the shared pool. Safe to call more than once: the
    ///     reference is cleared on the first call so the buffer is never returned twice.
    /// </summary>
    public void Dispose()
    {
        var rented = _rented;
        _rented = null;
        ArrayPool<byte>.Shared.ReturnIfNotNull(rented);
    }
}
