using System.Reflection;

using AslHelp.Collections;
using AslHelp.Shared.Results;

namespace AslHelp.Memory.NativeStructs;

public sealed class NativeStructCollection : OrderedDictionary<string, NativeStruct>
{
    internal NativeStructCollection() { }

    public static Result<NativeStructCollection> Parse((string, string, string) resource, bool is64Bit = true)
    {
        return Parse(resource, is64Bit, Assembly.GetCallingAssembly());
    }

    public static Result<NativeStructCollection> Parse((string, string, string) resource, bool is64Bit, Assembly assembly)
    {
        (string @namespace, string @runtime, string version) = resource;

        return CollectedInput.GetFromEmbeddedResource(@namespace, @runtime, version, assembly)
            .AndThen(input => NativeStructCollectionInitializer.Generate(input, is64Bit));
    }

    protected override string GetKeyForItem(NativeStruct item)
    {
        return item.Name;
    }
}
