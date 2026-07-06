using NUnit.Framework;

namespace AslHelp.LiveSplit.Tests;

[TestFixture]
public class ScriptActionTests
{
    // The methods table is only dereferenced by Recompile(), so a null is safe for these tests.

    [Test]
    public void EmptyConstructor_StartsWithEmptyBody()
    {
        ScriptAction action = new(null!, "startup");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(action.Name, Is.EqualTo("startup"));
            Assert.That(action.Body, Is.EqualTo(""));
        }
    }

    [Test]
    public void Append_AppendsToBodyAndReturnsSelf()
    {
        ScriptAction action = new(null!, "update", "a", 1, null);

        var returned = action.Append("b");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(action.Body, Is.EqualTo("ab"));
            Assert.That(returned, Is.SameAs(action));
        }
    }

    [Test]
    public void Prepend_PrependsToBodyAndReturnsSelf()
    {
        ScriptAction action = new(null!, "update", "a", 1, null);

        var returned = action.Prepend("b");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(action.Body, Is.EqualTo("ba"));
            Assert.That(returned, Is.SameAs(action));
        }
    }

    [Test]
    public void AppendAndPrepend_Chain_WrapTheOriginalBody()
    {
        ScriptAction action = new(null!, "update", "core", 1, null);

        action.Prepend("before ").Append(" after");

        Assert.That(action.Body, Is.EqualTo("before core after"));
    }

    [Test]
    public void ToString_FormatsNameAndLine()
    {
        ScriptAction action = new(null!, "split", "body", 7, null);

        Assert.That(action.ToString(), Is.EqualTo("split (line 7)"));
    }
}
