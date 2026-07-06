using NUnit.Framework;

namespace AslHelp.LiveSplit.Tests;

[TestFixture]
public class AttachErrorTests
{
    [Test]
    public void ScriptComponentNotFound_ListsEveryCandidate()
    {
        var error = AttachError.ScriptComponentNotFound(["a.asl", "b.asl"]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(error.Message, Does.Contain("a.asl"));
            Assert.That(error.Message, Does.Contain("b.asl"));
        }
    }

    [Test]
    public void OutsideStartup_IncludesTheActionName()
    {
        Assert.That(AttachError.OutsideStartup("update").Message, Does.Contain("'update'"));
    }

    [Test]
    public void LiveSplitInternalsChanged_IncludesTheMember()
    {
        Assert.That(
            AttachError.LiveSplitInternalsChanged("ASLSettings.Builder").Message,
            Does.Contain("ASLSettings.Builder"));
    }
}
