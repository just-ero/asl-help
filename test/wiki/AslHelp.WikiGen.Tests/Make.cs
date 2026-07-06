using System.Collections.Generic;

using AslHelp.WikiGen.Api;
using AslHelp.WikiGen.Docfx;

namespace AslHelp.WikiGen.Tests;

/// <summary>
///     Factory helpers for the internal API/docfx model records used across the WikiGen tests.
/// </summary>
internal static class Make
{
    public static ApiLink Link(string display, string? @ref = null)
    {
        return new(display, @ref);
    }

    public static ApiSource Src(string fileName, string url)
    {
        return new(fileName, url);
    }

    public static ApiMember Method(
        string name,
        string signature = "",
        string? summary = null,
        IReadOnlyList<ApiParameter>? parameters = null,
        ApiLink? returnType = null,
        string? returnSummary = null,
        ApiSource? source = null,
        string @ref = "")
    {
        return new(name, MemberGroup.Methods, signature, summary, null, parameters ?? [], returnType, returnSummary, source)
        {
            Ref = @ref,
        };
    }

    public static ApiMember Member(
        string name,
        MemberGroup group,
        string @ref = "",
        string? summary = null,
        ApiLink? valueType = null)
    {
        return new(name, group, $"sig {name}", summary, valueType, [], null, null, null)
        {
            Ref = @ref,
        };
    }

    public static ApiMember Property(string name, ApiLink? valueType = null, string? summary = null, string @ref = "")
    {
        return new(name, MemberGroup.Properties, $"public T {name}", summary, valueType, [], null, null, null)
        {
            Ref = @ref,
        };
    }

    public static ApiMember EnumField(string name, string value, string? summary = null, string @ref = "")
    {
        return new(name, MemberGroup.Fields, $"{name} = {value}", summary, null, [], null, null, null)
        {
            Ref = @ref,
            Value = value,
        };
    }

    public static ApiType Type(
        string display,
        string kind = "Class",
        string? @ref = null,
        string ns = "AslHelp",
        string assembly = "AslHelp",
        string? remarks = null,
        IReadOnlyList<ApiLink>? inheritance = null,
        IReadOnlyList<ApiLink>? implements = null,
        ApiSource? source = null,
        IReadOnlyList<ApiMember>? members = null)
    {
        return new(
            Uid: "",
            Ref: @ref ?? display,
            File: "",
            Display: display,
            Kind: kind,
            Signature: $"public {kind} {display}",
            Namespace: ns,
            Assembly: assembly,
            Summary: null,
            Remarks: remarks,
            Inheritance: inheritance ?? [],
            Implements: implements ?? [],
            Source: source,
            Members: members ?? []);
    }

    public static DocfxItem Item(
        string uid,
        string type = "Class",
        string name = "",
        string? ns = null,
        IEnumerable<string>? assemblies = null,
        IEnumerable<string>? children = null,
        string? syntaxContent = null,
        DocfxSource? source = null)
    {
        return new()
        {
            Uid = uid,
            Type = type,
            Name = name,
            Namespace = ns,
            Assemblies = [.. assemblies ?? []],
            Children = [.. children ?? []],
            Syntax = syntaxContent is null ? null : new DocfxSyntax { Content = syntaxContent },
            Source = source,
        };
    }

    public static DocfxReference Reference(string uid, string? name = null)
    {
        return new()
        {
            Uid = uid,
            Name = name,
        };
    }

    public static DocfxSource Source(string repo, string branch, string path, int startLine = 0)
    {
        return new()
        {
            Remote = new DocfxRemote { Repo = repo, Branch = branch, Path = path },
            StartLine = startLine,
        };
    }

    public static DocfxModel Model(IEnumerable<DocfxItem>? items = null, IEnumerable<DocfxReference>? references = null)
    {
        Dictionary<string, DocfxItem> itemMap = [];
        foreach (var item in items ?? [])
        {
            itemMap[item.Uid] = item;
        }

        Dictionary<string, DocfxReference> refMap = [];
        foreach (var reference in references ?? [])
        {
            refMap[reference.Uid] = reference;
        }

        return new(itemMap, refMap);
    }

    public static LinkResolver Resolver(IReadOnlyDictionary<string, string> uidToPath, DocfxModel? model = null)
    {
        return new(model ?? Model(), uidToPath);
    }
}
