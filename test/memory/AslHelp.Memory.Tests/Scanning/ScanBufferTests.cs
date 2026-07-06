using System.Linq;

using AslHelp.Memory.Scanning;

using NUnit.Framework;

namespace AslHelp.Memory.Tests.Scanning;

[TestFixture]
public class ScanBufferTests
{
    private static readonly byte[] _haystack =
    [
    //  0     1     2     3     4     5     6     7     8     9     10    11
        0x48, 0x8B, 0x05, 0x10, 0x20, 0x48, 0x8B, 0x05, 0x99, 0x48, 0x8B, 0xFF,
    ];

    private static readonly byte[] _repeated = [0xAA, 0xAA, 0xAA, 0xAA];

    private static void AssertMatches(byte[] haystack, string signature, params int[] expected)
    {
        Assert.That(Scan.Buffer(haystack, ScanStep.For(signature)), Is.EqualTo(expected));
    }

    // ---- fixed patterns ----

    [Test]
    public void Buffer_FixedPattern_ReturnsAllOccurrences()
    {
        AssertMatches(_haystack, "48 8B 05", 0, 5);
    }

    [Test]
    public void Buffer_SingleFixedByte_ReturnsEveryHit()
    {
        AssertMatches(_haystack, "8B", 1, 6, 10);
    }

    [Test]
    public void Buffer_WholeBuffer_ReturnsZero()
    {
        AssertMatches(_haystack, "48 8B 05 10 20 48 8B 05 99 48 8B FF", 0);
    }

    [Test]
    public void Buffer_MatchAtFinalOffset_ReturnsLastIndex()
    {
        AssertMatches(_haystack, "FF", 11);
    }

    // ---- wildcards ----

    [Test]
    public void Buffer_WildcardByteAtStart_AnchorsOnLaterFixedRun()
    {
        AssertMatches(_haystack, "?? 8B 05", 0, 5);
    }

    [Test]
    public void Buffer_WildcardByteAtEnd_Matches()
    {
        AssertMatches(_haystack, "48 8B ??", 0, 5, 9);
    }

    [Test]
    public void Buffer_InternalWildcard_Matches()
    {
        AssertMatches(_haystack, "48 ?? 05", 0, 5);
    }

    [Test]
    public void Buffer_HighNibbleWildcard_MatchesOnNibble()
    {
        AssertMatches(_haystack, "4?", 0, 5, 9);
    }

    [Test]
    public void Buffer_LowNibbleWildcard_MatchesOnNibble()
    {
        AssertMatches(_haystack, "?B", 1, 6, 10);
    }

    [Test]
    public void Buffer_NoFullyFixedByte_BruteForceMatches()
    {
        AssertMatches(_haystack, "1?", 3);
    }

    // ---- overlap & laziness ----

    [Test]
    public void Buffer_OverlappingMatches_ReturnsEachStart()
    {
        AssertMatches(_repeated, "AA AA", 0, 1, 2);
    }

    [Test]
    public void Buffer_IsLazy_TakeYieldsPrefix()
    {
        int[] expected = [0];

        Assert.That(Scan.Buffer(_repeated, ScanStep.For("AA AA")).Take(1), Is.EqualTo(expected));
    }

    // ---- empty / no match / validation ----

    [Test]
    public void Buffer_NoMatch_ReturnsEmpty()
    {
        AssertMatches(_haystack, "DE AD BE EF");
    }

    [Test]
    public void Buffer_NeedleLongerThanHaystack_ReturnsEmpty()
    {
        AssertMatches(_haystack, "48 8B 05 10 20 48 8B 05 99 48 8B FF 00");
    }

    [Test]
    public void Buffer_EmptyHaystack_ReturnsEmpty()
    {
        byte[] haystack = [];

        AssertMatches(haystack, "48");
    }

    [Test]
    public void Buffer_NullBuffer_ThrowsEagerly()
    {
        // Validation must run on the call, not be deferred to enumeration.
        Assert.That(() => Scan.Buffer(null!, ScanStep.For("48")), Throws.ArgumentNullException);
    }

    // ---- transforms (offset coordinates, base 0) ----

    [Test]
    public void Buffer_Transform_ProjectsOffsets()
    {
        byte[] haystack = [0x00, 0xAB, 0xCD, 0x00];
        int[] expected = [3];

        var results = Scan.Buffer(haystack, ScanStep.For("AB CD").Transform(a => a + 2));

        Assert.That(results, Is.EqualTo(expected));
    }

    [Test]
    public void Buffer_TransformBelowRegion_WithNextStep_Throws()
    {
        // Base is 0, so a transform to a negative offset trips the lower out-of-region bound.
        byte[] haystack = [0xAA, 0xBB, 0x00, 0x00];

        var results = Scan.Buffer(haystack,
            ScanStep.For("AA BB").Transform(a => a - 0x100),
            ScanStep.Forward(0x10).For("CC DD"));

        Assert.That(() => results.ToList(), Throws.InvalidOperationException);
    }

    // ---- large buffer (exercises the vectorized anchor search, not the scalar fallback) ----

    [Test]
    public void Buffer_LargeBuffer_FindsMatchesAcrossVectorBoundaries()
    {
        var big = new byte[8192];
        int[] at = [1000, 4093, 8188];
        foreach (var off in at)
        {
            big[off] = 0xDE;
            big[off + 1] = 0xAD;
            big[off + 2] = 0xBE;
            big[off + 3] = 0xEF;
        }

        Assert.That(Scan.Buffer(big, ScanStep.For("DE AD BE EF")), Is.EqualTo(at));
    }
}
