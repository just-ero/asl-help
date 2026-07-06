using AslHelp.WikiGen.Api;
using AslHelp.WikiGen.Docfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AslHelp.WikiGen.Render;

/// <summary>
///     Renders the markdown for each kind of wiki page from the <see cref="ApiType"/> model.
/// </summary>
internal static class Pages
{
    private static readonly MemberGroup[] _groupOrder =
    [
        MemberGroup.Constructors, MemberGroup.Properties, MemberGroup.Fields,
        MemberGroup.Methods, MemberGroup.Operators, MemberGroup.Events,
    ];

    private const string Br = "  "; // trailing spaces => line break inside a blockquote

    private static readonly (string Kind, string Heading)[] _typeKinds =
    [
        ("Class", "Classes"), ("Struct", "Structs"), ("Interface", "Interfaces"),
        ("Enum", "Enums"), ("Delegate", "Delegates"),
    ];

    public static string Type(ApiType type)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Breadcrumb(Md(type.Namespace, type.Namespace), Escape(type.Display)));
        sb.AppendLine();
        sb.AppendLine($"# {Escape(type.Display)} {type.Kind.ToLowerInvariant()}{SourceArrow(type.Source)}").AppendLine();

        var meta = new List<string>
        {
            $"**Assembly:** {type.Assembly}",
            $"**Namespace:** {Md(type.Namespace, type.Namespace)}",
        };
        for (var i = 0; i < meta.Count; i++)
        {
            sb.AppendLine($"> {meta[i]}{(i < meta.Count - 1 ? Br : "")}");
        }

        sb.AppendLine();

        if (!string.IsNullOrEmpty(type.Summary))
        {
            sb.AppendLine(type.Summary).AppendLine();
        }

        sb.AppendLine("```csharp").AppendLine(type.Signature).AppendLine("```").AppendLine();

        if (type.Inheritance.Count > 0)
        {
            sb.AppendLine($"**Inheritance:** {string.Join(" → ", type.Inheritance.Select(LinkResolver.Link).Append(Escape(type.Display)))}").AppendLine();
        }

        if (type.Implements.Count > 0)
        {
            sb.AppendLine($"**Implements:** {string.Join(", ", type.Implements.Select(LinkResolver.Link))}").AppendLine();
        }

        if (!string.IsNullOrEmpty(type.Remarks))
        {
            sb.AppendLine("## Remarks").AppendLine().AppendLine(type.Remarks).AppendLine();
        }

        foreach (var group in _groupOrder)
        {
            var pages = type.Members
                .Where(m => m.Group == group)
                .GroupBy(m => m.Ref)
                .Select(g => g.First())
                .OrderBy(m => m.Name, StringComparer.Ordinal)
                .ToList();
            if (pages.Count == 0)
            {
                continue;
            }

            var valued = group is MemberGroup.Properties or MemberGroup.Fields;
            var enumValues = type.Kind == "Enum" && group is MemberGroup.Fields;
            sb.AppendLine($"## {group}").AppendLine();
            sb.AppendLine(valued ? $"| Name | {(enumValues ? "Value" : "Type")} | Summary |" : "| Name | Summary |");
            sb.AppendLine(valued ? "| --- | --- | --- |" : "| --- | --- |");
            foreach (var m in pages)
            {
                var link = Md(MemberNaming.For(m).Display, m.Ref);
                var valueCell = enumValues
                    ? (m.Value is null ? "" : $"`{m.Value}`")
                    : (m.ValueType is null ? "" : LinkResolver.Link(m.ValueType));
                sb.AppendLine(valued
                    ? $"| {link} | {valueCell} | {Cell(m.Summary)} |"
                    : $"| {link} | {Cell(m.Summary)} |");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static string Member(ApiType type, IReadOnlyList<ApiMember> overloads)
    {
        var display = MemberNaming.For(overloads[0]).Display;
        var sb = new StringBuilder();
        sb.AppendLine(Breadcrumb(Md(type.Namespace, type.Namespace), Md(type.Display, type.Ref), Escape(display)));
        sb.AppendLine();
        var many = overloads.Count > 1;
        sb.AppendLine($"# {Escape(type.Display)}.{Escape(display)}{(many ? "" : SourceArrow(overloads[0].Source))}").AppendLine();
        for (var i = 0; i < overloads.Count; i++)
        {
            var m = overloads[i];
            if (many)
            {
                sb.AppendLine($"## Overload {i + 1}").AppendLine();
            }

            if (!string.IsNullOrEmpty(m.Summary))
            {
                sb.AppendLine(m.Summary).AppendLine();
            }

            sb.AppendLine("```csharp").AppendLine(m.Signature).AppendLine("```").AppendLine();

            if (m.ValueType is { } vt)
            {
                sb.AppendLine($"**Value:** {LinkResolver.Link(vt)}").AppendLine();
            }

            if (m.Parameters.Count > 0)
            {
                sb.AppendLine("| Parameter | Type | Description |").AppendLine("| --- | --- | --- |");
                foreach (var p in m.Parameters)
                {
                    sb.AppendLine($"| `{p.Name}` | {LinkResolver.Link(p.Type)} | {Cell(p.Summary)} |");
                }

                sb.AppendLine();
            }

            if (m.ReturnType is { } rt)
            {
                sb.AppendLine($"**Returns:** {LinkResolver.Link(rt)}{(string.IsNullOrEmpty(m.ReturnSummary) ? "" : " — " + m.ReturnSummary)}").AppendLine();
            }

            if (many && m.Source is { } src)
            {
                sb.AppendLine($"**Source:** [{src.FileName}]({src.Url}){SourceArrow(src)}").AppendLine();
            }
        }

        return sb.ToString();
    }

    // "Lines of text" + a text-presentation arrow (U+FE0E keeps it a small glyph, not a giant emoji).
    private const string SourceGlyph = "≣↗︎";

    private static string SourceArrow(ApiSource? source)
    {
        return source is null
            ? ""
            : $" <sup><a href=\"{source.Url}\" title=\"Go to source: {source.FileName}\">{SourceGlyph}</a></sup>";
    }

    public static string Namespace(string ns, IReadOnlyList<ApiType> typesInNamespace)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Breadcrumb(Escape(ns)));
        sb.AppendLine();
        sb.AppendLine($"# {ns} namespace").AppendLine();

        foreach (var (kind, heading) in _typeKinds)
        {
            List<ApiType> kinds = [.. typesInNamespace.Where(t => t.Kind == kind).OrderBy(t => t.Display, StringComparer.Ordinal)];
            if (kinds.Count == 0)
            {
                continue;
            }

            sb.AppendLine($"## {heading}").AppendLine();
            sb.AppendLine("| Name | Summary |").AppendLine("| --- | --- |");
            foreach (var t in kinds)
            {
                sb.AppendLine($"| {Md(t.Display, t.Ref)} | {Cell(t.Summary)} |");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static string Hub(IReadOnlyList<ApiType> types)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Breadcrumb(WikiConventions.HubTitle));
        sb.AppendLine();
        sb.AppendLine("# API Reference").AppendLine();
        AppendAssemblyTree(sb, types);
        return sb.ToString();
    }

    /// <summary>
    ///     The hand-authored intro of the Home page, written once. Edits here are preserved on
    ///     subsequent runs; only <see cref="HomeApi"/> below it is regenerated.
    /// </summary>
    public static string HomeScaffold(string assemblyName)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# {assemblyName}").AppendLine();
        sb.AppendLine($"Welcome to the {assemblyName} wiki.").AppendLine();
        return sb.ToString();
    }

    /// <summary>
    ///     The generated API navigation block embedded in the Home page.
    /// </summary>
    public static string HomeApi(IReadOnlyList<ApiType> types)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"## [{WikiConventions.HubTitle}]({WikiConventions.HubSlug})").AppendLine();
        AppendAssemblyTree(sb, types);
        return sb.ToString();
    }

    private static void AppendAssemblyTree(StringBuilder sb, IReadOnlyList<ApiType> types)
    {
        foreach (var assembly in types.GroupBy(t => t.Assembly).OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            sb.AppendLine($"### {assembly.Key}").AppendLine();
            foreach (var ns in assembly.Select(t => t.Namespace).Distinct().OrderBy(n => n, StringComparer.Ordinal))
            {
                sb.AppendLine($"- {Md(ns, ns)}");
            }

            sb.AppendLine();
        }
    }

    private static string Breadcrumb(params string[] tail)
    {
        var parts = new List<string> { $"[Home](Home)", $"[{WikiConventions.HubTitle}]({WikiConventions.HubSlug})" };
        parts.AddRange(tail);
        return string.Join(WikiConventions.Separator, parts);
    }

    private static string Md(string display, string target)
    {
        return $"[{Escape(display)}]({target})";
    }

    private static string Cell(string? text)
    {
        return (text ?? "").Replace("|", "\\|");
    }

    private static string Escape(string s)
    {
        return s.Replace("<", "&lt;").Replace(">", "&gt;");
    }
}
