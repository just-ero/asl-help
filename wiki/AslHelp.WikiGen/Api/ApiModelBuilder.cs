using AslHelp.WikiGen.Docfx;
using AslHelp.WikiGen.Reflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AslHelp.WikiGen.Api;

/// <summary>
///     Builds the unified <see cref="ApiType"/> model: structure/signatures/docs from docfx, plus
///     the <c>extension</c> members reflected out of the assembly (which docfx drops).
/// </summary>
internal static partial class ApiModelBuilder
{
    private static readonly string[] _typeKinds = ["Class", "Struct", "Interface", "Enum", "Delegate"];

    [GeneratedRegex("""`(\d+)""")] private static partial Regex Arity { get; }

    [GeneratedRegex("""``\d+""")] private static partial Regex MethodArity { get; }

    private static string _repoDir = ".";
    private static readonly Dictionary<string, string[]> _fileLines = new(StringComparer.OrdinalIgnoreCase);

    public static List<ApiType> Build(DocfxModel model, IReadOnlyList<string> assemblyPaths, XmlSummaries summaries, string repoDir)
    {
        _repoDir = repoDir;
        _fileLines.Clear();
        List<DocfxItem> typeItems = [.. model.OfKind(_typeKinds)];

        // uid -> unique link basename: types (local name, arity -N) and their members (Type.member).
        var uidToRef = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (DocfxItem t in typeItems)
        {
            string typeRef = TypeRef(t.Uid, t.Namespace ?? "");
            uidToRef[t.Uid] = typeRef;
            foreach (string childUid in t.Children)
            {
                if (model.Items.TryGetValue(childUid, out DocfxItem? child))
                {
                    string token = MemberNaming.For(child.Name, GroupOf(child.Type)).File;
                    uidToRef[StripMemberUid(childUid)] = $"{typeRef}.{token}";
                }
            }
        }

        var resolver = new LinkResolver(model, uidToRef);
        Dictionary<string, List<ApiMember>> extensions = ExtensionCollector.Collect(assemblyPaths, summaries);

        var types = new List<ApiType>();
        foreach (DocfxItem item in typeItems)
        {
            string ns = item.Namespace ?? "";
            string assembly = item.Assemblies.FirstOrDefault() ?? "";
            string typeRef = uidToRef[item.Uid];
            string typeFile = $"{assembly}/{ns}/{typeRef}";

            var members = item.Children
                .Select(uid => model.Items.GetValueOrDefault(uid))
                .Where(m => m is not null)
                .Select(m => ToMember(m!, resolver))
                .ToList();

            if (extensions.TryGetValue(item.Uid, out List<ApiMember>? extra))
            {
                members.AddRange(extra);
            }

            members = [.. members.Select(m =>
            {
                string memberRef = $"{typeRef}.{MemberNaming.For(m).File}";
                return m with { Ref = memberRef, File = $"{typeFile}/{memberRef}" };
            })];

            types.Add(new ApiType(
                Uid: item.Uid,
                Ref: typeRef,
                File: typeFile,
                Display: model.DisplayName(item.Uid),
                Kind: item.Type,
                Signature: item.Syntax?.Content ?? "",
                Namespace: ns,
                Assembly: assembly,
                Summary: resolver.ToMarkdown(item.Summary),
                Remarks: resolver.ToMarkdown(item.Remarks),
                Inheritance: [.. item.Inheritance.Select(resolver.TypeLink)],
                Implements: [.. item.Implements.Select(resolver.TypeLink)],
                Source: ToSource(item.Source),
                Members: members));
        }

        return types;
    }

    private static ApiMember ToMember(DocfxItem item, LinkResolver resolver)
    {
        bool valued = item.Type is "Property" or "Field";
        return new ApiMember(
            Name: item.Name,
            Group: GroupOf(item.Type),
            Signature: item.Syntax?.Content ?? "",
            Summary: resolver.ToMarkdown(item.Summary),
            ValueType: valued && item.Syntax?.Return?.Type is { } vt ? resolver.TypeLink(vt) : null,
            Parameters: [.. (item.Syntax?.Parameters ?? [])
                .Select(p => new ApiParameter(p.Id, resolver.TypeLink(p.Type ?? ""), resolver.ToMarkdown(p.Description)))],
            ReturnType: !valued && item.Syntax?.Return?.Type is { } rt ? resolver.TypeLink(rt) : null,
            ReturnSummary: resolver.ToMarkdown(item.Syntax?.Return?.Description),
            Source: ToSource(item.Source));
    }

    private static MemberGroup GroupOf(string kind)
    {
        return kind switch
        {
            "Constructor" => MemberGroup.Constructors,
            "Property" => MemberGroup.Properties,
            "Field" => MemberGroup.Fields,
            "Operator" => MemberGroup.Operators,
            "Event" => MemberGroup.Events,
            _ => MemberGroup.Methods,
        };
    }

    // Normalizes a member UID to the key form the resolver looks up (no parameters / method arity).
    private static string StripMemberUid(string uid)
    {
        int paren = uid.IndexOf('(', StringComparison.Ordinal);
        return MethodArity.Replace(paren >= 0 ? uid[..paren] : uid, "");
    }

    /// <summary>
    ///     The type's unique link basename: its local name with generic arity as <c>-N</c>.
    /// </summary>
    public static string TypeRef(string uid, string ns)
    {
        string local = ns.Length > 0 && uid.StartsWith(ns + ".", StringComparison.Ordinal)
            ? uid[(ns.Length + 1)..]
            : uid;
        return Arity.Replace(local, m => "-" + m.Groups[1].Value);
    }

    private static ApiSource? ToSource(DocfxSource? source)
    {
        if (source?.Remote is not { Repo: { } repo, Path: { } path })
        {
            return null;
        }

        string baseRepo = repo.TrimEnd('/');
        if (baseRepo.EndsWith(".git", StringComparison.Ordinal))
        {
            baseRepo = baseRepo[..^4];
        }

        string branch = source.Remote.Branch ?? "main";
        string encoded = path.Replace("{", "%7B").Replace("}", "%7D").Replace(" ", "%20");
        int line = DeclarationLine(path, source.StartLine);
        string anchor = line > 0 ? $"#L{line}" : "";
        return new ApiSource(System.IO.Path.GetFileName(path), $"{baseRepo}/blob/{branch}/{encoded}{anchor}");
    }

    // docfx's startLine (0-based) is the start of the syntax span, which includes attribute lists.
    // Scan forward from there past attribute/blank lines to the real declaration; return its 1-based line.
    private static int DeclarationLine(string repoRelativePath, int startLine)
    {
        if (startLine < 0)
        {
            return 0;
        }

        string full = System.IO.Path.Combine(_repoDir, repoRelativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));
        if (!_fileLines.TryGetValue(full, out string[]? lines))
        {
            lines = System.IO.File.Exists(full) ? System.IO.File.ReadAllLines(full) : [];
            _fileLines[full] = lines;
        }

        if (lines.Length == 0)
        {
            return startLine + 1; // fall back to docfx line (0-based -> 1-based)
        }

        int i = startLine;
        while (i < lines.Length)
        {
            string trimmed = lines[i].TrimStart();
            if (trimmed.Length == 0 || trimmed.StartsWith('['))
            {
                i++;
                continue;
            }

            break;
        }

        return i + 1;
    }
}
