using System.Runtime.InteropServices;
using System.Security;

using AslHelp.Memory.Utils;

namespace AslHelp.Memory.Native;

internal static unsafe partial class WinInterop
{
    /// <summary>
    ///     Closes an open object handle.
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-closehandle">
    ///         CloseHandle function (handleapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="hObject">
    ///     A handle that identifies the caller.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool CloseHandle(nuint objectHandle)
    {
        return CloseHandle((void*)objectHandle) != 0;

        [DllImport(Lib.Kernel32, EntryPoint = nameof(CloseHandle), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int CloseHandle(
            void* hObject);
    }
}
