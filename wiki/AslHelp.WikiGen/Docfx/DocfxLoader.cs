using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AslHelp.WikiGen.Docfx;

/// <summary>
///     Loads a docfx metadata directory into an in-memory model keyed by UID.
/// </summary>
internal static class DocfxLoader
{
    public static DocfxModel Load(string metadataDir)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        var items = new Dictionary<string, DocfxItem>(StringComparer.Ordinal);
        var references = new Dictionary<string, DocfxReference>(StringComparer.Ordinal);

        foreach (string path in Directory.EnumerateFiles(metadataDir, "*.yml"))
        {
            if (string.Equals(Path.GetFileName(path), "toc.yml", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            DocfxFile file = deserializer.Deserialize<DocfxFile>(File.ReadAllText(path));
            foreach (DocfxItem item in file.Items)
            {
                items[item.Uid] = item;
            }

            foreach (DocfxReference reference in file.References)
            {
                references.TryAdd(reference.Uid, reference);
            }
        }

        return new DocfxModel(items, references);
    }
}

/// <summary>An indexed docfx model: every item and reference, keyed by UID.</summary>
internal sealed class DocfxModel(
    IReadOnlyDictionary<string, DocfxItem> items,
    IReadOnlyDictionary<string, DocfxReference> references)
{
    public IReadOnlyDictionary<string, DocfxItem> Items => items;
    public IReadOnlyDictionary<string, DocfxReference> References => references;

    public IEnumerable<DocfxItem> OfKind(params string[] kinds)
    {
        return items.Values.Where(i => kinds.Contains(i.Type, StringComparer.Ordinal));
    }

    /// <summary>Returns the display name for a UID, preferring a reference's short name.</summary>
    public string DisplayName(string uid)
    {
        if (references.TryGetValue(uid, out DocfxReference? reference) && reference.Name is { } name)
        {
            return name;
        }

        if (items.TryGetValue(uid, out DocfxItem? item))
        {
            return item.Name;
        }

        return uid;
    }
}
