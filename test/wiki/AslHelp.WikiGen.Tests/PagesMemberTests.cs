using AslHelp.WikiGen.Api;
using AslHelp.WikiGen.Render;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class PagesMemberTests
{
    private static ApiType ResultType()
    {
        return Make.Type("Result", @ref: "Result-1", ns: "AslHelp");
    }

    [Test]
    public void Member_SingleOverload_RendersHeadingSummaryAndSignature()
    {
        var m = Make.Method("IsOk", "public bool IsOk()", "Checks.", @ref: "Result-1.IsOk");

        var page = Pages.Member(ResultType(), [m]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("# Result.IsOk"));
            Assert.That(page, Does.Contain("Checks."));
            Assert.That(page, Does.Contain("public bool IsOk()"));
            Assert.That(page, Does.Not.Contain("## Overload 1"));
        }
    }

    [Test]
    public void Member_Breadcrumb_LinksNamespaceAndOwningType()
    {
        var m = Make.Method("IsOk", @ref: "Result-1.IsOk");

        var page = Pages.Member(ResultType(), [m]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("[AslHelp](AslHelp)"));
            Assert.That(page, Does.Contain("[Result](Result-1)"));
        }
    }

    [Test]
    public void Member_MultipleOverloads_EmitsOverloadHeadings()
    {
        var m1 = Make.Method("Map", "public T Map()", @ref: "Result-1.Map");
        var m2 = Make.Method("Map", "public U Map<U>()", @ref: "Result-1.Map");

        var page = Pages.Member(ResultType(), [m1, m2]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("## Overload 1"));
            Assert.That(page, Does.Contain("## Overload 2"));
        }
    }

    [Test]
    public void Member_WithParameters_RendersParameterTable()
    {
        var m = Make.Method(
            "Do",
            parameters: [new ApiParameter("x", Make.Link("int"), "the input")],
            @ref: "Result-1.Do");

        var page = Pages.Member(ResultType(), [m]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("| Parameter | Type | Description |"));
            Assert.That(page, Does.Contain("| `x` | int | the input |"));
        }
    }

    [Test]
    public void Member_WithReturnTypeAndSummary_RendersReturnsWithDash()
    {
        var m = Make.Method("Do", returnType: Make.Link("bool"), returnSummary: "true if ok", @ref: "Result-1.Do");

        Assert.That(Pages.Member(ResultType(), [m]), Does.Contain("**Returns:** bool — true if ok"));
    }

    [Test]
    public void Member_WithReturnTypeNoSummary_RendersReturnsWithoutDash()
    {
        var m = Make.Method("Do", returnType: Make.Link("bool"), @ref: "Result-1.Do");

        var page = Pages.Member(ResultType(), [m]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("**Returns:** bool"));
            Assert.That(page, Does.Not.Contain(" — "));
        }
    }

    [Test]
    public void Member_PropertyWithValueType_RendersValueLine()
    {
        var m = Make.Property("Length", Make.Link("string"), @ref: "Result-1.Length");

        Assert.That(Pages.Member(ResultType(), [m]), Does.Contain("**Value:** string"));
    }

    [Test]
    public void Member_SingleOverloadWithSource_RendersSourceArrowInHeading()
    {
        var m = Make.Method("IsOk", source: Make.Src("Result.cs", "http://src/isok"), @ref: "Result-1.IsOk");

        Assert.That(Pages.Member(ResultType(), [m]), Does.Contain("href=\"http://src/isok\""));
    }

    [Test]
    public void Member_MultipleOverloads_RendersPerOverloadSourceLine()
    {
        var m1 = Make.Method("Map", @ref: "Result-1.Map");
        var m2 = Make.Method("Map", source: Make.Src("Result.cs", "http://src/map2"), @ref: "Result-1.Map");

        Assert.That(Pages.Member(ResultType(), [m1, m2]), Does.Contain("**Source:** [Result.cs](http://src/map2)"));
    }
}
