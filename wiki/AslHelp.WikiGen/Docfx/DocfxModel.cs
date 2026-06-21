using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace AslHelp.WikiGen.Docfx;

// Minimal mapping of the docfx ManagedReference YAML schema — only the fields the renderer needs.
// The deserializer is configured to ignore everything else.

internal sealed class DocfxFile
{
    public List<DocfxItem> Items { get; set; } = [];
    public List<DocfxReference> References { get; set; } = [];
}

internal sealed class DocfxItem
{
    public string Uid { get; set; } = "";
    public string Id { get; set; } = "";
    public string? Parent { get; set; }
    public List<string> Children { get; set; } = [];
    public string Name { get; set; } = "";
    public string NameWithType { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Type { get; set; } = "";          // Namespace, Class, Struct, Property, Method, ...
    public string? Namespace { get; set; }
    public List<string> Assemblies { get; set; } = [];
    public string? Summary { get; set; }
    public string? Remarks { get; set; }
    public DocfxSyntax? Syntax { get; set; }
    public List<string> Inheritance { get; set; } = [];
    public List<string> Implements { get; set; } = [];
    public DocfxSource? Source { get; set; }
}

internal sealed class DocfxSyntax
{
    public string? Content { get; set; }
    public List<DocfxParameter> Parameters { get; set; } = [];
    public List<DocfxParameter> TypeParameters { get; set; } = [];
    public DocfxReturn? Return { get; set; }
}

internal sealed class DocfxParameter
{
    public string Id { get; set; } = "";
    public string? Type { get; set; }
    public string? Description { get; set; }
}

internal sealed class DocfxReturn
{
    public string? Type { get; set; }
    public string? Description { get; set; }
}

internal sealed class DocfxSource
{
    public DocfxRemote? Remote { get; set; }
    public int StartLine { get; set; }
}

internal sealed class DocfxRemote
{
    public string? Path { get; set; }
    public string? Branch { get; set; }
    public string? Repo { get; set; }
}

internal sealed class DocfxReference
{
    public string Uid { get; set; } = "";
    public string? Name { get; set; }
    public string? NameWithType { get; set; }
    public string? FullName { get; set; }
    [YamlMember(Alias = "isExternal")]
    public bool IsExternal { get; set; }
}
