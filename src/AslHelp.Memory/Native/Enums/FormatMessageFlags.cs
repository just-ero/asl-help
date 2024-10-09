using System;

namespace AslHelp.Memory.Native.Enums;

/// <summary>
///     Provides flags for the <see cref="WinInterop.FormatMessage(FormatMessageFlags, nint, uint, uint, char*, uint, void*)"/> P/Invoke.
/// </summary>
/// <remarks>
///     For further information, see:
///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-formatmessage#parameters">FormatMessage function (winbase.h)</see></i>
/// </remarks>
[Flags]
internal enum FormatMessageFlags : uint
{
    /// <summary>
    ///     The function allocates a buffer large enough to hold the formatted message,
    ///     and places a pointer to the allocated buffer at the address specified by <c>lpBuffer</c>.
    /// </summary>
    AllocateBuffer = 0x0100,

    /// <summary>
    ///     Insert sequences in the message definition such as <c>%1</c> are to be passed through to the output buffer unchanged.
    /// </summary>
    IgnoreInserts = 0x0200,

    /// <summary>
    ///     The <c>lpSource</c> parameter is a pointer to a null-terminated string that contains a message definition.<br/>
    ///     The message definition may contain insert sequences, just as the message text in a message table resource may.
    /// </summary>
    FromString = 0x0400,

    /// <summary>
    ///     The <c>lpSource</c> parameter is a module handle containing the message-table resource(s) to search.<br/>
    ///     If this <c>lpSource</c> handle is <see langword="null"/>, the current process's application image file will be searched.
    /// </summary>
    FromModuleHandle = 0x0800,

    /// <summary>
    ///     The function should search the system message-table resource(s) for the requested message.<br/>
    ///     If this flag is specified with <see cref="FromModuleHandle"/>, the function searches the system message table,
    ///     if the message is not found in the module specified by <c>lpSource</c>.
    /// </summary>
    FromSystem = 0x1000,

    /// <summary>
    ///     The <c>Arguments</c> parameter is a pointer to an array of values that represent the arguments.
    /// </summary>
    ArgumentArray = 0x2000
}
