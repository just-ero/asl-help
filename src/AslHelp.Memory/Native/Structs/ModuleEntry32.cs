using System.Runtime.InteropServices;

namespace AslHelp.Memory.Native.Structs;

/// <summary>
///     Describes an entry from a list of the modules belonging to the specified process.
/// </summary>
/// <remarks>
///     For further information, see:
///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/api/tlhelp32/ns-tlhelp32-moduleentry32w">MODULEENTRY32W structure (tlhelp32.h)</see></i>
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ModuleEntry32
{
    public const int ModuleLength = 256;
    public const int ExePathLength = 260;

    /// <summary>
    ///     The size of the structure, in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    ///     This member is no longer used, and is always set to one.
    /// </summary>
    public uint ModuleId;

    /// <summary>
    ///     The identifier of the process whose modules are to be examined.
    /// </summary>
    public uint ProcessId;

    /// <summary>
    ///     The load count of the module, which is not generally meaningful, and usually equal to 0xFFFF.
    /// </summary>
    public uint GlobalCountUsage;

    /// <summary>
    ///     The load count of the module (same as <see cref="GlobalCountUsage"/>), which is not generally meaningful, and usually equal to 0xFFFF.
    /// </summary>
    public uint ProcessCountUsage;

    /// <summary>
    ///     The base address of the module in the context of the owning process.
    /// </summary>
    public byte* ModuleBaseAddress;

    /// <summary>
    ///     The size of the module, in bytes.
    /// </summary>
    public uint ModuleBaseSize;

    /// <summary>
    ///     A handle to the module in the context of the owning process.
    /// </summary>
    public void* ModuleHandle;

    /// <summary>
    ///     The module name.
    /// </summary>
    public fixed ushort Module[ModuleLength];

    /// <summary>
    ///     The module path.
    /// </summary>
    public fixed ushort ExePath[ExePathLength];
}
