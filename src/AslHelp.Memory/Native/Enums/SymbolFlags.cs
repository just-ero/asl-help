using System;

namespace AslHelp.Memory.Native.Enums;

/// <summary>
///     Specifies the type of the <see cref="Structs.SymbolInfo"/>.
/// </summary>
/// <remarks>
///     For further information, see:
///     <i><see href="https://docs.microsoft.com/windows/win32/api/dbghelp/ns-dbghelp-symbol_infow#members">SYMBOL_INFOW structure (dbghelp.h)</see></i>
/// </remarks>
[Flags]
internal enum SymbolFlags : uint
{
    /// <summary>
    ///     The <see cref="Structs.SymbolInfo.Value"/> member is used.
    /// </summary>
    ValuePresent = 0x00001,

    /// <summary>
    ///     The symbol is a register. The <see cref="Structs.SymbolInfo.Register"/> member is used.
    /// </summary>
    Register = 0x00008,

    /// <summary>
    ///     Offsets are register relative.
    /// </summary>
    RegisterRelative = 0x00010,

    /// <summary>
    ///     Offsets are frame relative.
    /// </summary>
    FrameRelative = 0x00020,

    /// <summary>
    ///     The symbol is a parameter.
    /// </summary>
    Parameter = 0x00040,

    /// <summary>
    ///     The symbol is a local variable.
    /// </summary>
    Local = 0x00080,

    /// <summary>
    ///     The symbol is a constant.
    /// </summary>
    Constant = 0x00100,

    /// <summary>
    ///     The symbol is from the export table.
    /// </summary>
    Export = 0x00200,

    /// <summary>
    ///     The symbol is a forwarder.
    /// </summary>
    Forwarder = 0x00400,

    /// <summary>
    ///     The symbol is a known function.
    /// </summary>
    Function = 0x00800,

    /// <summary>
    ///     The symbol is a virtual symbol created by the SymAddSymbol function.
    /// </summary>
    Virtual = 0x01000,

    /// <summary>
    ///     The symbol is a thunk.
    /// </summary>
    Thunk = 0x02000,

    /// <summary>
    ///     The symbol is an offset into the TLS data area.
    /// </summary>
    TlsRelativeOffset = 0x04000,

    /// <summary>
    ///     The symbol is a managed code slot.
    /// </summary>
    Slot = 0x08000,

    /// <summary>
    ///     The symbol address is an offset relative to the beginning of the intermediate language block.
    /// </summary>
    IlRelativeOffset = 0x10000,

    /// <summary>
    ///     The symbol is managed metadata.
    /// </summary>
    Metadata = 0x20000,

    /// <summary>
    ///     The symbol is a CLR token.
    /// </summary>
    ClrToken = 0x40000
}
