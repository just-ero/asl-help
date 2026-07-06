using System;
using System.Collections.Generic;

using AslHelp.WikiGen.Api;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class SidebarBuilderTests
{
    private static ApiType Type(string assembly, string ns, string display, string @ref)
    {
        return new("", @ref, "", display, "Class", "", ns, assembly, null, null, [], [], null, []);
    }

    [Test]
    public void Build_Nested_ListsHubAssemblyNamespaceAndType()
    {
        List<ApiType> types = [Type("AslHelp", "AslHelp", "Result", "Result-1")];

        var sidebar = SidebarBuilder.Build(types, SidebarStyle.Nested);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sidebar, Does.Contain("- [API Reference](Documentation)"));
            Assert.That(sidebar, Does.Contain("- **AslHelp**"));
            Assert.That(sidebar, Does.Contain("  - [AslHelp](AslHelp)"));
            Assert.That(sidebar, Does.Contain("    - [Result](Result-1)"));
            Assert.That(sidebar, Does.Not.Contain("<details>"));
        }
    }

    [Test]
    public void Build_Collapsible_WrapsTreeInDetails()
    {
        List<ApiType> types = [Type("AslHelp", "AslHelp", "Result", "Result-1")];

        var sidebar = SidebarBuilder.Build(types, SidebarStyle.Collapsible);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sidebar, Does.Contain("<details>"));
            Assert.That(sidebar, Does.Contain("<summary><strong>API Reference</strong></summary>"));
            Assert.That(sidebar, Does.Contain("</details>"));
            Assert.That(sidebar, Does.Not.Contain("- [API Reference](Documentation)"));
        }
    }

    [Test]
    public void Build_SortsTypesWithinNamespaceOrdinally()
    {
        List<ApiType> types =
        [
            Type("AslHelp", "AslHelp", "Zebra", "Zebra"),
            Type("AslHelp", "AslHelp", "Alpha", "Alpha"),
        ];

        var sidebar = SidebarBuilder.Build(types, SidebarStyle.Nested);

        Assert.That(
            sidebar.IndexOf("[Alpha]", StringComparison.Ordinal),
            Is.LessThan(sidebar.IndexOf("[Zebra]", StringComparison.Ordinal)));
    }
}
