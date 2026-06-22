using System.Runtime.InteropServices;
using System.Security;

namespace AslHelp.Memory.Win32;

internal static unsafe partial class PInvoke
{
    /// <summary>
    ///     Waits until the specified object is in the signaled state or the time-out interval elapses.<br/>
    ///     For further information see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/synchapi/nf-synchapi-waitforsingleobject">
    ///         WaitForSingleObject function (synchapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="handle">
    ///     A handle to the object.
    /// </param>
    /// <param name="milliseconds">
    ///     The time-out interval, in milliseconds.
    /// </param>
    /// <returns>
    ///     The event that caused the function to return.
    /// </returns>
    public static uint WaitForSingleObject(SafeHandle handle, uint milliseconds)
    {
        return WaitForSingleObject(handle, milliseconds);

        [DllImport(Kernel32, EntryPoint = nameof(WaitForSingleObject), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern uint WaitForSingleObject(
            SafeHandle hHandle,
            uint dwMilliseconds);
    }
}
