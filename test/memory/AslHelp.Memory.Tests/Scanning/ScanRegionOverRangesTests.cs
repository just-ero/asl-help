using AslHelp.Memory.Scanning;

using NUnit.Framework;

namespace AslHelp.Memory.Tests.Scanning;

// Covers the construction-time normalization in ScanRegion.OverRanges: clamp each sub-range to the
// region bounds and drop any that fall entirely outside. Asserted against the stored sub-ranges
// (ScanRegion.Readable) so a clamping/dropping regression is actually caught — the public
// EnumerateReadable/Rent paths independently re-clamp to the region and would hide it.
[TestFixture]
public class ScanRegionOverRangesTests
{
    private static nint Base => 0x1000;

    private static MemoryPage Range(nint @base, int size)
    {
        return new(@base, size, default, default, default);
    }

    private static void AssertReadable(MemoryPage[] subRanges, params (nint Start, int Length)[] expected)
    {
        var region = ScanRegion.OverRanges(new FakeMemoryReader(Base, new byte[16]), Base, 16, subRanges);

        Assert.That(region.Readable, Is.EqualTo(expected));
    }

    [Test]
    public void OverRanges_SubRangeOverhangsRegionEnd_ClampsToRegionEnd()
    {
        AssertReadable([Range(Base, 100)], (Base, 16));
    }

    [Test]
    public void OverRanges_SubRangeStartsBeforeRegion_ClampsToBaseAddress()
    {
        AssertReadable([Range(Base - 0x100, 0x200)], (Base, 16));
    }

    [Test]
    public void OverRanges_SubRangeWithinRegion_IsStoredVerbatim()
    {
        // Guards against an over-eager clamp trimming a sub-range that is already inside.
        AssertReadable([Range(Base + 4, 8)], (Base + 4, 8));
    }

    [Test]
    public void OverRanges_SubRangeFullyBeforeRegion_IsDropped()
    {
        AssertReadable([Range(0x0000, 4), Range(Base, 4)], (Base, 4));
    }

    [Test]
    public void OverRanges_SubRangeFullyAfterRegion_IsDropped()
    {
        AssertReadable([Range(Base, 4), Range(0x2000, 4)], (Base, 4));
    }

    [Test]
    public void OverRanges_MultipleSubRanges_AreStoredClampedAndAscending()
    {
        // First overhangs the start (clamped to Base), second sits inside; the gap is a hole.
        AssertReadable(
            [Range(Base - 4, 8), Range(Base + 8, 4)],
            (Base, 4),
            (Base + 8, 4));
    }

    [Test]
    public void OverRanges_EmptySubRanges_StoresNothing()
    {
        AssertReadable([]);
    }
}
