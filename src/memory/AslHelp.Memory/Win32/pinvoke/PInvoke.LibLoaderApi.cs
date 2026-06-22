using System;
using System.Runtime.InteropServices;
using System.Security;

namespace AslHelp.Memory.Win32;

internal static unsafe partial class PInvoke
{
    /// <summary>
    ///     Retrieves a module handle for the specified module. The module must have been loaded by the calling process.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/libloaderapi/nf-libloaderapi-getmodulehandlew">
    ///         GetModuleHandleW function (libloaderapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="moduleName">
    ///     The name of the loaded module.
    /// </param>
    /// <returns>
    ///     A handle to the specified module, if the function succeeds;
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    public static SafeLibraryHandle GetModuleHandle(string? moduleName)
    {
        fixed (char* pModuleName = moduleName)
        {
            return GetModuleHandleW((ushort*)pModuleName);
        }

        [DllImport(Kernel32, EntryPoint = nameof(GetModuleHandleW), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern SafeLibraryHandle GetModuleHandleW(
            ushort* lpModuleName);
    }

    /// <summary>
    ///     Retrieves the address of an exported function or variable from the specified DLL.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/libloaderapi/nf-libloaderapi-getprocaddress">
    ///         GetProcAddress function (libloaderapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="moduleHandle">
    ///     A handle to the DLL module that contains the function or variable.
    /// </param>
    /// <param name="procedure">
    ///     The function or variable name, or the function's ordinal value.
    /// </param>
    /// <returns>
    ///     The address of the exported function or variable, if the function succeeds;
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    public static nuint GetProcAddress(SafeLibraryHandle moduleHandle, ReadOnlySpan<byte> procedure)
    {
        fixed (byte* pProcedure = procedure)
        {
            return (nuint)GetProcAddress(moduleHandle, pProcedure);
        }

        [DllImport(Kernel32, EntryPoint = nameof(GetProcAddress), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern void* GetProcAddress(
            SafeLibraryHandle hModule,
            byte* lpProcName);
    }
}
