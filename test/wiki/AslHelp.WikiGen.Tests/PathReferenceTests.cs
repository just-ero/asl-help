using System;
using System.IO;
using System.Linq;

using AslHelp.WikiGen.Api;
using AslHelp.WikiGen.Reflect;
using AslHelp.WikiGen.Render;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

// End-to-end path-reference correctness: the Ref/File that ApiModelBuilder.Build computes for a
// type/member is exactly the link target every renderer (Type/Member/Namespace/Sidebar) points at.
[TestFixture]
public class PathReferenceTests
{
    private static ApiType BuildResultType()
    {
        var model = Make.Model(items:
        [
            Make.Item("N.Result`1", type: "Class", name: "Result", ns: "N", assemblies: ["Asm"], children: ["N.Result`1.IsOk"]),
            Make.Item("N.Result`1.IsOk", type: "Method", name: "IsOk"),
        ]);

        var noRepo = Path.Combine(Path.GetTempPath(), $"aslhelp_norepo_{Guid.NewGuid():N}");
        return ApiModelBuilder.Build(model, [], new XmlSummaries([]), noRepo).Single();
    }

    [Test]
    public void Build_GenericType_RefAndMemberPathsAreCorrect()
    {
        var type = BuildResultType();
        var member = type.Members.Single();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(type.Ref, Is.EqualTo("Result-1"));
            Assert.That(type.File, Is.EqualTo("Asm/N/Result-1"));
            Assert.That(member.Ref, Is.EqualTo("Result-1.IsOk"));
            Assert.That(member.File, Is.EqualTo("Asm/N/Result-1/Result-1.IsOk"));
        }
    }

    [Test]
    public void NamespacePage_LinksTypeAtItsRef()
    {
        var type = BuildResultType();

        Assert.That(Pages.Namespace(type.Namespace, [type]), Does.Contain($"[{type.Display}]({type.Ref})"));
    }

    [Test]
    public void Sidebar_LinksTypeAtItsRef()
    {
        var type = BuildResultType();

        Assert.That(SidebarBuilder.Build([type], SidebarStyle.Nested), Does.Contain($"[{type.Display}]({type.Ref})"));
    }

    [Test]
    public void TypePage_LinksMemberAtItsRef()
    {
        var type = BuildResultType();
        var member = type.Members.Single();

        Assert.That(Pages.Type(type), Does.Contain($"({member.Ref})"));
    }

    [Test]
    public void MemberPage_BreadcrumbLinksOwningTypeAtItsRef()
    {
        var type = BuildResultType();
        var member = type.Members.Single();

        Assert.That(Pages.Member(type, [member]), Does.Contain($"[{type.Display}]({type.Ref})"));
    }
}
