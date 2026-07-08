using System.Linq;

using NUnit.Framework;

namespace AslHelp.Memory.Tests;

[TestFixture]
public class MemoryRangeExtensionsTests
{
    private static MemoryPage Page(nint @base, int size)
    {
        return new(@base, size, default, default, default);
    }

    private static void AssertCombined(MemoryPage[] pages, params (nint Base, int Size)[] expected)
    {
        Assert.That(pages.AsContiguousRanges().Select(r => (r.Base, r.Size)), Is.EqualTo(expected));
    }

    // ---- CombineContiguousRanges ----

    [Test]
    public void CombineContiguousPages_Empty_ReturnsEmpty()
    {
        MemoryPage[] pages = [];

        Assert.That(pages.AsContiguousRanges(), Is.Empty);
    }

    [Test]
    public void CombineContiguousPages_Single_ReturnsItself()
    {
        AssertCombined([Page(0x1000, 0x1000)], (0x1000, 0x1000));
    }

    [Test]
    public void CombineContiguousPages_AdjacentRanges_AreMerged()
    {
        AssertCombined([Page(0x1000, 0x1000), Page(0x2000, 0x1000)], (0x1000, 0x2000));
    }

    [Test]
    public void CombineContiguousPages_GapBetweenRanges_AreKeptSeparate()
    {
        AssertCombined([Page(0x1000, 0x1000), Page(0x3000, 0x1000)], (0x1000, 0x1000), (0x3000, 0x1000));
    }

    [Test]
    public void CombineContiguousPages_RunOfAdjacentRanges_CollapseToOne()
    {
        AssertCombined([Page(0x1000, 0x1000), Page(0x2000, 0x1000), Page(0x3000, 0x1000)], (0x1000, 0x3000));
    }

    [Test]
    public void CombineContiguousPages_MergeThenGap_FlushesBeforeGap()
    {
        AssertCombined(
            [Page(0x1000, 0x1000), Page(0x2000, 0x1000), Page(0x4000, 0x1000)],
            (0x1000, 0x2000),
            (0x4000, 0x1000));
    }

    [Test]
    public void CombineContiguousPages_RunExceedingIntMax_DoesNotTruncate()
    {
        // Two adjacent int.MaxValue pages form a >2GiB contiguous run; the combined Size must not
        // wrap through a narrowing cast.
        MemoryPage[] pages =
        [
            Page(0x1000, int.MaxValue),
            Page(unchecked((nint)(0x1000L + int.MaxValue)), int.MaxValue),
        ];

        MemoryRange[] combined = [.. pages.AsContiguousRanges()];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(combined.All(r => r.Size > 0), Is.True, "no range may report a truncated/negative size");
            Assert.That(combined.Sum(r => (long)r.Size), Is.EqualTo(2L * int.MaxValue), "the full span must be covered");
        }
    }
}
