global using InputField = (string TypeName, string Name, uint Alignment);
global using InputStruct = (string Name, string? Super, (string Type, string Name, uint Alignment)[] Fields);

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using AslHelp.Collections;
using AslHelp.Collections.Errors;
using AslHelp.Memory.Scanning;
using AslHelp.Shared.Results;

namespace AslHelp.Memory.NativeStructs;

internal sealed class CollectedInput : OrderedDictionary<string, InputStruct>
{
    public Dictionary<string, ScanPattern> Patterns { get; } = [];

    public static Result<CollectedInput> GetFromEmbeddedResource(
        string @namespace,
        string runtime,
        string version,
        Assembly containingAssembly)
    {
        string resourceName = $"{@namespace}.{runtime}-{version}.json";
        using Stream? resource = containingAssembly.GetManifestResourceStream(resourceName);
        if (resource is null)
        {
            return NativeStructInitializationError.EmbeddedResourceNotFound(resourceName);
        }

        JsonDefinition definition;
        try
        {
            DataContractJsonSerializer serializer = new(typeof(JsonDefinition));
            definition = (JsonDefinition)serializer.ReadObject(resource);
        }
        catch (Exception ex)
        {
            return ex;
        }

        if (definition.Inheritance is null && definition.Structs is null)
        {
            return NativeStructInitializationError.InheritanceOrStructsMustBeProvided;
        }

        Result<CollectedInput> res = definition.Inheritance is { Runtime.Length: > 0, Version.Length: > 0 } inheritance
            ? GetFromEmbeddedResource(@namespace, inheritance.Runtime, inheritance.Version, containingAssembly)
            : new CollectedInput();

        return res
            .AndThen<CollectedInput>(input =>
            {
                if (definition.Structs is { Length: > 0 } structs)
                {
                    foreach (JsonStruct @struct in structs)
                    {
                        input[@struct.Name] = (@struct.Name, @struct.Super, @struct.Fields.Select(f => (f.Type, f.Name, f.Alignment)).ToArray());
                    }
                }

                if (definition.Patterns is { Length: > 0 } patterns)
                {
                    foreach (JsonScanPattern pattern in patterns)
                    {
                        input.Patterns[pattern.Name] = ScanPattern.Parse(pattern.Offset, pattern.Pattern);
                    }
                }

                return input;
            });
    }

    protected override string GetKeyForItem(InputStruct item)
    {
        return item.Name;
    }

    [DataContract]
    private record JsonDefinition(
        [property: DataMember(Name = "inherits")] Inheritance? Inheritance,
        [property: DataMember(Name = "structs")] JsonStruct[]? Structs,
        [property: DataMember(Name = "signatures")] JsonScanPattern[]? Patterns);

    [DataContract]
    private record Inheritance(
        [property: DataMember(Name = "runtime")] string Runtime,
        [property: DataMember(Name = "version")] string Version);

    [DataContract]
    private record JsonScanPattern(
        [property: DataMember(Name = "name")] string Name,
        [property: DataMember(Name = "offset")] int Offset,
        [property: DataMember(Name = "pattern")] string Pattern);

    [DataContract]
    private record JsonStruct(
        [property: DataMember(Name = "name")] string Name,
        [property: DataMember(Name = "super")] string? Super,
        [property: DataMember(Name = "fields")] JsonField[] Fields);

    [DataContract]
    private record JsonField(
        [property: DataMember(Name = "type")] string Type,
        [property: DataMember(Name = "name")] string Name,
        [property: DataMember(Name = "align")] uint Alignment);
}
