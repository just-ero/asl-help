using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;

namespace AslHelp.Memory.Scanning;

/// <summary>
///     Provides region bytes to the scan engine, either from a buffer (zero-copy) or through an
///     <see cref="IMemoryReader"/> (pooled), over one or more ascending, non-overlapping readable
///     sub-ranges. Gaps between sub-ranges are holes; a leased window never crosses one.
/// </summary>
internal sealed class ScanRegion
{
    private readonly byte[]? _buffer;
    private readonly IMemoryReader? _reader;
    private readonly (nint Start, int Length)[] _readable;

    private ScanRegion(
        nint baseAddress,
        int size,
        byte[]? buffer,
        IMemoryReader? reader,
        (nint Start, int Length)[] readable)
    {
        _buffer = buffer;
        _reader = reader;
        _readable = readable;

        BaseAddress = baseAddress;
        Size = size;
    }

    /// <summary>
    ///     Gets the inclusive start address of the logical region.
    /// </summary>
    public nint BaseAddress { get; }

    /// <summary>
    ///     Gets the length of the logical region, in bytes.
    /// </summary>
    public int Size { get; }

    /// <summary>
    ///     Gets the readable sub-ranges after construction-time clamping and dropping, ascending and
    ///     non-overlapping. Exposed so the normalization in <see cref="OverRanges"/> can be observed
    ///     directly, before the re-clamping <see cref="EnumerateReadable"/>/<see cref="Rent"/> paths.
    /// </summary>
    internal IReadOnlyList<(nint Start, int Length)> Readable => _readable;

