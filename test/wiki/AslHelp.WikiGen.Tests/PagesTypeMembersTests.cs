using System;

using AslHelp.WikiGen.Api;
using AslHelp.WikiGen.Render;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class PagesTypeMembersTests
{
    private static int Index(string haystack, string needle)
    {
        return haystack.IndexOf(needle, StringComparison.Ordinal);
    }

    // ---- kind heading (page generation across kinds) ----

    [TestCase("Class", "class")]
    [TestCase("Struct", "struct")]
    [TestCase("Interface", "interface")]
    [TestCase("Enum", "enum")]
    [TestCase("Delegate", "delegate")]
    public void Type_Heading_LowercasesKind(string kind, string expected)
    {
        var page = Pages.Type(Make.Type("Foo", kind: kind));

        Assert.That(page, Does.Contain($"# Foo {expected}"));
    }

    // ---- breadcrumb + metadata + signature ----

    [Test]
    public void Type_RendersBreadcrumbMetadataAndSignatureFence()
    {
        var type = Make.Type("Result", ns: "AslHelp", assembly: "AslHelp");

        var page = Pages.Type(type);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("[Home](Home)"));
            Assert.That(page, Does.Contain("[API Reference](Documentation)"));
            Assert.That(page, Does.Contain("[AslHelp](AslHelp)"));
            Assert.That(page, Does.Contain("**Assembly:** AslHelp"));
            Assert.That(page, Does.Contain("**Namespace:** [AslHelp](AslHelp)"));
            Assert.That(page, Does.Contain("```csharp"));
            Assert.That(page, Does.Contain("public Class Result"));
        }
    }

    // ---- member groups: order and column shape ----

    [Test]
    public void Type_MemberGroups_RenderInGroupOrder()
    {
        var type = Make.Type("T", kind: "Class", members:
        [
            Make.Member("Evt", MemberGroup.Events, "T.Evt"),
            Make.Member("Mth", MemberGroup.Methods, "T.Mth"),
            Make.Member("Prop", MemberGroup.Properties, "T.Prop"),
            Make.Member("Ctor", MemberGroup.Constructors, "T.ctor"),
            Make.Member("Fld", MemberGroup.Fields, "T.Fld"),
            Make.Member("Op", MemberGroup.Operators, "T.Op"),
        ]);

        var page = Pages.Type(type);

        // _groupOrder: Constructors, Properties, Fields, Methods, Operators, Events.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Index(page, "## Constructors"), Is.LessThan(Index(page, "## Properties")));
            Assert.That(Index(page, "## Properties"), Is.LessThan(Index(page, "## Fields")));
            Assert.That(Index(page, "## Fields"), Is.LessThan(Index(page, "## Methods")));
            Assert.That(Index(page, "## Methods"), Is.LessThan(Index(page, "## Operators")));
            Assert.That(Index(page, "## Operators"), Is.LessThan(Index(page, "## Events")));
        }
    }

    [Test]
    public void Type_NonValuedGroup_UsesTwoColumnHeader()
    {
        var type = Make.Type("T", kind: "Class", members: [Make.Member("Do", MemberGroup.Methods, "T.Do")]);

        var page = Pages.Type(type);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("| Name | Summary |"));
            Assert.That(page, Does.Not.Contain("| Name | Type | Summary |"));
        }
    }

    // ---- member row link target (path-reference correctness in the rendered page) ----

    [Test]
    public void Type_MemberRow_LinksDisplayToMemberRef()
    {
        var type = Make.Type("T", kind: "Class", members: [Make.Member("Do", MemberGroup.Methods, "T.Do", summary: "does it")]);

        var page = Pages.Type(type);

        Assert.That(page, Does.Contain("| [Do](T.Do) | does it |"));
    }

    [Test]
    public void Type_DuplicateMemberRef_RendersRowOnce()
    {
        var type = Make.Type("Widget", kind: "Class", members:
        [
            Make.Member("Do", MemberGroup.Methods, "Widget.Do", summary: "first"),
            Make.Member("Do", MemberGroup.Methods, "Widget.Do", summary: "second"),
        ]);

        var page = Pages.Type(type);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("first"));
            Assert.That(page, Does.Not.Contain("second"));
        }
    }

    // ---- inheritance / implements / remarks ----

    [Test]
    public void Type_WithInheritance_RendersChainEndingInSelf()
    {
        var type = Make.Type("Result", inheritance: [Make.Link("Object", "obj")]);

        Assert.That(Pages.Type(type), Does.Contain("**Inheritance:** [Object](obj) → Result"));
    }

    [Test]
    public void Type_WithImplements_RendersLinkedAndPlainInterfaces()
    {
        var type = Make.Type("Result", implements: [Make.Link("IFoo", "IFoo-ref"), Make.Link("IBar")]);

        Assert.That(Pages.Type(type), Does.Contain("**Implements:** [IFoo](IFoo-ref), IBar"));
    }

    [Test]
    public void Type_WithRemarks_EmitsRemarksSection()
    {
        var type = Make.Type("Result", remarks: "Some remarks.");

        var page = Pages.Type(type);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("## Remarks"));
            Assert.That(page, Does.Contain("Some remarks."));
        }
    }

    [Test]
    public void Type_NoRemarks_OmitsRemarksSection()
    {
        Assert.That(Pages.Type(Make.Type("Result")), Does.Not.Contain("## Remarks"));
    }

    // ---- source link ----

    [Test]
    public void Type_WithSource_RendersSourceLinkInHeading()
    {
        var type = Make.Type("Result", source: Make.Src("Result.cs", "http://src/result"));

        Assert.That(Pages.Type(type), Does.Contain("href=\"http://src/result\""));
    }
}
