using System;
using System.Collections.Generic;
using System.Linq;

using AslHelp.Memory.Scanning;

using NUnit.Framework;

namespace AslHelp.Memory.Tests.Scanning;

[TestFixture]
public class ScanRegionTests
{
    private static nint Base => 0x1000;

    private static byte[] Bytes16()
    {
        return [.. Enumerable.Range(0, 16).Select(i => (byte)i)];
    }

    private static MemoryPage Range(nint @base, int size)
    {
        return new(@base, size, default, default, default);
    }

    // ---- factory validation ----

    [Test]
    public void OverBuffer_NullBuffer_Throws()
    {
        Assert.That(() => ScanRegion.OverBuffer(null!, Base, 0), Throws.ArgumentNullException);
    }

    [Test]
    public void OverBuffer_SizeGreaterThanBuffer_Throws()
    {
        Assert.That(
            () => ScanRegion.OverBuffer(new byte[4], Base, 5),
            Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void OverBuffer_NegativeSize_Throws()
    {
        Assert.That(
            () => ScanRegion.OverBuffer(new byte[4], Base, -1),
            Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void OverMemory_NullReader_Throws()
    {
        Assert.That(() => ScanRegion.OverMemory(null!, Base, 0), Throws.ArgumentNullException);
    }

    [Test]
    public void OverMemory_NegativeSize_Throws()
    {
        Assert.That(
            () => ScanRegion.OverMemory(new FailingMemoryReader(), Base, -1),
            Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void OverRanges_NullReader_Throws()
    {
        Assert.That(() => ScanRegion.OverRanges(null!, Base, 0, []), Throws.ArgumentNullException);
    }

    [Test]
    public void OverRanges_NullSubRanges_Throws()
    {
        Assert.That(
            () => ScanRegion.OverRanges(new FailingMemoryReader(), Base, 0, null!),
            Throws.ArgumentNullException);
    }

    [Test]
    public void OverRanges_NegativeSize_Throws()
    {
        Assert.That(
            () => ScanRegion.OverRanges(new FailingMemoryReader(), Base, -1, []),
            Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Properties_ReflectBaseAndSize()
    {
        var region = ScanRegion.OverBuffer(Bytes16(), Base, 16);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(region.BaseAddress, Is.EqualTo(Base));
            Assert.That(region.Size, Is.EqualTo(16));
        }
    }

    // ---- Rent: buffer-backed (zero-copy) ----

    [Test]
    public void Rent_OverBuffer_ReturnsSliceWithContentAndStart()
    {
        var region = ScanRegion.OverBuffer(Bytes16(), Base, 16);
        byte[] expected = [4, 5, 6, 7];

        using var lease = region.Rent(Base + 4, 4);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(lease.Start, Is.EqualTo(Base + 4));
            Assert.That(lease.Bytes.ToArray(), Is.EqualTo(expected));
        }
    }

    [Test]
    public void Rent_WindowOverhangingRegionEnd_ClampsLength()
    {
        var region = ScanRegion.OverBuffer(Bytes16(), Base, 16);
        byte[] expected = [12, 13, 14, 15];

        using var lease = region.Rent(Base + 12, 100);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(lease.Start, Is.EqualTo(Base + 12));
            Assert.That(lease.Bytes.ToArray(), Is.EqualTo(expected));
        }
    }

    [Test]
    public void Rent_WindowStartingBeforeRegion_ClampsStart()
    {
        var region = ScanRegion.OverBuffer(Bytes16(), Base, 16);
        byte[] expected = [0, 1];

        using var lease = region.Rent(Base - 4, 6);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(lease.Start, Is.EqualTo(Base));
            Assert.That(lease.Bytes.ToArray(), Is.EqualTo(expected));
        }
    }

    [Test]
    public void Rent_FullyOutsideRegion_ReturnsEmpty()
    {
        var region = ScanRegion.OverBuffer(Bytes16(), Base, 16);

        using var lease = region.Rent(Base + 100, 4);

        Assert.That(lease.Bytes.IsEmpty, Is.True);
    }

    [Test]
    public void Rent_ZeroLength_ReturnsEmpty()
    {
        var region = ScanRegion.OverBuffer(Bytes16(), Base, 16);

        using var lease = region.Rent(Base, 0);

        Assert.That(lease.Bytes.IsEmpty, Is.True);
    }

    // ---- Rent: reader-backed (pooled) ----

    [Test]
    public void Rent_OverMemory_ReadsThroughReader()
    {
        FakeMemoryReader reader = new(Base, Bytes16());
        var region = ScanRegion.OverMemory(reader, Base, 16);
        byte[] expected = [8, 9, 10, 11];

        using var lease = region.Rent(Base + 8, 4);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(lease.Start, Is.EqualTo(Base + 8));
            Assert.That(lease.Bytes.ToArray(), Is.EqualTo(expected));
            Assert.That(reader.Reads, Is.EqualTo(1));
        }
    }

    [Test]
    public void Rent_OverMemory_WhenReadFails_ReturnsEmpty()
    {
        var region = ScanRegion.OverMemory(new FailingMemoryReader(), Base, 16);

        using var lease = region.Rent(Base, 4);

        Assert.That(lease.Bytes.IsEmpty, Is.True);
    }

    // ---- EnumerateReadable ----

    [Test]
    public void EnumerateReadable_ContiguousRegion_YieldsWholeWindow()
    {
        var region = ScanRegion.OverMemory(new FakeMemoryReader(Base, Bytes16()), Base, 16);
        (nint, int)[] expected = [(Base, 16)];

        (nint, int)[] windows = [.. region.EnumerateReadable(Base, 16)];

        Assert.That(windows, Is.EqualTo(expected));
    }

    [Test]
    public void EnumerateReadable_AcrossHole_YieldsEachSubRange()
    {
        // Readable [0x1000, 0x1004) and [0x1008, 0x100C); a hole spans [0x1004, 0x1008).
        var region = ScanRegion.OverRanges(
            new FakeMemoryReader(Base, new byte[16]),
            Base,
            16,
            [Range(Base, 4), Range(Base + 8, 4)]);
        (nint, int)[] expected = [(Base, 4), (Base + 8, 4)];

        (nint, int)[] windows = [.. region.EnumerateReadable(Base, 16)];

        Assert.That(windows, Is.EqualTo(expected));
    }

    [Test]
    public void EnumerateReadable_WindowInsideHole_YieldsNothing()
    {
        var region = ScanRegion.OverRanges(
            new FakeMemoryReader(Base, new byte[16]),
            Base,
            16,
            [Range(Base, 4), Range(Base + 8, 4)]);

        (nint, int)[] windows = [.. region.EnumerateReadable(Base + 4, 4)];

        Assert.That(windows, Is.Empty);
    }

    [Test]
    public void EnumerateReadable_WindowSpanningHole_ClampsToReadableParts()
    {
        var region = ScanRegion.OverRanges(
            new FakeMemoryReader(Base, new byte[16]),
            Base,
            16,
            [Range(Base, 4), Range(Base + 8, 4)]);
        (nint, int)[] expected = [(Base + 2, 2), (Base + 8, 2)];

        // Window [0x1002, 0x100A) overlaps the tail of the first sub-range and the head of the second.
        (nint, int)[] windows = [.. region.EnumerateReadable(Base + 2, 8)];

        Assert.That(windows, Is.EqualTo(expected));
    }

    [Test]
    public void EnumerateReadable_WindowOverhangingRegion_Clamps()
    {
        var region = ScanRegion.OverMemory(new FakeMemoryReader(Base, new byte[16]), Base, 16);
        (nint, int)[] expected = [(Base + 12, 4)];

        (nint, int)[] windows = [.. region.EnumerateReadable(Base + 12, 100)];

        Assert.That(windows, Is.EqualTo(expected));
    }

    // ---- integration: enumerate readable windows, lease them, scan for a pattern ----

    [Test]
    public void EnumerateThenRentThenScan_FindsPatternInReadableMemory()
    {
        var region = ScanRegion.OverMemory(new FakeMemoryReader(Base, Bytes16()), Base, 16);
        nint[] expected = [Base + 4];

        List<nint> hits = [];
        foreach (var (start, length) in region.EnumerateReadable(Base, 16))
        {
            using var lease = region.Rent(start, length);
            foreach (var offset in Scan.Buffer(lease.Bytes.ToArray(), ScanStep.For("04 05 06")))
            {
                hits.Add(start + offset);
            }
        }

        Assert.That(hits, Is.EqualTo(expected));
    }
}
