using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AslHelp.WikiGen.Api;
using AslHelp.WikiGen.Docfx;
using AslHelp.WikiGen.Reflect;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class ApiModelBuilderBuildTests
{
    // A non-existent repo dir: DeclarationLine sees no file and falls back to the docfx line.
    private static string NoRepo()
    {
        return Path.Combine(Path.GetTempPath(), $"aslhelp_norepo_{Guid.NewGuid():N}");
    }

    private static List<ApiType> Run(DocfxModel model)
    {
        return ApiModelBuilder.Build(model, [], new XmlSummaries([]), NoRepo());
    }

    [Test]
    public void Build_EnumField_PopulatesConstantValueFromSyntax()
    {
        var model = Make.Model(items:
        [
            Make.Item("N.E", type: "Enum", name: "E", ns: "N", assemblies: ["Asm"], children: ["N.E.Ok"]),
            Make.Item("N.E.Ok", type: "Field", name: "Ok", syntaxContent: "Ok = 1"),
        ]);

        var ok = Run(model).Single().Members.Single();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok.Value, Is.EqualTo("1"));
            Assert.That(ok.Group, Is.EqualTo(MemberGroup.Fields));
            Assert.That(ok.Ref, Is.EqualTo("E.Ok"));
        }
    }

    [Test]
    public void Build_TypeRefAndFile_ComposedFromAssemblyAndNamespace()
    {
        var model = Make.Model(items:
        [
            Make.Item("N.Result`1", type: "Class", name: "Result", ns: "N", assemblies: ["Asm"]),
        ]);

        var type = Run(model).Single();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(type.Ref, Is.EqualTo("Result-1"));
            Assert.That(type.File, Is.EqualTo("Asm/N/Result-1"));
        }
    }

    [Test]
    public void Build_MemberRefAndFile_NestUnderType()
    {
        var model = Make.Model(items:
        [
            Make.Item("N.C", type: "Class", name: "C", ns: "N", assemblies: ["Asm"], children: ["N.C.Do"]),
            Make.Item("N.C.Do", type: "Method", name: "Do"),
        ]);

        var member = Run(model).Single().Members.Single();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(member.Ref, Is.EqualTo("C.Do"));
            Assert.That(member.File, Is.EqualTo("Asm/N/C/C.Do"));
        }
    }

    [Test]
    public void Build_PropertyMember_GroupedAndReffed()
    {
        var model = Make.Model(items:
        [
            Make.Item("N.C", type: "Class", name: "C", ns: "N", assemblies: ["Asm"], children: ["N.C.Size"]),
            Make.Item("N.C.Size", type: "Property", name: "Size"),
        ]);

        var member = Run(model).Single().Members.Single();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(member.Group, Is.EqualTo(MemberGroup.Properties));
            Assert.That(member.Ref, Is.EqualTo("C.Size"));
        }
    }

    [Test]
    public void Build_SourceWithRemote_BuildsGithubBlobUrl()
    {
        var model = Make.Model(items:
        [
            Make.Item("N.C", type: "Class", name: "C", ns: "N", assemblies: ["Asm"],
                source: Make.Source("https://github.com/o/r.git", "main", "src/C.cs", 0)),
        ]);

        var type = Run(model).Single();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(type.Source!.FileName, Is.EqualTo("C.cs"));
            Assert.That(type.Source.Url, Is.EqualTo("https://github.com/o/r/blob/main/src/C.cs#L1"));
        }
    }

    [Test]
    public void Build_NonEnumField_HasNullValue()
    {
        var model = Make.Model(items:
        [
            Make.Item("N.C", type: "Class", name: "C", ns: "N", assemblies: ["Asm"], children: ["N.C.X"]),
            Make.Item("N.C.X", type: "Field", name: "X", syntaxContent: "public int X"),
        ]);

        var member = Run(model).Single().Members.Single();

        Assert.That(member.Value, Is.Null);
    }
}
