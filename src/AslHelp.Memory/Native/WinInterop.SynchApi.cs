using System.Runtime.InteropServices;
using System.Security;

using AslHelp.Memory.Utils;

namespace AslHelp.Memory.Native;

internal static unsafe partial class WinInterop
{
    /// <summary>
    ///     Waits until the specified object is in the signaled state or the time-out interval elapses.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-waitforsingleobject">
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
    public static uint WaitForSingleObject(nint handle, uint milliseconds)
    {
        return WaitForSingleObject((void*)handle, milliseconds);

        [DllImport(Lib.Kernel32, EntryPoint = nameof(WaitForSingleObject), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern uint WaitForSingleObject(
            void* hHandle,
            uint dwMilliseconds);
    }
}
