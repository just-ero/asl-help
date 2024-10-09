using System.Runtime.InteropServices;
using System.Security;

using AslHelp.Memory.Utils;

namespace AslHelp.Memory.Native;

internal static unsafe partial class WinInterop
{
    /// <summary>
    ///     Determines whether the specified process is running under WOW64 or an Intel64 of x64 processor.<br/>
    ///     For further information, see:
    ///     <i><see href="https://docs.microsoft.com/windows/win32/api/wow64apiset/nf-wow64apiset-iswow64process">
    ///         IsWow64Process function (wow64apiset.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process.
    /// </param>
    /// <param name="isWow64">
    ///     Specifies whether the process is running under WOW64 on an Intel64 or x64 processor.
    ///     <para>
    ///     If the process is running under 32-bit Windows, the value is set to <see langword="false"/>.<br/>
    ///     If the process is a 32-bit application running under 64-bit Windows 10 on ARM, the value is set to <see langword="false"/>.<br/>
    ///     If the process is a 64-bit application running under 64-bit Windows, the value is also set to <see langword="false"/>.
    ///     </para>
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsWow64Process(nint processHandle, out bool isWow64)
    {
        int bWow64Process;
        bool success = IsWow64Process((void*)processHandle, &bWow64Process) != 0;

        isWow64 = bWow64Process != 0;
        return success;

        [DllImport(Lib.Kernel32, EntryPoint = nameof(IsWow64Process), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int IsWow64Process(
            void* hProcess,
#pragma warning disable IDE1006
            int* Wow64Process);
#pragma warning restore IDE1006
    }
}
