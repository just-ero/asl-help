using AslHelp.Memory.Win32;

using NUnit.Framework;

namespace AslHelp.Memory.Tests;

[TestFixture]
public class MemoryPageTests
{
    [Test]
    public void Constructor_MapsAllMembers()
    {
        MemoryPage page = new(
            0x7FF000,
            0x1000,
            MemoryPageProtect.ExecuteRead,
            MemoryPageState.Commit,
            MemoryPageType.Image);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page.Base, Is.EqualTo((nint)0x7FF000));
            Assert.That(page.RegionSize, Is.EqualTo(0x1000));
            Assert.That(page.Protect, Is.EqualTo(MemoryPageProtect.ExecuteRead));
            Assert.That(page.State, Is.EqualTo(MemoryPageState.Commit));
            Assert.That(page.Type, Is.EqualTo(MemoryPageType.Image));
        }
    }

    [Test]
    public void ToString_FormatsBaseAndSizeAsHex()
    {
        MemoryPage page = new(0x7FF000, 0x1000, default, default, default);

        Assert.That(page.ToString(), Is.EqualTo("MemoryPage { Base = 0x7FF000, RegionSize = 0x1000 }"));
    }
}
