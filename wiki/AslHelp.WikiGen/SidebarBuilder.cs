using AslHelp.WikiGen.Api;
using AslHelp.WikiGen.Docfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AslHelp.WikiGen;

/// <summary>
///     Builds <c>_Sidebar.md</c> from the API model, grouped assembly → namespace → type, using flat
///     <c>[text](Basename)</c> links (which resolve from every page; wikilinks mis-parse on slashes).
/// </summary>
internal static class SidebarBuilder
{
    /// <summary>
    ///     The hand-authored head of the sidebar, written once. Custom tutorial links can be added
    ///     here (or after the generated region) and survive regeneration.
    /// </summary>
    public static string Scaffold()
    {
        var sb = new StringBuilder();
        sb.AppendLine("### asl-help").AppendLine();
        sb.AppendLine("- [Home](Home)").AppendLine();
        return sb.ToString();
    }

    /// <summary>
    ///     The generated API tree (assembly → namespace → type) embedded in the sidebar.
    /// </summary>
    public static string Build(IReadOnlyList<ApiType> types, SidebarStyle style)
    {
        var sb = new StringBuilder();

        var collapse = style == SidebarStyle.Collapsible;
        if (collapse)
        {
            sb.AppendLine("<details>");
            sb.AppendLine($"<summary><strong>{WikiConventions.HubTitle}</strong></summary>");
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine($"- [{WikiConventions.HubTitle}]({WikiConventions.HubSlug})");
        }

        foreach (var assembly in types.GroupBy(t => t.Assembly).OrderBy(g => g.Key, StringComparer.Ordinal))
        {
            sb.AppendLine($"- **{assembly.Key}**");
            foreach (var ns in assembly.GroupBy(t => t.Namespace).OrderBy(g => g.Key, StringComparer.Ordinal))
            {
                sb.AppendLine($"  - {LinkResolver.Link(ns.Key, ns.Key)}");
                foreach (var type in ns.OrderBy(t => t.Display, StringComparer.Ordinal))
                {
                    sb.AppendLine($"    - {LinkResolver.Link(type.Display, type.Ref)}");
                }
            }
        }

        if (collapse)
        {
            sb.AppendLine().AppendLine("</details>");
        }

        return sb.ToString();
    }
}
