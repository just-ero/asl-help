using System.Runtime.InteropServices;
using System.Security;

using AslHelp.Memory.Native.Enums;
using AslHelp.Memory.Utils;

namespace AslHelp.Memory.Native;

internal static unsafe partial class WinInterop
{
    /// <summary>
    ///     Formats a message string. The function finds the message definition in a message table resource
    ///     based on a message identifier and a language identifier.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-formatmessagew">
    ///         FormatMessageW function (winbase.h)
    ///     </see></i>
    /// </summary>
    /// <param name="flags">
    ///     The formatting options, and how to interpret the <paramref name="source"/> parameter.
    /// </param>
    /// <param name="source">
    ///     The location of the message definition. The type of this parameter depends upon the settings in the <paramref name="flags"/>
    ///     parameter.
    /// </param>
    /// <param name="messageId">
    ///     The message identifier for the requested message.
    /// </param>
    /// <param name="languageId">
    ///     The language identifier for the requested message.
    /// </param>
    /// <param name="buffer">
    ///     A pointer to a buffer that receives the null-terminated string that specifies the formatted message.<br/>
    ///     If <see cref="FormatMessageFlags.AllocateBuffer"/> is specified, this parameter is a pointer to a pointer to a buffer
    ///     that receives the allocated buffer containing the formatted message.
    /// </param>
    /// <param name="size">
    ///     If the <see cref="FormatMessageFlags.AllocateBuffer"/> flag is not set,
    ///     this parameter specifies the size of the output buffer, in <see cref="char"/>s.<br/>
    ///     If <see cref="FormatMessageFlags.AllocateBuffer"/> is set,
    ///     this parameter specifies the minimum number of <see cref="char"/>s to allocate for an output buffer.
    /// </param>
    /// <param name="arguments">
    ///     An array of values that are used as insert values in the formatted message.
    /// </param>
    /// <returns>
    ///     The number of <see cref="char"/>s stored in the output buffer if the function succeeds;
    ///     otherwise, zero.
    /// </returns>
    public static int FormatMessage(
        FormatMessageFlags flags,
        nint source,
        uint messageId,
        uint languageId,
        char* buffer,
        uint size,
        void* arguments)
    {
        return (int)FormatMessageW((uint)flags, (void*)source, messageId, languageId, (ushort*)buffer, size, arguments);

        [DllImport(Lib.Kernel32, EntryPoint = nameof(FormatMessageW), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern uint FormatMessageW(
            uint dwFlags,
            void* lpSource,
            uint dwMessageId,
            uint dwLanguageId,
            ushort* lpBuffer,
            uint nSize,
#pragma warning disable IDE1006
            void* Arguments);
#pragma warning restore IDE1006
    }
}