    /// <summary>
    ///     Creates a region over an in-memory buffer; windows are leased zero-copy.
    /// </summary>
    /// <param name="buffer">The region bytes; index 0 maps to <paramref name="baseAddress"/>.</param>
    /// <param name="baseAddress">The virtual address of <c>buffer[0]</c>.</param>
    /// <param name="size">The number of valid bytes in <paramref name="buffer"/>.</param>
    /// <returns>
    ///     The buffer-backed region.
    /// </returns>
    public static ScanRegion OverBuffer(byte[] buffer, nint baseAddress, int size)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(size, buffer.Length);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 0);

        return new(baseAddress, size, buffer, reader: null, [(baseAddress, size)]);
    }

    /// <summary>
    ///     Creates a region over a contiguous readable span served by <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The reader providing the bytes.</param>
    /// <param name="baseAddress">The inclusive start of the span.</param>
    /// <param name="size">The length of the span, in bytes.</param>
    /// <returns>
    ///     The reader-backed region.
    /// </returns>
    public static ScanRegion OverMemory(IMemoryReader reader, nint baseAddress, int size)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 0);

        return new(baseAddress, size, buffer: null, reader, [(baseAddress, size)]);
    }

    /// <summary>
    ///     Creates a region over the readable sub-ranges of a logical span, skipping the gaps
    ///     between them.
    /// </summary>
    /// <param name="reader">The reader providing the bytes inside each sub-range.</param>
    /// <param name="baseAddress">The inclusive start of the logical span.</param>
    /// <param name="size">The length of the logical span, in bytes.</param>
    /// <param name="subRanges">Readable sub-ranges, ascending by base, non-overlapping.</param>
    /// <returns>
    ///     The reader-backed, hole-aware region.
    /// </returns>
    public static ScanRegion OverRanges(
        IMemoryReader reader,
        nint baseAddress,
        int size,
        IReadOnlyList<MemoryPage> subRanges)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 0);
        ArgumentNullException.ThrowIfNull(subRanges);

        long regionStart = baseAddress;
        var regionEnd = regionStart + size;

        List<(nint, int)> readable = [with(subRanges.Count)];
        foreach (var range in subRanges)
        {
            long start = range.Base;
            var end = start + range.RegionSize;

            if (start < regionStart)
            {
                start = regionStart;
            }

            if (end > regionEnd)
            {
                end = regionEnd;
            }

            if (end > start)
            {
                readable.Add(((nint)start, (int)(end - start)));
            }
        }

        AssertAscendingNonOverlapping(readable);

        return new(baseAddress, size, buffer: null, reader, [.. readable]);
    }

    /// <summary>
    ///     Leases the bytes of <c>[start, start + length)</c>, clamped to the region. The caller
    ///     guarantees the range lies within a single readable sub-range, so the lease never
    ///     crosses a hole.
    /// </summary>
    /// <param name="start">The start address of the window.</param>
    /// <param name="length">The window length, in bytes.</param>
    /// <returns>
    ///     The leased window; <see cref="WindowLease.Empty"/> when empty or unreadable.
    /// </returns>
    public WindowLease Rent(nint start, int length)
    {
        var (from, len) = Clamp(start, length);
        if (len == 0)
        {
            return WindowLease.Empty;
        }

        AssertWithinSingleSubRange(from, len);

        if (_buffer is { } buffer)
        {
            // Zero-copy: buffer[0] maps to BaseAddress, so the slice is an in-bounds view over
            // the caller's array. No pooling, no copy.
            var index = (int)(from - BaseAddress);
            return new(buffer.AsMemory(index, len), from, rented: null);
        }

        var rented = ArrayPool<byte>.Shared.Rent(len);
        if (_reader!.Read(from, rented.AsSpan(0, len)).IsErr)
        {
            // Page freed or protection changed under us; treat the window as a hole.
            ArrayPool<byte>.Shared.Return(rented);
            return WindowLease.Empty;
        }

        return new(rented.AsMemory(0, len), from, rented);
    }

    /// <summary>
    ///     Enumerates the contiguous readable sub-windows of <c>[start, start + length)</c> in
    ///     ascending address order.
    /// </summary>
    /// <param name="start">The start address of the window.</param>
    /// <param name="length">The window length, in bytes.</param>
    /// <returns>
    ///     The readable sub-windows, ascending and non-overlapping.
    /// </returns>
    public IEnumerable<(nint Start, int Length)> EnumerateReadable(nint start, int length)
    {
        var (from, len) = Clamp(start, length);
        if (len == 0)
        {
            yield break;
        }

        long winStart = from;
        var winEnd = winStart + len;

        foreach (var (subStart, subLength) in _readable)
        {
            long subBase = subStart;
            var subEnd = subBase + subLength;

            if (subEnd <= winStart)
            {
                continue;
            }

            if (subBase >= winEnd)
            {
                yield break;
            }

            var s = subBase < winStart ? winStart : subBase;
            var e = subEnd > winEnd ? winEnd : subEnd;
            if (e > s)
            {
                yield return ((nint)s, (int)(e - s));
            }
        }
    }

    private (nint Start, int Length) Clamp(nint start, int length)
    {
        long regionStart = BaseAddress;
        var regionEnd = regionStart + Size;

        long from = start;
        var to = from + length;

        if (from < regionStart)
        {
            from = regionStart;
        }

        if (to > regionEnd)
        {
            to = regionEnd;
        }

        var len = to - from;
        return len <= 0 ? (default, 0) : ((nint)from, (int)len);
    }

    [Conditional("DEBUG")]
    private static void AssertAscendingNonOverlapping(List<(nint Start, int Length)> readable)
    {
        for (var i = 1; i < readable.Count; i++)
        {
            long previousEnd = readable[i - 1].Start + readable[i - 1].Length;
            if (readable[i].Start < previousEnd)
            {
                Debug.Fail(
                    "ScanRegion sub-ranges must be ascending by base and non-overlapping.");
            }
        }
    }

    [Conditional("DEBUG")]
    private void AssertWithinSingleSubRange(nint from, int len)
    {
        long start = from;
        var end = start + len;

        foreach (var (subStart, subLength) in _readable)
        {
            if (start >= subStart && end <= subStart + subLength)
            {
                return;
            }
        }

        Debug.Fail(
            $"Rent([0x{(long)from:X}, +0x{len:X})) crosses a hole. "
            + "Callers must only rent windows yielded by EnumerateReadable.");
    }
}
