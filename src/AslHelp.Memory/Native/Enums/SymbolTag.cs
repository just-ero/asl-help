namespace AslHelp.Memory.Native.Enums;

/// <summary>
///     Specifies the type of the <see cref="Structs.SymbolInfo"/>.
/// </summary>
/// <remarks>
///     For further information, see:
///     <i><see href="https://docs.microsoft.com/previous-versions/visualstudio/visual-studio-2010/bkedss5f(v=vs.100)">SymTagEnum</see></i>
/// </remarks>
internal enum SymbolTag : uint
{
    /// <summary>
    ///     Indicates that the symbol has no type.
    /// </summary>
    Null,

    /// <summary>
    ///     Indicates that the symbol is an .exe file.
    /// </summary>
    Exe,

    /// <summary>
    ///     Indicates the compiland symbol for each compiland component of the symbol store.
    /// </summary>
    Compiland,

    /// <summary>
    ///     Indicates that the symbol contains extended attributes of the compiland.
    /// </summary>
    CompilandDetails,

    /// <summary>
    ///     Indicates that the symbol is an environment string defined for the compiland.
    /// </summary>
    CompilandEnv,

    /// <summary>
    ///     Indicates that the symbol is a function.
    /// </summary>
    Function,

    /// <summary>
    ///     Indicates that the symbol is a nested block.
    /// </summary>
    Block,

    /// <summary>
    ///     Indicates that the symbol is data.
    /// </summary>
    Data,

    /// <summary>
    ///     Indicates that the symbol is for a code annotation.
    /// </summary>
    Annotation,

    /// <summary>
    ///     Indicates that the symbol is a label.
    /// </summary>
    Label,

    /// <summary>
    ///     Indicates that the symbol is a public symbol.
    /// </summary>
    PublicSymbol,

    /// <summary>
    ///     Indicates that the symbol is a user-defined type (structure, class, or union).
    /// </summary>
    UDT,

    /// <summary>
    ///     Indicates that the symbol is an enumeration.
    /// </summary>
    Enum,

    /// <summary>
    ///     Indicates that the symbol is a function signature type.
    /// </summary>
    FunctionType,

    /// <summary>
    ///     Indicates that the symbol is a pointer type.
    /// </summary>
    PointerType,

    /// <summary>
    ///     Indicates that the symbol is an array type.
    /// </summary>
    ArrayType,

    /// <summary>
    ///     Indicates that the symbol is a base type.
    /// </summary>
    BaseType,

    /// <summary>
    ///     Indicates that the symbol is a typedef, that is, an alias for another type.
    /// </summary>
    Typedef,

    /// <summary>
    ///     Indicates that the symbol is a base class of a user-defined type.
    /// </summary>
    BaseClass,

    /// <summary>
    ///     Indicates that the symbol is a friend of a user-defined type.
    /// </summary>
    Friend,

    /// <summary>
    ///     Indicates that the symbol is a function argument.
    /// </summary>
    FunctionArgType,

    /// <summary>
    ///     Indicates that the symbol is the end location of the function's prologue code.
    /// </summary>
    FuncDebugStart,

    /// <summary>
    ///     Indicates that the symbol is the beginning location of the function's epilogue code.
    /// </summary>
    FuncDebugEnd,

    /// <summary>
    ///     Indicates that the symbol is a namespace name, active in the current scope.
    /// </summary>
    UsingNamespace,

    /// <summary>
    ///     Indicates that the symbol is a virtual table description.
    /// </summary>
    VTableShape,

    /// <summary>
    ///     Indicates that the symbol is a virtual table pointer.
    /// </summary>
    VTable,

    /// <summary>
    ///     Indicates that the symbol is a custom symbol and is not interpreted by DIA.
    /// </summary>
    Custom,

    /// <summary>
    ///     Indicates that the symbol is a thunk used for sharing data between 16 and 32 bit code.
    /// </summary>
    Thunk,

    /// <summary>
    ///     Indicates that the symbol is a custom compiler symbol.
    /// </summary>
    CustomType,

    /// <summary>
    ///     Indicates that the symbol is in metadata.
    /// </summary>
    ManagedType,

    /// <summary>
    ///     Indicates that the symbol is a FORTRAN multi-dimensional array.
    /// </summary>
    Dimension
}
