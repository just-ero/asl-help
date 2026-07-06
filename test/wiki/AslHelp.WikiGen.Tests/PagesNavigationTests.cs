using System;

using AslHelp.WikiGen.Api;
using AslHelp.WikiGen.Render;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class PagesNavigationTests
{
    private static int Index(string haystack, string needle)
    {
        return haystack.IndexOf(needle, StringComparison.Ordinal);
    }

    private static int LastIndex(string haystack, string needle)
    {
        return haystack.LastIndexOf(needle, StringComparison.Ordinal);
    }

    // ---- Namespace ----

    [Test]
    public void Namespace_RendersHeadingAndClassRow()
    {
        var page = Pages.Namespace("AslHelp", [Make.Type("Result", @ref: "Result-1")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("# AslHelp namespace"));
            Assert.That(page, Does.Contain("## Classes"));
            Assert.That(page, Does.Contain("| [Result](Result-1) |"));
        }
    }

    [Test]
    public void Namespace_GroupsAllKindsInFixedOrder()
    {
        ApiType[] types =
        [
            Make.Type("D", kind: "Delegate"),
            Make.Type("E", kind: "Enum"),
            Make.Type("I", kind: "Interface"),
            Make.Type("S", kind: "Struct"),
            Make.Type("C", kind: "Class"),
        ];

        var page = Pages.Namespace("AslHelp", types);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Index(page, "## Classes"), Is.LessThan(Index(page, "## Structs")));
            Assert.That(Index(page, "## Structs"), Is.LessThan(Index(page, "## Interfaces")));
            Assert.That(Index(page, "## Interfaces"), Is.LessThan(Index(page, "## Enums")));
            Assert.That(Index(page, "## Enums"), Is.LessThan(Index(page, "## Delegates")));
        }
    }

    [Test]
    public void Namespace_EmptyKind_OmitsHeading()
    {
        var page = Pages.Namespace("AslHelp", [Make.Type("C", kind: "Class")]);

        Assert.That(page, Does.Not.Contain("## Enums"));
    }

    [Test]
    public void Namespace_SortsTypesOrdinallyWithinKind()
    {
        ApiType[] types = [Make.Type("Zebra", @ref: "Zebra"), Make.Type("Alpha", @ref: "Alpha")];

        var page = Pages.Namespace("AslHelp", types);

        Assert.That(Index(page, "[Alpha]"), Is.LessThan(Index(page, "[Zebra]")));
    }

    // ---- Hub ----

    [Test]
    public void Hub_RendersHeadingAssemblyAndNamespaceBullet()
    {
        var page = Pages.Hub([Make.Type("Result", ns: "AslHelp", assembly: "AslHelp")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("# API Reference"));
            Assert.That(page, Does.Contain("### AslHelp"));
            Assert.That(page, Does.Contain("- [AslHelp](AslHelp)"));
        }
    }

    [Test]
    public void Hub_GroupsAssembliesOrdinally()
    {
        ApiType[] types = [Make.Type("Z", assembly: "Zeta", ns: "Zeta"), Make.Type("A", assembly: "Alpha", ns: "Alpha")];

        var page = Pages.Hub(types);

        Assert.That(Index(page, "### Alpha"), Is.LessThan(Index(page, "### Zeta")));
    }

    [Test]
    public void Hub_DistinctNamespacesPerAssembly()
    {
        ApiType[] types =
        [
            Make.Type("A", assembly: "Asm", ns: "N"),
            Make.Type("B", assembly: "Asm", ns: "N"),
        ];

        var page = Pages.Hub(types);

        // The single namespace bullet must appear exactly once.
        Assert.That(Index(page, "- [N](N)"), Is.EqualTo(LastIndex(page, "- [N](N)")));
    }

    // ---- Home ----

    [Test]
    public void HomeApi_EmitsLinkedHubHeaderAndTree()
    {
        var page = Pages.HomeApi([Make.Type("Result", ns: "AslHelp", assembly: "AslHelp")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("## [API Reference](Documentation)"));
            Assert.That(page, Does.Contain("### AslHelp"));
            Assert.That(page, Does.Contain("- [AslHelp](AslHelp)"));
        }
    }

    [Test]
    public void HomeScaffold_IncludesTitleAndWelcomeLine()
    {
        var page = Pages.HomeScaffold("AslHelp");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("# AslHelp"));
            Assert.That(page, Does.Contain("Welcome to the AslHelp wiki."));
        }
    }
}
