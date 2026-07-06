using System.Collections.Generic;

using AslHelp.WikiGen.Api;
using AslHelp.WikiGen.Render;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class PagesTests
{
    private static ApiType Type(string kind, string display, IReadOnlyList<ApiMember> members)
    {
        return new("", display, "", display, kind, $"public {kind} {display}", "Ns", "Asm", null, null, [], [], null, members);
    }

    private static ApiMember EnumField(string typeRef, string name, string value, string? summary)
    {
        return new(name, MemberGroup.Fields, $"{name} = {value}", summary, null, [], null, null, null)
        {
            Ref = $"{typeRef}.{name}",
            Value = value,
        };
    }

    private static ApiMember Property(string typeRef, string name, ApiLink valueType, string? summary)
    {
        return new(name, MemberGroup.Properties, $"public T {name}", summary, valueType, [], null, null, null)
        {
            Ref = $"{typeRef}.{name}",
        };
    }

    [Test]
    public void Type_Enum_RendersValueColumnWithConstantValues()
    {
        var type = Type("Enum", "Status",
        [
            EnumField("Status", "Ok", "0", "All good"),
            EnumField("Status", "Error", "1", "Something failed"),
        ]);

        var page = Pages.Type(type);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("## Fields"));
            Assert.That(page, Does.Contain("| Name | Value | Summary |"));
            Assert.That(page, Does.Contain("| [Ok](Status.Ok) | `0` | All good |"));
            Assert.That(page, Does.Contain("| [Error](Status.Error) | `1` | Something failed |"));
            Assert.That(page, Does.Not.Contain("| Name | Type | Summary |"));
        }
    }

    [Test]
    public void Type_NonEnum_UsesTypeColumnNotValue()
    {
        var type = Type("Class", "Widget",
        [
            Property("Widget", "Size", new ApiLink("int", null), "The size"),
        ]);

        var page = Pages.Type(type);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(page, Does.Contain("## Properties"));
            Assert.That(page, Does.Contain("| Name | Type | Summary |"));
            Assert.That(page, Does.Not.Contain("| Name | Value | Summary |"));
        }
    }
}
