using System.Runtime.InteropServices;
using System.Security;

namespace AslHelp.Memory.Win32;

internal static unsafe partial class PInvoke
{
    /// <summary>
    ///     Closes an open object handle.
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/handleapi/nf-handleapi-closehandle">
    ///         CloseHandle function (handleapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="objectHandle">
    ///     A handle that identifies the caller.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool CloseHandle(nuint objectHandle)
    {
        return CloseHandle((void*)objectHandle) != 0;

        [DllImport(Kernel32, EntryPoint = nameof(CloseHandle), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int CloseHandle(
            void* hObject);
    }
}
