using System.Runtime.InteropServices;
using System.Security;

namespace AslHelp.Memory.Win32;

internal static unsafe partial class PInvoke
{
    /// <summary>
    ///     Takes a snapshot of the specified processes, as well as the heaps, modules, and threads used by these processes.<br/>
    ///     For further information see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/tlhelp32/nf-tlhelp32-createtoolhelp32snapshot">
    ///         CreateToolhelp32Snapshot function (tlhelp32.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processId">
    ///     The process identifier of the process to be included in the snapshot.
    /// </param>
    /// <param name="flags">
    ///     The portions of the system to be included in the snapshot.
    /// </param>
    /// <returns>
    ///     An open handle to the specified snapshot if the funcion succeeds;
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    public static SafeSnapshotHandle CreateToolhelp32Snapshot(uint processId, ThFlags flags)
    {
        return CreateToolhelp32Snapshot((uint)flags, processId);

        [DllImport(Kernel32, EntryPoint = nameof(CreateToolhelp32Snapshot), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern SafeSnapshotHandle CreateToolhelp32Snapshot(
            uint dwFlags,
            uint th32ProcessID);
    }

    /// <summary>
    ///     Retrieves information about the next module associated with a process or thread.<br/>
    ///     For further information see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/tlhelp32/nf-tlhelp32-module32nextw">
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
    public static bool Module32First(SafeSnapshotHandle snapshotHandle, out ModuleEntry32 me)
    {
        var tMe = new ModuleEntry32 { SizeOfStruct = (uint)sizeof(ModuleEntry32) };

        if (Module32FirstW(snapshotHandle, &tMe) != 0)
        {
            me = tMe;
            return true;
        }

        me = default;
        return false;

        [DllImport(Kernel32, EntryPoint = nameof(Module32FirstW), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int Module32FirstW(
            SafeSnapshotHandle hSnapshot,
            ModuleEntry32* lpme);
    }

    /// <summary>
    ///     Retrieves information about the next module associated with a process or thread.<br/>
    ///     For further information see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/tlhelp32/nf-tlhelp32-module32nextw">
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
    public static bool Module32Next(SafeSnapshotHandle snapshotHandle, ref ModuleEntry32 me)
    {
        fixed (ModuleEntry32* pModuleEntry = &me)
        {
            return Module32NextW(snapshotHandle, pModuleEntry) != 0;
        }

        [DllImport(Kernel32, EntryPoint = nameof(Module32NextW), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int Module32NextW(
            SafeSnapshotHandle hSnapshot,
            ModuleEntry32* lpme);
    }
}
