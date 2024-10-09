using System.Runtime.InteropServices;

using AslHelp.Memory.Native.Enums;

namespace AslHelp.Memory.Native.Structs;

/// <summary>
///     Contains symbol information.
/// </summary>
/// <remarks>
///     For further information, see:
///     <i><see href="https://docs.microsoft.com/windows/win32/api/dbghelp/ns-dbghelp-symbol_infow">SYMBOL_INFOW structure (dbghelp.h)</see></i>
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct SymbolInfo
{
    /// <summary>
    ///     The size of the structure, in bytes.
    /// </summary>
    public required uint SizeOfStruct;

    /// <summary>
    ///     A unique value that identifies the type data that describes the symbol.
    /// </summary>
    public uint TypeIndex;

    /// <summary>
    ///     This member is reserved for system use.
    /// </summary>
    public ulong Reserved0;

    /// <summary>
    ///     This member is reserved for system use.
    /// </summary>
    public ulong Reserved1;

    /// <summary>
    ///     The unique value for the symbol.
    /// </summary>
    public uint Index;

    /// <summary>
    ///     The symbol size, in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    ///     The base address of the module that contains the symbol.
    /// </summary>
    public ulong ModuleBase;

    /// <summary>
    ///     The type of the symbol.
    /// </summary>
    public SymbolFlags Flags;

    /// <summary>
    ///     The value of a constant.
    /// </summary>
    public ulong Value;

    /// <summary>
    ///     The virtual address of the start of the symbol.
    /// </summary>
    public ulong Address;

    /// <summary>
    ///     The register.
    /// </summary>
    public uint Register;

    /// <summary>
    ///     The DIA scope.
    /// </summary>
    public uint Scope;

    /// <summary>
    ///     The PDB classification.
    /// </summary>
    public SymbolTag Tag;

    /// <summary>
    ///     The length of the name, in characters, not including the null-terminating character.
    /// </summary>
    public uint NameLength;

    /// <summary>
    ///     The size of the Name buffer, in characters.
    /// </summary>
    public uint MaxNameLength;

    /// <summary>
    ///     The name of the symbol.
    /// </summary>
    public fixed byte Name[1024];
}
