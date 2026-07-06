using System.Collections.Generic;
using AslHelp.WikiGen.Docfx;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class LinkResolverInstanceTests
{
    private static LinkResolver WithPaths(params (string Uid, string Path)[] entries)
    {
        Dictionary<string, string> paths = [];
        foreach (var (uid, path) in entries)
        {
            paths[uid] = path;
        }

        return Make.Resolver(paths);
    }

    // ---- ResolveToPath (path-reference resolution) ----

    [Test]
    public void ResolveToPath_DirectHit_ReturnsPath()
    {
        var resolver = WithPaths(("AslHelp.Result`1", "Result-1"));

        Assert.That(resolver.ResolveToPath("AslHelp.Result`1"), Is.EqualTo("Result-1"));
    }

    [Test]
    public void ResolveToPath_MemberUid_FallsBackToOwningType()
    {
        var resolver = WithPaths(("AslHelp.Result`1", "Result-1"));

        Assert.That(resolver.ResolveToPath("AslHelp.Result`1.IsOk"), Is.EqualTo("Result-1"));
    }

    [Test]
    public void ResolveToPath_MethodWithParamsAndArity_StripsToOwningType()
    {
        var resolver = WithPaths(("AslHelp.ResultExtensions", "ResultExtensions"));

        Assert.That(
            resolver.ResolveToPath("AslHelp.ResultExtensions.Map``1(AslHelp.Result,System.Func{System.Int32})"),
            Is.EqualTo("ResultExtensions"));
    }

    [Test]
    public void ResolveToPath_ConstructedGeneric_OpenedBeforeLookup()
    {
        var resolver = WithPaths(("AslHelp.Result`1", "Result-1"));

        Assert.That(resolver.ResolveToPath("AslHelp.Result{System.Int32}"), Is.EqualTo("Result-1"));
    }

    [Test]
    public void ResolveToPath_Unknown_ReturnsNull()
    {
        var resolver = Make.Resolver(new Dictionary<string, string>());

        Assert.That(resolver.ResolveToPath("System.String"), Is.Null);
    }

    [Test]
    public void ResolveToPath_PercentEncodedBacktick_IsUnescaped()
    {
        var resolver = WithPaths(("AslHelp.Result`1", "Result-1"));

        Assert.That(resolver.ResolveToPath("AslHelp.Result%601"), Is.EqualTo("Result-1"));
    }

    // ---- TypeLink ----

    [Test]
    public void TypeLink_KnownType_LinksDisplayNameToPath()
    {
        var model = Make.Model(references: [Make.Reference("AslHelp.Result`1", "Result<T>")]);
        Dictionary<string, string> paths = new() { ["AslHelp.Result`1"] = "Result-1" };
        var resolver = Make.Resolver(paths, model);

        var link = resolver.TypeLink("AslHelp.Result`1");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(link.Display, Is.EqualTo("Result<T>"));
            Assert.That(link.Ref, Is.EqualTo("Result-1"));
        }
    }

    [Test]
    public void TypeLink_UnknownType_DisplayIsUidAndRefIsNull()
    {
        var resolver = Make.Resolver(new Dictionary<string, string>());

        var link = resolver.TypeLink("System.Int32");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(link.Display, Is.EqualTo("System.Int32"));
            Assert.That(link.Ref, Is.Null);
        }
    }

    // ---- ToMarkdown ----

    [Test]
    public void ToMarkdown_Whitespace_ReturnsEmpty()
    {
        var resolver = Make.Resolver(new Dictionary<string, string>());

        Assert.That(resolver.ToMarkdown("   "), Is.EqualTo(""));
    }

    [Test]
    public void ToMarkdown_InlineCode_BecomesBackticks()
    {
        var resolver = Make.Resolver(new Dictionary<string, string>());

        Assert.That(resolver.ToMarkdown("use <code>Foo</code> here"), Is.EqualTo("use `Foo` here"));
    }

    [Test]
    public void ToMarkdown_RefCode_BecomesItalic()
    {
        var resolver = Make.Resolver(new Dictionary<string, string>());

        Assert.That(resolver.ToMarkdown("<code class=\"paramref\">value</code>"), Is.EqualTo("*value*"));
    }

    [Test]
    public void ToMarkdown_Anchor_BecomesMarkdownLink()
    {
        var resolver = Make.Resolver(new Dictionary<string, string>());

        Assert.That(resolver.ToMarkdown("<a href=\"http://x\">site</a>"), Is.EqualTo("[site](http://x)"));
    }

    [Test]
    public void ToMarkdown_Xref_ResolvesViaModelAndPaths()
    {
        var model = Make.Model(references: [Make.Reference("AslHelp.Result`1", "Result")]);
        Dictionary<string, string> paths = new() { ["AslHelp.Result`1"] = "Result-1" };
        var resolver = Make.Resolver(paths, model);

        var md = resolver.ToMarkdown("<xref href=\"AslHelp.Result%601\" data-throw-if-not-resolved=\"true\"></xref>");

        Assert.That(md, Is.EqualTo("[Result](Result-1)"));
    }

    [Test]
    public void ToMarkdown_Paragraphs_CollapseToSpaces()
    {
        var resolver = Make.Resolver(new Dictionary<string, string>());

        Assert.That(resolver.ToMarkdown("<p>Hello</p><p>World</p>"), Is.EqualTo("Hello World"));
    }
}
