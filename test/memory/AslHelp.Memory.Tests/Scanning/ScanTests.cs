using System.Collections.Generic;
using System.Linq;

using AslHelp.Memory.Scanning;

using NUnit.Framework;

namespace AslHelp.Memory.Tests.Scanning;

[TestFixture]
public class ScanTests
{
    private static FakeMemoryReader Reader(byte[] data)
    {
        return new FakeMemoryReader(0x1000, data);
    }

    // ---- single steps ----

    [Test]
    public void Memory_SingleForStep_ReturnsMatchAddresses()
    {
        byte[] data =
        [
        //  0     1     2     3     4     5     6     7     8     9
            0x00, 0x00, 0xAB, 0xCD, 0x00, 0x00, 0x00, 0xAB, 0xCD, 0x00,
        ];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length, [ScanStep.For("AB CD")]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1002, 0x1007 }));
    }

    [Test]
    public void Memory_ForwardStep_AnchorsOnTransformedMatch()
    {
        byte[] data =
        [
        //  0     1     2     3     4     5     6     7
            0xAA, 0xBB, 0x00, 0x00, 0x00, 0xCC, 0xDD, 0x00,
        ];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AA BB").Transform(a => a + 2),
            ScanStep.Forward(0x10).For("CC DD"),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1005 }));
    }

    [Test]
    public void Memory_BackwardStep_AnchorsOnPreviousMatch()
    {
        byte[] data =
        [
        //  0     1     2     3     4     5     6     7
            0xCC, 0xDD, 0x00, 0x00, 0x00, 0xAA, 0xBB, 0x00,
        ];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AA BB"),
            ScanStep.Backward(0x10).For("CC DD"),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1000 }));
    }

    // ---- transforms ----

    [Test]
    public void Memory_PureTransform_ProjectsEachAddress()
    {
        byte[] data = [0x00, 0x00, 0xAB, 0xCD, 0x00, 0x00, 0x00, 0xAB, 0xCD, 0x00];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AB CD").Transform(a => a + 0x10),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1012, 0x1017 }));
    }

    [Test]
    public void Memory_FallibleTransform_DropsFailedMatches()
    {
        byte[] data = [0x00, 0x00, 0xAB, 0xCD, 0x00, 0x00, 0x00, 0xAB, 0xCD, 0x00];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AB CD").Transform(a => a == 0x1002 ? Result.Err<nint>("drop") : Result.Ok(a)),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1007 }));
    }

    [Test]
    public void Memory_FallibleTransform_AllFail_ReturnsEmpty()
    {
        byte[] data = [0xAB, 0xCD];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AB CD").Transform(_ => Result.Err<nint>("nope")),
        ]);

        Assert.That(results, Is.Empty);
    }

    // ---- caps ----

    [Test]
    public void Memory_First_CapsOpenerAtFirstMatch()
    {
        byte[] data = [0xAB, 0xCD, 0x00, 0xAB, 0xCD, 0x00, 0xAB, 0xCD];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AB CD").First(),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1000 }));
    }

    [Test]
    public void Memory_Take_CapsOpenerAtCount()
    {
        byte[] data = [0xAB, 0xCD, 0x00, 0xAB, 0xCD, 0x00, 0xAB, 0xCD];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AB CD").Take(2),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1000, 0x1003 }));
    }

    [Test]
    public void Memory_First_CapsWindowPerAnchor()
    {
        // Two openers; each forward window holds two CC DDs. First keeps the nearest per anchor.
        byte[] data =
        [
        //  0     1     2     3     4     5     6     7     8     9
            0xAA, 0xBB, 0xCC, 0xDD, 0xCC, 0xDD, 0xAA, 0xBB, 0xCC, 0xDD,
        ];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AA BB"),
            ScanStep.Forward(0x10).For("CC DD").First(),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1002, 0x1008 }));
    }

    // ---- out-of-region rule ----

    [Test]
    public void Memory_TransformOutOfRegion_OnLastStep_Yields()
    {
        byte[] data = [0xAA, 0xBB, 0x00, 0x00];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AA BB").Transform(a => a + 0x1000),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x2000 }));
    }

    [Test]
    public void Memory_TransformOutOfRegion_WithNextStep_Throws()
    {
        byte[] data = [0xAA, 0xBB, 0x00, 0x00];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AA BB").Transform(a => a + 0x1000),
            ScanStep.Forward(0x10).For("CC DD"),
        ]);

        Assert.That(() => results.ToList(), Throws.InvalidOperationException);
    }

    // ---- multi-region ----

    [Test]
    public void Memory_MultiRegion_ConcatenatesPerRegionResults()
    {
        byte[][] buffers =
        [
            [0x00, 0xAB, 0xCD, 0x00],
            [0xAB, 0xCD, 0x00, 0xAB, 0xCD],
        ];
        nint[] starts = [0x1000, 0x2000];

        var results = Scan.Memory(buffers, starts, [ScanStep.For("AB CD")]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1001, 0x2000, 0x2003 }));
    }

    [Test]
    public void Memory_MultiRegion_LengthMismatch_Throws()
    {
        byte[][] buffers = [[0xAB, 0xCD]];
        nint[] starts = [0x1000, 0x2000];

        Assert.That(
            () => Scan.Memory(buffers, starts, [ScanStep.For("AB CD")]),
            Throws.ArgumentException);
    }

    // ---- laziness ----

    [Test]
    public void Memory_IsLazy_OuterFirstStopsAfterOneMatch()
    {
        byte[] data = [0xAB, 0xCD, 0x00, 0xAB, 0xCD, 0x00, 0xAB, 0xCD];
        var transformed = 0;

        var first = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AB CD").Transform(a =>
            {
                transformed++;
                return a;
            }),
        ]).First();

        nint expected = 0x1000;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.EqualTo(expected));
            Assert.That(transformed, Is.EqualTo(1), "only the pulled match should be transformed");
        }
    }

    // ---- validation ----

    [Test]
    public void Memory_NoSteps_Throws()
    {
        Assert.That(
            () => Scan.Memory(Reader([0xAB]), 0x1000, 1),
            Throws.ArgumentException);
    }

    // ---- windowed openers ----

    [Test]
    public void Memory_ForwardOpener_TrimsRegionFromStart()
    {
        byte[] data =
        [
        //  0     1     2     3     4     5     6     7
            0xAB, 0xCD, 0x00, 0x00, 0x00, 0x00, 0xAB, 0xCD,
        ];

        // The window covers only [0x1000, 0x1004); the second AB CD at 0x1006 is out of range.
        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.Forward(4).For("AB CD"),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1000 }));
    }

    [Test]
    public void Memory_BackwardOpener_YieldsNothing()
    {
        byte[] data = [0xAB, 0xCD, 0x00, 0x00];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.Backward(0x10).For("AB CD"),
        ]);

        Assert.That(results, Is.Empty);
    }

    // ---- the real use case: follow a rel32 call to its target ----

    [Test]
    public void Memory_FollowsRel32CallToTarget()
    {
        // E8 <rel32> at offset 4 (a near-call); rel32 = 0x17 resolves the target to 0x1020,
        // where the marker 90 90 lives. The transform derefs through the reader, like real code.
        var data = new byte[0x24];
        data[4] = 0xE8;
        data[5] = 0x17; // little-endian rel32 = 0x00000017
        data[0x20] = 0x90;
        data[0x21] = 0x90;

        var reader = Reader(data);

        nint resolve(nint call)
        {
            var disp = new byte[4];
            reader.Read(call + 1, disp).Unwrap();
            var rel = disp[0] | (disp[1] << 8) | (disp[2] << 16) | (disp[3] << 24);
            return call + 5 + rel;
        }

        var results = Scan.Memory(reader, 0x1000, data.Length,
        [
            ScanStep.For("E8").Transform(resolve),
            ScanStep.Forward(2).For("90 90"),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1020 }));
    }

    // ---- laziness across steps ----

    [Test]
    public void Memory_IsLazy_OuterFirstStopsSecondStepEarly()
    {
        // Two openers, each with a CC DD in its forward window. A non-lazy fold would transform
        // every windowed match (3); pulling one result must transform exactly one.
        byte[] data =
        [
        //  0     1     2     3     4     5     6     7     8     9
            0xAA, 0xBB, 0xCC, 0xDD, 0x00, 0x00, 0xAA, 0xBB, 0xCC, 0xDD,
        ];
        var transformed = 0;

        var first = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AA BB"),
            ScanStep.Forward(0x10).For("CC DD").Transform(a =>
            {
                transformed++;
                return a;
            }),
        ]).First();

        nint expected = 0x1002;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(first, Is.EqualTo(expected));
            Assert.That(transformed, Is.EqualTo(1), "the second step must not run for later anchors");
        }
    }

    // ---- cap ordering vs. dropped matches ----

    [Test]
    public void Memory_First_KeepsFirstSurvivingMatch_NotFirstRaw()
    {
        // The first raw match (0x1002) is dropped; First must surface the next survivor, not nothing.
        byte[] data = [0x00, 0x00, 0xAB, 0xCD, 0x00, 0x00, 0x00, 0xAB, 0xCD, 0x00];

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AB CD")
                .Transform(a => a == 0x1002 ? Result.Err<nint>("drop") : Result.Ok(a))
                .First(),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1007 }));
    }

    // ---- transform composition ----

    [Test]
    public void Memory_ChainedTransforms_ApplyInOrder()
    {
        var data = new byte[0x20];
        data[0] = 0xAB;
        data[1] = 0xCD;

        // 0x1000 -> +2 -> 0x1002 -> +4 -> 0x1006; dropping either link would shift the result.
        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AB CD").Transform(a => a + 2).Transform(a => a + 4),
        ]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1006 }));
    }

    [Test]
    public void Memory_ChainedTransforms_ShortCircuitOnFailure()
    {
        byte[] data = [0xAB, 0xCD];
        var second = 0;

        var results = Scan.Memory(Reader(data), 0x1000, data.Length,
        [
            ScanStep.For("AB CD")
                .Transform(_ => Result.Err<nint>("stop"))
                .Transform(a =>
                {
                    second++;
                    return a;
                }),
        ]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(results.ToList(), Is.Empty);
            Assert.That(second, Is.EqualTo(0), "a transform after a failed one must not run");
        }
    }

    // ---- read failure ----

    [Test]
    public void Memory_WhenReadFails_ThrowsOnEnumeration()
    {
        var results = Scan.Memory(new FailingMemoryReader(), 0x1000, 4, [ScanStep.For("AB")]);

        Assert.That(() => results.ToList(), Throws.InvalidOperationException);
    }

    // ---- integration: scan readable pages of a holed region ----

    [Test]
    public void Memory_OverReadablePages_SkipsHolesAndMapsAddresses()
    {
        // AA BB sits in two readable sub-ranges and once inside the hole; only the readable hits
        // should surface, at their true addresses.
        byte[] backing =
        [
        //  0     1     2     3     4     5     6     7     8     9
            0xAA, 0xBB, 0x00, 0x00, 0xAA, 0xBB, 0x00, 0x00, 0xAA, 0xBB,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        ];

        FakeMemoryReader reader = new(0x1000, backing);
        var region = ScanRegion.OverRanges(
            reader,
            0x1000,
            16,
            [
                new MemoryPage(0x1000, 4, default, default, default),
                new MemoryPage(0x1008, 4, default, default, default),
            ]);

        List<byte[]> buffers = [];
        List<nint> starts = [];
        foreach (var (start, length) in region.EnumerateReadable(0x1000, 16))
        {
            using var lease = region.Rent(start, length);
            buffers.Add(lease.Bytes.ToArray());
            starts.Add(start);
        }

        var results = Scan.Memory([.. buffers], [.. starts], [ScanStep.For("AA BB")]);

        Assert.That(results, Is.EqualTo(new nint[] { 0x1000, 0x1008 }));
    }
}
