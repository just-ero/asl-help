using System;

using AslHelp.Memory.Scanning;

using NUnit.Framework;

namespace AslHelp.Memory.Tests.Scanning;

[TestFixture]
public class ScanPatternTests
{
    // ---- values & masks ----

    [Test]
    public void Parse_FixedSignature_HasNoMask()
    {
        var pattern = ScanPattern.Parse("48 8B 05");
        byte[] expectedValues = [0x48, 0x8B, 0x05];

        Assert.Multiple(() =>
        {
            Assert.That(pattern.ByteLength, Is.EqualTo(3));
            Assert.That(pattern.Values, Is.EqualTo(expectedValues));
            Assert.That(pattern.Masks, Is.Null);
        });
    }

    [Test]
    public void Parse_FullByteWildcard_ZeroesValueKeepsMaskHoles()
    {
        var pattern = ScanPattern.Parse("48 ?? 05");
        byte[] expectedValues = [0x48, 0x00, 0x05];
        byte[] expectedMasks = [0xFF, 0x00, 0xFF];

        Assert.Multiple(() =>
        {
            Assert.That(pattern.Values, Is.EqualTo(expectedValues));
            Assert.That(pattern.Masks, Is.EqualTo(expectedMasks));
        });
    }

    [Test]
    public void Parse_HighNibbleWildcard_MasksHighNibble()
    {
        var pattern = ScanPattern.Parse("?B");
        byte[] expectedValues = [0x0B];
        byte[] expectedMasks = [0x0F];

        Assert.Multiple(() =>
        {
            Assert.That(pattern.Values, Is.EqualTo(expectedValues));
            Assert.That(pattern.Masks, Is.EqualTo(expectedMasks));
        });
    }

    [Test]
    public void Parse_LowNibbleWildcard_MasksLowNibble()
    {
        var pattern = ScanPattern.Parse("1?");
        byte[] expectedValues = [0x10];
        byte[] expectedMasks = [0xF0];

        Assert.Multiple(() =>
        {
            Assert.That(pattern.Values, Is.EqualTo(expectedValues));
            Assert.That(pattern.Masks, Is.EqualTo(expectedMasks));
        });
    }

    [Test]
    public void Parse_PreMasksValue_ClearsWildcardNibbleBits()
    {
        // The 'F' high nibble is wildcarded, so its bits are cleared from the stored value.
        var pattern = ScanPattern.Parse("?F");
        byte[] expectedValues = [0x0F];

        Assert.That(pattern.Values, Is.EqualTo(expectedValues));
    }

    // ---- whitespace & wildcard spellings ----

    [Test]
    public void Parse_IgnoresWhitespace()
    {
        Assert.That(ScanPattern.Parse("488B05").Values, Is.EqualTo(ScanPattern.Parse("48 8B 05").Values));
    }

    [Test]
    public void Parse_AnyNonHexCharIsWildcard()
    {
        var expected = ScanPattern.Parse("12 ?? 56").Masks!;

        Assert.Multiple(() =>
        {
            Assert.That(ScanPattern.Parse("12 xx 56").Masks, Is.EqualTo(expected));
            Assert.That(ScanPattern.Parse("12 .. 56").Masks, Is.EqualTo(expected));
        });
    }

    // ---- lead (anchor) ----

    [Test]
    public void Parse_Lead_IsLongestFixedRun()
    {
        // "8B 05" is the longest run of fully fixed bytes, at byte offset 1.
        var pattern = ScanPattern.Parse("?? 8B 05 ??");

        Assert.That(pattern.Lead, Is.EqualTo((1, 2)));
    }

    [Test]
    public void Parse_Lead_NoFixedByte_IsZeroLength()
    {
        var pattern = ScanPattern.Parse("1? 2?");

        Assert.That(pattern.Lead.Length, Is.EqualTo(0));
    }

    // ---- validation ----

    [Test]
    public void TryParse_OddLength_ReturnsFalse()
    {
        Assert.That(ScanPattern.TryParse("48 8", out _), Is.False);
    }

    [Test]
    public void Parse_OddLength_Throws()
    {
        Assert.That(() => ScanPattern.Parse("48 8"), Throws.InstanceOf<FormatException>());
    }

    // ---- ToString round-trip ----

    [TestCase("48 8B 05")]
    [TestCase("48 ?? 05")]
    [TestCase("1? ?B ??")]
    public void ToString_RoundTripsCanonicalForm(string signature)
    {
        Assert.That(ScanPattern.Parse(signature).ToString(), Is.EqualTo(signature));
    }
}
