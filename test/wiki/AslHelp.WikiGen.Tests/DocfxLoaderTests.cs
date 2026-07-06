using System;
using System.IO;

using AslHelp.WikiGen.Docfx;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class DocfxLoaderTests
{
    private string _dir = "";

    [SetUp]
    public void CreateDir()
    {
        _dir = Path.Combine(Path.GetTempPath(), $"aslhelp_docfx_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_dir);
    }

    [TearDown]
    public void DeleteDir()
    {
        if (Directory.Exists(_dir))
        {
            Directory.Delete(_dir, recursive: true);
        }
    }

    private void Write(string fileName, string yaml)
    {
        File.WriteAllText(Path.Combine(_dir, fileName), yaml);
    }

    [Test]
    public void Load_SkipsTocYml()
    {
        Write("a.yml", "items:\n- uid: N.C\n  type: Class\n  name: C\n");
        Write("toc.yml", "items:\n- uid: N.Skip\n  type: Class\n  name: Skip\n");

        var model = DocfxLoader.Load(_dir);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(model.Items.ContainsKey("N.C"), Is.True);
            Assert.That(model.Items.ContainsKey("N.Skip"), Is.False);
        }
    }

    [Test]
    public void Load_ItemsLastWins_OnDuplicateUid()
    {
        Write("a.yml", "items:\n- uid: N.C\n  type: Class\n  name: First\n- uid: N.C\n  type: Class\n  name: Second\n");

        var model = DocfxLoader.Load(_dir);

        Assert.That(model.Items["N.C"].Name, Is.EqualTo("Second"));
    }

    [Test]
    public void Load_ReferencesFirstWins_OnDuplicateUid()
    {
        Write("a.yml", "references:\n- uid: R\n  name: First\n- uid: R\n  name: Second\n");

        var model = DocfxLoader.Load(_dir);

        Assert.That(model.References["R"].Name, Is.EqualTo("First"));
    }
}
