using System;

namespace AslHelp.Memory.Win32;

/// <summary>
///     Provides flags for the <see cref="PInvoke.EnumProcessModules"/> P/Invoke.
/// </summary>
/// <remarks>
///     For further information see:
///     <i><see href="https://learn.microsoft.com/windows/win32/api/psapi/nf-psapi-enumprocessmodulesex#parameters">
///         EnumProcessModulesEx function (psapi.h)
///     </see></i>
/// </remarks>
[Flags]
internal enum ListModulesFilter : uint
{
    /// <summary>
    ///     Use the default behavior.
    /// </summary>
    Default,

    /// <summary>
    ///     List the 32-bit modules.
    /// </summary>
    List32Bit = 1 << 0,

    /// <summary>
    ///     List the 64-bit modules.
    /// </summary>
    List64Bit = 1 << 1,

    /// <summary>
    ///     List all modules.
    /// </summary>
    ListAll = List32Bit | List64Bit,
}
