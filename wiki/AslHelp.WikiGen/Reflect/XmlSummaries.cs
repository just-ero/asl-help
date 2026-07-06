using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AslHelp.WikiGen.Reflect;

/// <summary>
///     Reads member summaries from the (InheritDoc-expanded) XML doc file, keyed by
///     <c>Type.Member</c> (overloads/params/arity dropped — first summary wins).
/// </summary>
internal sealed partial class XmlSummaries
{
    private readonly Dictionary<string, string> _byMember = new(StringComparer.Ordinal);

    [GeneratedRegex("""``?\d+""")] private static partial Regex Arity { get; }
    [GeneratedRegex("""<[^>]+>""")] private static partial Regex Tags { get; }
    [GeneratedRegex("""\s+""")] private static partial Regex Whitespace { get; }

    public XmlSummaries(IEnumerable<string> xmlPaths)
    {
        foreach (var xmlPath in xmlPaths)
        {
            foreach (var member in XDocument.Load(xmlPath).Descendants("member"))
            {
                var name = member.Attribute("name")?.Value;
                var summary = member.Element("summary");
                if (name is null || summary is null || name.Length < 2)
                {
                    continue;
                }

                var key = MemberKey(name);
                if (!_byMember.ContainsKey(key))
                {
                    _byMember[key] = Convert(summary);
                }
            }
        }
    }

    /// <summary>
    ///     Returns the markdown summary for <paramref name="typeFullName"/>.<paramref name="member"/>.
    /// </summary>
    public string? For(string typeFullName, string member)
    {
        return _byMember.GetValueOrDefault($"{typeFullName}.{member}");
    }

    // "M:AslHelp.ResultExtensions.Map``1(AslHelp.Result,``0)" -> "AslHelp.ResultExtensions.Map"
    private static string MemberKey(string xmlName)
    {
        var id = xmlName[2..]; // drop "M:" / "P:" / "T:"
        var paren = id.IndexOf('(', StringComparison.Ordinal);
        if (paren >= 0)
        {
            id = id[..paren];
        }

        return Arity.Replace(id, "");
    }

    // Minimal XML-doc -> markdown: <c>/paramref/typeparamref/langword/see kept as readable text.
    private static string Convert(XElement summary)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var node in summary.Nodes())
        {
            switch (node)
            {
                case XText t:
                    sb.Append(t.Value);
                    break;
                case XElement e:
                    sb.Append(Inline(e));
                    break;
            }
        }

        var text = sb.ToString();
        text = Tags.Replace(text, "");
        return Whitespace.Replace(text, " ").Trim();
    }

    private static string Inline(XElement e)
    {
        return e.Name.LocalName switch
        {
            "c" => $"`{e.Value}`",
            "paramref" or "typeparamref" => $"*{e.Attribute("name")?.Value}*",
            "see" when e.Attribute("langword") is { } lw => $"`{lw.Value}`",
            "see" when e.Attribute("cref") is { } cref => LastSegment(cref.Value),
            _ => e.Value,
        };
    }

    private static string LastSegment(string cref)
    {
        var id = cref.Contains(':') ? cref[(cref.IndexOf(':') + 1)..] : cref;
        var paren = id.IndexOf('(', StringComparison.Ordinal);
        if (paren >= 0)
        {
            id = id[..paren];
        }

        var dot = id.LastIndexOf('.');
        return dot >= 0 ? id[(dot + 1)..] : id;
    }
}
