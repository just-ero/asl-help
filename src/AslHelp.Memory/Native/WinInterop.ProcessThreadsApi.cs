using System.Runtime.InteropServices;
using System.Security;

using AslHelp.Memory.Native.Enums;
using AslHelp.Memory.Utils;

namespace AslHelp.Memory.Native;

internal static unsafe partial class WinInterop
{
    /// <summary>
    ///     Retrieves the termination status of the specified thread.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getexitcodethread">
    ///         GetExitCodeThread function (processthreadsapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="threadHandle">
    ///     A handle to the thread.
    /// </param>
    /// <param name="exitCode">
    ///     The thread termination status.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool GetExitCodeThread(nuint threadHandle, out uint exitCode)
    {
        fixed (uint* pExitCode = &exitCode)
        {
            return GetExitCodeThread((void*)threadHandle, pExitCode) != 0;
        }

        [DllImport(Lib.Kernel32, EntryPoint = nameof(GetExitCodeThread), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int GetExitCodeThread(
            void* hThread,
            uint* lpExitCode);
    }

    /// <summary>
    ///     Creates a thread that runs in the virtual address space of another process.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-createremotethread">
    ///         CreateRemoteThread function (processthreadsapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="hProcess">
    ///     A handle to the process in which the thread is to be created.
    /// </param>
    /// <param name="lpThreadAttributes">
    ///     A pointer to a SECURITY_ATTRIBUTES structure that specifies a security descriptor.
    /// </param>
    /// <param name="stackSize">
    ///     The initial size of the stack, in bytes.
    /// </param>
    /// <param name="startAddress">
    ///     The starting address of the thread in the remote process.
    /// </param>
    /// <param name="lpParameter">
    ///     A pointer to a variable to be passed to the thread function.
    /// </param>
    /// <param name="creationFlags">
    ///     The flags that control the creation of the thread.
    /// </param>
    /// <param name="threadId">
    ///     A pointer to a variable that receives the thread identifier.
    /// </param>
    /// <returns>
    ///     A handle to the new thread if the function succeeds,
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    public static nuint CreateRemoteThread(
        nuint hProcess,
        void* lpThreadAttributes,
        nuint stackSize,
        nuint startAddress,
        void* lpParameter,
        uint creationFlags,
        out uint threadId)
    {
        fixed (uint* pThreadId = &threadId)
        {
            return (nuint)CreateRemoteThread(
                (void*)hProcess,
                lpThreadAttributes,
                stackSize,
                (void*)startAddress,
                lpParameter,
                creationFlags,
                pThreadId);
        }

        [DllImport(Lib.Kernel32, EntryPoint = nameof(CreateRemoteThread), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern void* CreateRemoteThread(
            void* hProcess,
            void* lpThreadAttributes,
            nuint dwStackSize,
            void* lpStartAddress,
            void* lpParameter,
            uint dwCreationFlags,
            uint* lpThreadId);
    }

    /// <summary>
    ///     Opens an existing local process object.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-openprocess">
    ///         OpenProcess function (processthreadsapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processId">
    ///     The identifier of the local process to be opened.
    /// </param>
    /// <param name="desiredAccess">
    ///     The access to the process object.
    /// </param>
    /// <param name="inheritHandle">
    ///     If this value is <see langword="true"/>, processes created by this process will inherit the handle.
    /// </param>
    /// <returns>
    ///     An open handle to the specified process if the function succeeds,
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    public static nuint OpenProcess(uint processId, ProcessAccess desiredAccess, bool inheritHandle)
    {
        return (nuint)OpenProcess((uint)desiredAccess, inheritHandle ? 1 : 0, processId);

        [DllImport(Lib.Kernel32, EntryPoint = nameof(OpenProcess), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern void* OpenProcess(
            uint dwDesiredAccess,
            int bInheritHandle,
            uint dwProcessId);
    }
}
