using System;
using System.Collections.Generic;
using System.IO;

using AslHelp.WikiGen.Reflect;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class XmlSummariesTests
{
    private readonly List<string> _tempFiles = [];

    private XmlSummaries Load(string membersXml)
    {
        var path = Path.Combine(Path.GetTempPath(), $"aslhelp_xmldoc_{Guid.NewGuid():N}.xml");
        File.WriteAllText(path, $"<doc><members>{membersXml}</members></doc>");
        _tempFiles.Add(path);

        return new XmlSummaries([path]);
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

    [Test]
    public void For_PlainSummary_CollapsesWhitespaceAndTrims()
    {
        var summaries = Load("""<member name="M:AslHelp.Foo.Bar"><summary>  Does   a thing.  </summary></member>""");

        Assert.That(summaries.For("AslHelp.Foo", "Bar"), Is.EqualTo("Does a thing."));
    }

    [Test]
    public void For_KeyDropsParametersAndArity()
    {
        var summaries = Load(
            """<member name="M:AslHelp.ResultExtensions.Map``1(AslHelp.Result,``0)"><summary>Maps it.</summary></member>""");

        Assert.That(summaries.For("AslHelp.ResultExtensions", "Map"), Is.EqualTo("Maps it."));
    }

    [Test]
    public void For_CElement_BecomesBackticks()
    {
        var summaries = Load("""<member name="P:AslHelp.Foo.Bar"><summary>value <c>true</c> ok</summary></member>""");

        Assert.That(summaries.For("AslHelp.Foo", "Bar"), Is.EqualTo("value `true` ok"));
    }

    [Test]
    public void For_Paramref_BecomesItalic()
    {
        var summaries = Load(
            """<member name="M:AslHelp.Foo.Bar"><summary>uses <paramref name="x"/> now</summary></member>""");

        Assert.That(summaries.For("AslHelp.Foo", "Bar"), Is.EqualTo("uses *x* now"));
    }

    [Test]
    public void For_SeeLangword_BecomesBacktickedKeyword()
    {
        var summaries = Load(
            """<member name="M:AslHelp.Foo.Bar"><summary>returns <see langword="null"/></summary></member>""");

        Assert.That(summaries.For("AslHelp.Foo", "Bar"), Is.EqualTo("returns `null`"));
    }

    [Test]
    public void For_SeeCref_BecomesLastSegment()
    {
        var summaries = Load(
            """<member name="M:AslHelp.Foo.Bar"><summary>see <see cref="T:AslHelp.Result`1"/></summary></member>""");

        Assert.That(summaries.For("AslHelp.Foo", "Bar"), Is.EqualTo("see Result`1"));
    }

    [Test]
    public void For_DuplicateKey_FirstSummaryWins()
    {
        var summaries = Load(
            """
            <member name="M:AslHelp.Foo.Bar(System.Int32)"><summary>first</summary></member>
            <member name="M:AslHelp.Foo.Bar(System.String)"><summary>second</summary></member>
            """);

        Assert.That(summaries.For("AslHelp.Foo", "Bar"), Is.EqualTo("first"));
    }

    [Test]
    public void For_UnknownMember_ReturnsNull()
    {
        var summaries = Load("""<member name="M:AslHelp.Foo.Other"><summary>x</summary></member>""");

        Assert.That(summaries.For("AslHelp.Foo", "Missing"), Is.Null);
    }

    [Test]
    public void For_MemberWithoutSummary_IsSkipped()
    {
        var summaries = Load("""<member name="M:AslHelp.Foo.Bar"><remarks>no summary</remarks></member>""");

        Assert.That(summaries.For("AslHelp.Foo", "Bar"), Is.Null);
    }
}
