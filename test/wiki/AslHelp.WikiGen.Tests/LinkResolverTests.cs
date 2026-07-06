using AslHelp.WikiGen.Api;
using AslHelp.WikiGen.Docfx;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class LinkResolverTests
{
    // ---- OpenForm ----

    [Test]
    public void OpenForm_NoGenerics_IsUnchanged()
    {
        Assert.That(LinkResolver.OpenForm("Foo.Bar"), Is.EqualTo("Foo.Bar"));
    }

    [Test]
    public void OpenForm_OneTypeArgument()
    {
        Assert.That(LinkResolver.OpenForm("Foo{T}"), Is.EqualTo("Foo`1"));
    }

    [Test]
    public void OpenForm_TwoTypeArguments()
    {
        Assert.That(LinkResolver.OpenForm("Dictionary{TKey,TValue}"), Is.EqualTo("Dictionary`2"));
    }

    [Test]
    public void OpenForm_NestedGenerics_CountsOnlyOuterArguments()
    {
        Assert.That(LinkResolver.OpenForm("List{Task{int}}"), Is.EqualTo("List`1"));
    }

    [Test]
    public void OpenForm_MultipleGenericSegments()
    {
        Assert.That(LinkResolver.OpenForm("Outer{T}.Inner{U,V}"), Is.EqualTo("Outer`1.Inner`2"));
    }

    // ---- Link ----

    [Test]
    public void Link_WithRef_ProducesMarkdownLink()
    {
        Assert.That(LinkResolver.Link(new ApiLink("Result", "Result-1")), Is.EqualTo("[Result](Result-1)"));
    }

    [Test]
    public void Link_WithoutRef_ProducesEscapedPlainText()
    {
        Assert.That(LinkResolver.Link(new ApiLink("List<int>", null)), Is.EqualTo("List&lt;int&gt;"));
    }
}
