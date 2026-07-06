using AslHelp.Memory.Win32;

using NUnit.Framework;

namespace AslHelp.Memory.Tests;

[TestFixture]
public class MemoryPageTests
{
    [Test]
    public void ToString_FormatsBaseAndSizeAsHex()
    {
        MemoryPage page = new(0x7FF000, 0x1000, default, default, default);

        Assert.That(page.ToString(), Is.EqualTo("MemoryPage { Base = 0x7FF000, RegionSize = 0x1000 }"));
    }
}
