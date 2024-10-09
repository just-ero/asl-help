using AslHelp.Shared.Results.Errors;

namespace AslHelp.Collections.Errors;

internal sealed record NativeStructInitializationError : ResultError
{
    private NativeStructInitializationError(string message)
        : base(message) { }

    public static NativeStructInitializationError Other(string message)
    {
        return new(message);
    }

    // CollectedInput.GetFromEmbeddedResource
    public static NativeStructInitializationError EmbeddedResourceNotFound(string resourceName)
    {
        return new($"Embedded resource '{resourceName}' could not be found.");
    }

    public static NativeStructInitializationError JsonContentsInvalid
        => new("The provided JSON input was not valid.");

    public static NativeStructInitializationError InheritanceOrStructsMustBeProvided
        => new("At least one of 'inheritance' or 'structs' must be provided.");

    // GetTypeSize
    public static NativeStructInitializationError BitfieldTypeMustBeInteger(string type)
    {
        return new($"Bitfield must be an integer type (was '{type}').");
    }

    public static NativeStructInitializationError BitfieldSizeMustBeUnsignedInteger(string type, string size)
    {
        return new($"'{type}': Bitfield size must be an integer between {uint.MinValue} and {uint.MaxValue} (was '{size}').");
    }

    public static NativeStructInitializationError ArrayLengthMustBeUnsignedInteger(string type, string length)
    {
        return new($"'{type}': Array length must be an integer between {uint.MinValue} and {uint.MaxValue} (was '{length}').");
    }

    public static NativeStructInitializationError GenericDefinitionNotFound(string name)
    {
        return new($"Generic definition '{name}' not found. Make sure it is listed before any of its uses in the definition.");
    }
}
