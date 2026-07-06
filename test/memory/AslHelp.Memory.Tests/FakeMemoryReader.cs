using System;

namespace AslHelp.Memory.Tests;

/// <summary>
///     An <see cref="IMemoryReader"/> backed by a byte array whose first byte sits at a chosen base
///     address. A read fully inside the backing range succeeds; anything else fails.
/// </summary>
internal sealed class FakeMemoryReader : IMemoryReader
{
    private readonly nint _base;
    private readonly byte[] _data;

    public FakeMemoryReader(nint baseAddress, byte[] data)
    {
        _base = baseAddress;
        _data = data;
    }

    /// <summary>
    ///     Gets the number of times <see cref="Read"/> has been called.
    /// </summary>
    public int Reads { get; private set; }

    public Result Read(nint address, Span<byte> buffer)
    {
        Reads++;

        var offset = (long)address - _base;
        if (offset < 0 || offset + buffer.Length > _data.Length)
        {
            return Result.Err($"Unreadable [0x{(long)address:X}, +0x{buffer.Length:X}).");
        }

        _data.AsSpan((int)offset, buffer.Length).CopyTo(buffer);
        return Result.Ok();
    }
}

/// <summary>
///     An <see cref="IMemoryReader"/> whose every read fails, simulating freed or protected pages.
/// </summary>
internal sealed class FailingMemoryReader : IMemoryReader
{
    public Result Read(nint address, Span<byte> buffer)
    {
        return Result.Err("unreadable");
    }
}
