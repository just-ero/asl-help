using System.Runtime.InteropServices;
using System.Security;

using AslHelp.Memory.Native.Enums;
using AslHelp.Memory.Native.Structs;
using AslHelp.Memory.Utils;

namespace AslHelp.Memory.Native;

internal static unsafe partial class WinInterop
{
    /// <summary>
    ///     Takes a snapshot of the specified processes, as well as the heaps, modules, and threads used by these processes.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/api/tlhelp32/nf-tlhelp32-createtoolhelp32snapshot">
    ///         CreateToolhelp32Snapshot function (tlhelp32.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processID">
    ///     The process identifier of the process to be included in the snapshot.
    /// </param>
    /// <param name="flags">
    ///     The portions of the system to be included in the snapshot.
    /// </param>
    /// <returns>
    ///     An open handle to the specified snapshot if the funcion succeeds;
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    public static nuint CreateToolhelp32Snapshot(uint processID, ThFlags flags)
    {
        return (nuint)CreateToolhelp32Snapshot((uint)flags, processID);

        [DllImport(Lib.Kernel32, EntryPoint = nameof(CreateToolhelp32Snapshot), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern void* CreateToolhelp32Snapshot(
            uint dwFlags,
            uint th32ProcessID);
    }

    /// <summary>
    ///     Retrieves information about the next module associated with a process or thread.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/api/tlhelp32/nf-tlhelp32-module32nextw">
    ///         Module32NextW function (dbghelp.h)
    ///     </see></i>
    /// </summary>
    /// <param name="snapshotHandle">
    ///     A handle to the snapshot returned from a previous call to the <see cref="CreateToolhelp32Snapshot"/> function.
    /// </param>
    /// <param name="me">
    ///     A reference to the <see cref="ModuleEntry32"/> structure that receives information about the module.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Module32First(nuint snapshotHandle, ref ModuleEntry32 me)
    {
        fixed (ModuleEntry32* pModuleEntry = &me)
        {
            return Module32FirstW((void*)snapshotHandle, pModuleEntry) != 0;
        }

        [DllImport(Lib.Kernel32, EntryPoint = nameof(Module32FirstW), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int Module32FirstW(
            void* hSnapshot,
            ModuleEntry32* lpme);
    }

    /// <summary>
    ///     Retrieves information about the next module associated with a process or thread.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/api/tlhelp32/nf-tlhelp32-module32nextw">
    ///         Module32NextW function (dbghelp.h)
    ///     </see></i>
    /// </summary>
    /// <param name="snapshotHandle">
    ///     A handle to the snapshot returned from a previous call to the <see cref="CreateToolhelp32Snapshot"/> function.
    /// </param>
    /// <param name="me">
    ///     A reference to the <see cref="ModuleEntry32"/> structure that receives information about the module.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool Module32Next(nuint snapshotHandle, ref ModuleEntry32 me)
    {
        fixed (ModuleEntry32* pModuleEntry = &me)
        {
            return Module32NextW((void*)snapshotHandle, pModuleEntry) != 0;
        }

        [DllImport(Lib.Kernel32, EntryPoint = nameof(Module32NextW), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int Module32NextW(
            void* hSnapshot,
            ModuleEntry32* lpme);
    }
}
