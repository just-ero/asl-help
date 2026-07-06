using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AslHelp.LiveSplit.Settings;

using LiveSplit.ASL;

using NUnit.Framework;

namespace AslHelp.LiveSplit.Tests;

// Drives SettingsBuilder end to end through a real ASLSettings.Builder and inspects the resulting
// settings tree. CA1814: the public API takes a multidimensional array, so the tests must too.
#pragma warning disable CA1814
[TestFixture]
public class SettingsBuilderTests
{
    private readonly List<string> _tempFiles = [];

    private static (ASLSettings Settings, SettingsBuilder Builder) NewBuilder()
    {
        ASLSettings settings = new();
        return (settings, new SettingsBuilder(settings.Builder));
    }

    private static ASLSetting Get(ASLSettings settings, string id)
    {
        return settings.Settings[id];
    }

    private string TempFile(string content, string extension)
    {
        var path = Path.Combine(Path.GetTempPath(), $"asl-help_settings_{Guid.NewGuid():N}{extension}");
        File.WriteAllText(path, content);
        _tempFiles.Add(path);

        return path;
    }

    [TearDown]
    public void DeleteTempFiles()
    {
        foreach (var path in _tempFiles)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        _tempFiles.Clear();
    }

    // ---- Add(Dictionary) ----

