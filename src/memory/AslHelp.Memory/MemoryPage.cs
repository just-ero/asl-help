using System.Collections.Generic;

using AslHelp.Memory.Win32;

namespace AslHelp.Memory;

/// <summary>
///     Describes a contiguous range of pages in a process's virtual address space, together with
///     their state, protection, and type.
/// </summary>
public readonly record struct MemoryPage
{
    /// <summary>
    ///     Creates a memory range with the given bounds and page attributes.
    /// </summary>
    /// <param name="base">The base address of the region of pages.</param>
    /// <param name="regionSize">The size of the region, in bytes.</param>
    /// <param name="protect">The access protection of the pages in the region.</param>
    /// <param name="state">The state of the pages in the region.</param>
    /// <param name="type">The type of pages in the region.</param>
    public MemoryPage(nint @base, int regionSize, MemoryPageProtect protect, MemoryPageState state, MemoryPageType type)
    {
        Base = @base;
        RegionSize = regionSize;
        Protect = protect;
        State = state;
        Type = type;
    }

    internal unsafe MemoryPage(MemoryBasicInformation mbi)
    {
        Base = (nint)mbi.BaseAddress;
        RegionSize = (int)mbi.RegionSize;
        Protect = mbi.Protect;
        State = mbi.State;
        Type = mbi.Type;
    }

    /// <summary>
    ///     Gets the base address of the region of pages.
    /// </summary>
    public nint Base { get; }

    /// <summary>
    ///     Gets the size of the region, in bytes.
    /// </summary>
    public int RegionSize { get; }

    /// <summary>
    ///     Gets the access protection of the pages in the region.
    /// </summary>
    public MemoryPageProtect Protect { get; }

    /// <summary>
    ///     Gets the state of the pages in the region.
    /// </summary>
    public MemoryPageState State { get; }

    /// <summary>
    ///     Gets the type of pages in the region.
    /// </summary>
    public MemoryPageType Type { get; }

    /// <summary>
    ///     Returns a string representation of the memory range.
    /// </summary>
    /// <returns>
    ///     The base address and size, e.g. <c>"MemoryRange { Base = 0x7FF000, RegionSize = 0x1000 }"</c>.
    /// </returns>
    public override string ToString()
    {
        return $"{nameof(MemoryPage)} {{ "
            + $"{nameof(Base)} = 0x{(long)Base:X}, "
            + $"{nameof(RegionSize)} = 0x{RegionSize:X} "
            + $"}}";
    }
}

internal static class MemoryPageExtensions
{
    extension(MemoryPage)
    {
        public static bool IsReadable(MemoryPage page)
        {
            return page.State == MemoryPageState.Commit
                && page.Protect != 0
                && (page.Protect & (MemoryPageProtect.NoAccess | MemoryPageProtect.Guard)) == 0;
        }
    }

    extension(IEnumerable<MemoryPage> pages)
    {
        /// <summary>
        ///     Merges address-adjacent ranges into contiguous chunks, breaking at every gap.
        ///     Assumes <paramref name="pages"/> is ascending by base and non-overlapping.
        /// </summary>
        public IEnumerable<MemoryRange> AsContiguousRanges()
        {
            using var e = pages.GetEnumerator();
            if (!e.MoveNext())
            {
                yield break;
            }

            var start = e.Current.Base;
            long size = e.Current.RegionSize;

            while (e.MoveNext())
            {
                // Merge only while the run stays addressable as an int-sized MemoryRange. A larger
                // contiguous span cannot be read into a single buffer, so break it at the ceiling
                // rather than truncate the size through the (int) cast.
                if (e.Current.Base == start + size
                    && size + e.Current.RegionSize <= int.MaxValue)
                {
                    size += e.Current.RegionSize;
                }
                else
                {
                    yield return new(start, (int)size);
                    start = e.Current.Base;
                    size = e.Current.RegionSize;
                }
            }

            yield return new(start, (int)size);
        }
    }
}
