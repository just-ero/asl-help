using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class DocfxModelTests
{
    // ---- DisplayName ----

    [Test]
    public void DisplayName_ReferenceWithName_PrefersReferenceName()
    {
        var model = Make.Model(
            items: [Make.Item("u", name: "ItemName")],
            references: [Make.Reference("u", "RefName")]);

        Assert.That(model.DisplayName("u"), Is.EqualTo("RefName"));
    }

    [Test]
    public void DisplayName_NoReference_FallsBackToItemName()
    {
        var model = Make.Model(items: [Make.Item("u", name: "ItemName")]);

        Assert.That(model.DisplayName("u"), Is.EqualTo("ItemName"));
    }

    [Test]
    public void DisplayName_Unknown_ReturnsUid()
    {
        var model = Make.Model();

        Assert.That(model.DisplayName("Unknown.Uid"), Is.EqualTo("Unknown.Uid"));
    }

    [Test]
    public void DisplayName_ReferenceWithNullName_FallsThroughToItem()
    {
        var model = Make.Model(
            items: [Make.Item("u", name: "ItemName")],
            references: [Make.Reference("u", name: null)]);

        Assert.That(model.DisplayName("u"), Is.EqualTo("ItemName"));
    }

    // ---- OfKind ----

    [Test]
    public void OfKind_FiltersByExactType()
    {
        var model = Make.Model(items:
        [
            Make.Item("a", type: "Class"),
            Make.Item("b", type: "Struct"),
            Make.Item("c", type: "Namespace"),
        ]);

        List<string> uids = [.. model.OfKind("Class", "Struct").Select(i => i.Uid)];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(uids, Has.Count.EqualTo(2));
            Assert.That(uids, Does.Contain("a"));
            Assert.That(uids, Does.Contain("b"));
            Assert.That(uids, Does.Not.Contain("c"));
        }
    }

    [Test]
    public void OfKind_IsOrdinalCaseSensitive()
    {
        var model = Make.Model(items: [Make.Item("a", type: "Class")]);

        Assert.That(model.OfKind("class"), Is.Empty);
    }
}
