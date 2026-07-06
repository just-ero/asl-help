using System;
using System.Text;

namespace AslHelp.WikiGen.Api;

/// <summary>
///     Derives a member's page file name and its display label, merging overloads onto one page.
/// </summary>
internal static class MemberNaming
{
    public static (string File, string Display) For(ApiMember member)
    {
        return For(member.Name, member.Group);
    }

    public static (string File, string Display) For(string name, MemberGroup group)
    {
        if (group == MemberGroup.Operators)
        {
            return name.Contains("explicit", StringComparison.Ordinal)
                ? ("op_Explicit", "explicit operator")
                : ("op_Implicit", "implicit operator");
        }

        var baseName = name;
        var cut = baseName.IndexOfAny(['<', '(']);
        if (cut >= 0)
        {
            baseName = baseName[..cut];
        }

        return (Sanitize(baseName), baseName);
    }

    private static string Sanitize(string name)
    {
        var sb = new StringBuilder(name.Length);
        foreach (var c in name)
        {
            sb.Append(char.IsLetterOrDigit(c) || c is '_' or '-' ? c : '_');
        }

        return sb.ToString();
    }
}
