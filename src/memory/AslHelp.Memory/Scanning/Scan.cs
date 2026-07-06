using System;
using System.Collections.Generic;

namespace AslHelp.Memory.Scanning;

/// <summary>
///     Runs a chain of <see cref="ScanStep"/>s over raw bytes (<see cref="Buffer"/>) or a process's
///     memory (<see cref="Memory(IMemoryReader, nint, int, ScanStep[])"/>). The first step scans the
///     region; each later step is re-anchored at every match the previous step produced. Results are
///     yielded lazily, so terminal LINQ such as <c>First()</c> or <c>Take(n)</c> stops the scan
///     early.
/// </summary>
public static class Scan
{
    /// <summary>
    ///     Runs <paramref name="steps"/> over <paramref name="buffer"/>, yielding the offsets the
    ///     final step produces.
    /// </summary>
    /// <param name="buffer">The bytes to scan.</param>
    /// <param name="steps">
    ///     The steps to apply in order. A <see cref="ScanStep.Forward"/> or
    ///     <see cref="ScanStep.Backward"/> opener anchors at the region start.
    /// </param>
    /// <returns>
    ///     The match offsets produced by the final step, enumerated lazily.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="buffer"/> or <paramref name="steps"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="steps"/> is empty.
    /// </exception>
    public static IEnumerable<int> Buffer(byte[] buffer, params ScanStep[] steps)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ValidateSteps(steps);

        return BufferIterator(buffer, steps);
    }

    private static IEnumerable<int> BufferIterator(byte[] buffer, ScanStep[] steps)
    {
        foreach (var address in Fold(buffer, 0, steps, 0, 0))
        {
            // The region base is 0, so an address is its own offset.
            yield return (int)address;
        }
    }

    /// <summary>
    ///     Reads <c>[address, address + size)</c> and runs <paramref name="steps"/> over it, mapping
    ///     the final offsets back to process addresses.
    /// </summary>
    /// <param name="reader">The reader supplying the region bytes.</param>
    /// <param name="address">The base address of the region to scan.</param>
    /// <param name="size">The length of the region, in bytes.</param>
    /// <param name="steps">
    ///     The steps to apply in order. A <see cref="ScanStep.Forward"/> or
    ///     <see cref="ScanStep.Backward"/> opener anchors at the region start.
    /// </param>
    /// <returns>
    ///     The process addresses produced by the final step, enumerated lazily.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="reader"/> or <paramref name="steps"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="steps"/> is empty.
    /// </exception>
    public static IEnumerable<nint> Memory(IMemoryReader reader, nint address, int size, params ScanStep[] steps)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ValidateSteps(steps);

        return MemoryIterator(reader, address, size, steps);
    }

    private static IEnumerable<nint> MemoryIterator(IMemoryReader reader, nint address, int size, ScanStep[] steps)
    {
        var buffer = new byte[size];
        reader.Read(address, buffer).Unwrap();

        foreach (var result in Fold(buffer, address, steps, 0, address))
        {
            yield return result;
        }
    }

    /// <summary>
    ///     Runs <paramref name="steps"/> over each pre-read region in turn, mapping each region's
    ///     final offsets back to process addresses and concatenating the results.
    /// </summary>
    /// <param name="buffers">The bytes of each region.</param>
    /// <param name="starts">The base address of each region, parallel to <paramref name="buffers"/>.</param>
    /// <param name="steps">
    ///     The steps to apply in order. A <see cref="ScanStep.Forward"/> or
    ///     <see cref="ScanStep.Backward"/> opener anchors at the region start.
    /// </param>
    /// <returns>
    ///     The process addresses produced by the final step, across all regions, enumerated lazily.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="buffers"/>, <paramref name="starts"/>, or <paramref name="steps"/> is
    ///     <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <paramref name="buffers"/> and <paramref name="starts"/> differ in length, or
    ///     <paramref name="steps"/> is empty.
    /// </exception>
    public static IEnumerable<nint> Memory(byte[][] buffers, nint[] starts, params ScanStep[] steps)
    {
        ArgumentNullException.ThrowIfNull(buffers);
        ArgumentNullException.ThrowIfNull(starts);
        ValidateSteps(steps);

        if (buffers.Length != starts.Length)
        {
            throw new ArgumentException(
                "There must be one start address per region buffer.", nameof(starts));
        }

        return MultiRegionIterator(buffers, starts, steps);
    }

    private static IEnumerable<nint> MultiRegionIterator(byte[][] buffers, nint[] starts, ScanStep[] steps)
    {
        for (var i = 0; i < buffers.Length; i++)
        {
            foreach (var result in Fold(buffers[i], starts[i], steps, 0, starts[i]))
            {
                yield return result;
            }
        }
    }

    private static IEnumerable<nint> Fold(byte[] buffer, nint @base, ScanStep[] steps, int index, nint anchor)
    {
        var last = index == steps.Length - 1;
        foreach (var address in steps[index].Evaluate(buffer, @base, anchor))
        {
            if (last)
            {
                yield return address;
                continue;
            }

            // Window steps clamp to the region on their own; only a transform can land an anchor
            // outside it, and a following step cannot read bytes that were never loaded.
            if (address < @base || address >= @base + buffer.Length)
            {
                throw new InvalidOperationException(
                    $"Step {index} produced 0x{address:X} outside the scanned region "
                    + $"[0x{@base:X}, 0x{@base + buffer.Length:X}); the next step cannot anchor there.");
            }

            foreach (var result in Fold(buffer, @base, steps, index + 1, address))
            {
                yield return result;
            }
        }
    }

    private static void ValidateSteps(ScanStep[] steps)
    {
        ArgumentNullException.ThrowIfNull(steps);

        if (steps.Length == 0)
        {
            throw new ArgumentException("At least one step is required.", nameof(steps));
        }
    }
}
