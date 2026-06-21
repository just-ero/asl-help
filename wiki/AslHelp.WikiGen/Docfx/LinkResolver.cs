using AslHelp.WikiGen.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AslHelp.WikiGen.Docfx;

/// <summary>
///     Resolves docfx UIDs to wiki page paths/links and converts docfx HTML summaries to markdown.
/// </summary>
internal sealed partial class LinkResolver(DocfxModel model, IReadOnlyDictionary<string, string> uidToPath)
{
    [GeneratedRegex("""<xref href="([^"]+)"[^>]*>(?:</xref>)?""")] private static partial Regex Xref { get; }
    [GeneratedRegex("""<code class="[^"]*ref">([^<]+)</code>""")] private static partial Regex RefCode { get; }
    [GeneratedRegex("""<code>([^<]+)</code>""")] private static partial Regex InlineCode { get; }
    [GeneratedRegex("""<a href="([^"]+)">([^<]+)</a>""")] private static partial Regex Anchor { get; }
    [GeneratedRegex("""</?p>""")] private static partial Regex Paragraph { get; }
    [GeneratedRegex("""\s+""")] private static partial Regex Whitespace { get; }
    [GeneratedRegex("""``\d+""")] private static partial Regex MethodArity { get; }

    /// <summary>
    ///     Formats a flat-basename markdown link, or plain text when there is no target.
    /// </summary>
    public static string Link(ApiLink link)
    {
        return link.Ref is null ? Escape(link.Display) : $"[{Escape(link.Display)}]({link.Ref})";
    }

    public static string Link(string display, string target)
    {
        return $"[{Escape(display)}]({target})";
    }

    /// <summary>
    ///     Builds a link to a type (open or constructed generic) from its UID.
    /// </summary>
    public ApiLink TypeLink(string uid)
    {
        return new ApiLink(model.DisplayName(uid), ResolveToPath(uid));
    }

    /// <summary>
    ///     Resolves any UID (type or member, open or constructed) to its owning type's page path.
    /// </summary>
    public string? ResolveToPath(string uid)
    {
        string u = OpenForm(Uri.UnescapeDataString(uid));
        int paren = u.IndexOf('(', StringComparison.Ordinal);
        if (paren >= 0)
        {
            u = u[..paren];
        }

        u = MethodArity.Replace(u, "");

        while (true)
        {
            if (uidToPath.TryGetValue(u, out string? path))
            {
                return path;
            }

            int dot = u.LastIndexOf('.');
            if (dot < 0)
            {
                return null;
            }

            u = u[..dot];
        }
    }

    /// <summary>
    ///     Converts a constructed-generic UID (<c>Foo{T}</c>) to its open form (<c>Foo`1</c>).
    /// </summary>
    public static string OpenForm(string uid)
    {
        if (!uid.Contains('{', StringComparison.Ordinal))
        {
            return uid;
        }

        var sb = new StringBuilder();
        int i = 0;
        while (i < uid.Length)
        {
            if (uid[i] == '{')
            {
                int depth = 0, args = 1, j = i;
                for (; j < uid.Length; j++)
                {
                    if (uid[j] == '{')
                    {
                        depth++;
                    }
                    else if (uid[j] == '}')
                    {
                        depth--;
                        if (depth == 0)
                        {
                            break;
                        }
                    }
                    else if (uid[j] == ',' && depth == 1)
                    {
                        args++;
                    }
                }

                sb.Append('`').Append(args);
                i = j + 1;
            }
            else
            {
                sb.Append(uid[i++]);
            }
        }

        return sb.ToString();
    }

    /// <summary>Converts a docfx HTML summary/remarks fragment into wiki markdown.</summary>
    public string ToMarkdown(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return "";
        }

        string text = Xref.Replace(html, m =>
        {
            string uid = m.Groups[1].Value;
            return Link(new ApiLink(model.DisplayName(Uri.UnescapeDataString(uid)), ResolveToPath(uid)));
        });

        text = Anchor.Replace(text, m => $"[{m.Groups[2].Value}]({m.Groups[1].Value})");
        text = RefCode.Replace(text, m => $"*{m.Groups[1].Value}*");
        text = InlineCode.Replace(text, m => $"`{m.Groups[1].Value}`");
        text = Paragraph.Replace(text, " ");
        text = Whitespace.Replace(text, " ").Trim();
        return text;
    }

    private static string Escape(string display)
    {
        return display.Replace("<", "&lt;").Replace(">", "&gt;");
    }
}