    [Test]
    public void AddDictionary_UsesKeyAsIdAndValueAsLabel()
    {
        var (settings, builder) = NewBuilder();

        builder.Add(new Dictionary<string, string> { ["enable"] = "Enable feature" });

        var setting = Get(settings, "enable");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(setting.Label, Is.EqualTo("Enable feature"));
            Assert.That(setting.DefaultValue, Is.False);
            Assert.That(setting.Parent, Is.Null);
        }
    }

    // ---- Add(dynamic?[,]) arities ----

    [Test]
    public void AddDynamic_OneColumn_ReusesIdAsLabel()
    {
        var (settings, builder) = NewBuilder();

        builder.Add(new dynamic?[,] { { "solo" } });

        var setting = Get(settings, "solo");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(setting.Label, Is.EqualTo("solo"));
            Assert.That(setting.DefaultValue, Is.False);
        }
    }

    [Test]
    public void AddDynamic_TwoColumns_AreIdAndLabel()
    {
        var (settings, builder) = NewBuilder();

        builder.Add(new dynamic?[,] { { "id", "Label" } });

        var setting = Get(settings, "id");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(setting.Label, Is.EqualTo("Label"));
            Assert.That(setting.DefaultValue, Is.False);
        }
    }

    [Test]
    public void AddDynamic_ThreeColumns_AreIdStateParent_AndReuseIdAsLabel()
    {
        var (settings, builder) = NewBuilder();

        builder.Add(new dynamic?[,] { { "root" } });
        builder.Add(new dynamic?[,] { { "child", true, "root" } });

        var setting = Get(settings, "child");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(setting.Label, Is.EqualTo("child"));
            Assert.That(setting.DefaultValue, Is.True);
            Assert.That(setting.Parent, Is.EqualTo("root"));
        }
    }

    [Test]
    public void AddDynamic_FourColumns_AreIdStateLabelParent()
    {
        var (settings, builder) = NewBuilder();

        builder.Add(new dynamic?[,] { { "root" } });
        builder.Add(new dynamic?[,] { { "id", true, "Label", "root" } });

        var setting = Get(settings, "id");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(setting.Label, Is.EqualTo("Label"));
            Assert.That(setting.DefaultValue, Is.True);
            Assert.That(setting.Parent, Is.EqualTo("root"));
        }
    }

    [Test]
    public void AddDynamic_FiveColumns_IncludeTooltip()
    {
        var (settings, builder) = NewBuilder();

        builder.Add(new dynamic?[,] { { "id", false, "Label", null, "Helpful tip" } });

        var setting = Get(settings, "id");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(setting.Label, Is.EqualTo("Label"));
            Assert.That(setting.ToolTip, Is.EqualTo("Helpful tip"));
        }
    }

    [Test]
    public void AddDynamic_WhitespaceTooltip_IsNotSet()
    {
        var (settings, builder) = NewBuilder();

        builder.Add(new dynamic?[,] { { "id", false, "Label", null, "   " } });

        Assert.That(Get(settings, "id").ToolTip, Is.Null);
    }

    // ---- Add(dynamic?[,]) validation ----

    [Test]
    public void AddDynamic_ZeroColumns_Throws()
    {
        (_, var builder) = NewBuilder();

        Assert.That(() => builder.Add(new dynamic?[1, 0]), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void AddDynamic_MoreThanFiveColumns_Throws()
    {
        (_, var builder) = NewBuilder();

        Assert.That(
            () => builder.Add(new dynamic?[,] { { 1, 2, 3, 4, 5, 6 } }),
            Throws.InstanceOf<ArgumentException>());
    }

    // ---- real-pipeline behaviour ----

    [Test]
    public void Add_DuplicateId_ThrowsFromLiveSplit()
    {
        (_, var builder) = NewBuilder();

        builder.Add(new Dictionary<string, string> { ["dup"] = "First" });

        Assert.That(
            () => builder.Add(new Dictionary<string, string> { ["dup"] = "Second" }),
            Throws.InstanceOf<ArgumentException>());
    }

    // ---- FromJson ----

    [Test]
    public void FromJson_NestedSettings_FlattenWithParentRefsInOrder()
    {
        var (settings, builder) = NewBuilder();
        var json = """
            {
              "parent": {
                "label": "Parent",
                "state": true,
                "tooltip": "Parent tip",
                "children": {
                  "child": { "label": "Child", "state": false }
                }
              }
            }
            """;
        string[] expectedOrder = ["parent", "child"];

        builder.FromJson(TempFile(json, ".json"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(settings.OrderedSettings.Select(s => s.Id), Is.EqualTo(expectedOrder));
            Assert.That(Get(settings, "parent").DefaultValue, Is.True);
            Assert.That(Get(settings, "parent").ToolTip, Is.EqualTo("Parent tip"));
            Assert.That(Get(settings, "child").Parent, Is.EqualTo("parent"));
            Assert.That(Get(settings, "child").Label, Is.EqualTo("Child"));
        }
    }

    [Test]
    public void FromJson_MissingLabel_FallsBackToId()
    {
        var (settings, builder) = NewBuilder();

        builder.FromJson(TempFile("""{ "noLabel": { "state": true } }""", ".json"));

        Assert.That(Get(settings, "noLabel").Label, Is.EqualTo("noLabel"));
    }

    [Test]
    public void FromJson_PropertyNamesAreCaseInsensitive()
    {
        var (settings, builder) = NewBuilder();

        builder.FromJson(TempFile("""{ "x": { "Label": "Upper", "State": true } }""", ".json"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Get(settings, "x").Label, Is.EqualTo("Upper"));
            Assert.That(Get(settings, "x").DefaultValue, Is.True);
        }
    }

    [Test]
    public void FromJson_EmptyObject_AddsNothing()
    {
        var (settings, builder) = NewBuilder();

        builder.FromJson(TempFile("{}", ".json"));

        Assert.That(settings.OrderedSettings, Is.Empty);
    }

    // ---- FromXml ----

    [Test]
    public void FromXml_NestedSettings_FlattenWithParentRefsInOrder()
    {
        var (settings, builder) = NewBuilder();
        var xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <Settings>
              <Setting Id="parent" Label="Parent" State="true" ToolTip="Parent tip">
                <Setting Id="child" Label="Child" State="false" />
              </Setting>
            </Settings>
            """;
        string[] expectedOrder = ["parent", "child"];

        builder.FromXml(TempFile(xml, ".xml"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(settings.OrderedSettings.Select(s => s.Id), Is.EqualTo(expectedOrder));
            Assert.That(Get(settings, "parent").ToolTip, Is.EqualTo("Parent tip"));
            Assert.That(Get(settings, "child").Parent, Is.EqualTo("parent"));
        }
    }

    [Test]
    public void FromXml_UnparseableState_DefaultsToFalse()
    {
        var (settings, builder) = NewBuilder();

        builder.FromXml(TempFile("""<Settings><Setting Id="x" Label="X" State="notabool" /></Settings>""", ".xml"));

        Assert.That(Get(settings, "x").DefaultValue, Is.False);
    }

    [Test]
    public void FromXml_MissingLabel_FallsBackToId()
    {
        var (settings, builder) = NewBuilder();

        builder.FromXml(TempFile("""<Settings><Setting Id="bare" /></Settings>""", ".xml"));

        Assert.That(Get(settings, "bare").Label, Is.EqualTo("bare"));
    }
}
