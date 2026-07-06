using System;
using System.Runtime.InteropServices;
using System.Security;

namespace AslHelp.Memory.Win32;

internal static unsafe partial class PInvoke
{
    public const int UnicodeStringMaxChars = 32767;

    /// <summary>
    ///     Retrieves a handle for each module in the specified process that meets the specified filter criteria.<br/>
    ///     For further information see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/psapi/nf-psapi-enumprocessmodulesex#parameters">
    ///         EnumProcessModulesEx function (psapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process.
    /// </param>
    /// <param name="modules">
    ///     An array that receives the list of module handles.
    /// </param>
    /// <param name="count">
    ///     The number of module handles to store in the <paramref name="modules"/> array.
    /// </param>
    /// <param name="filter">
    ///     The filter criteria.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool EnumProcessModules(
        SafeProcessHandle processHandle,
        Span<nuint> modules,
        out int count,
        ListModulesFilter filter)
    {
        var cb = checked((uint)modules.Length * (uint)sizeof(nuint));
        uint bytesNeeded;

        fixed (nuint* pModules = modules)
        {
            var success = EnumProcessModulesEx(processHandle, (void**)pModules, cb, &bytesNeeded, (uint)filter) != 0;

            count = (int)(bytesNeeded / (uint)sizeof(nuint));
            return success;
        }

        [DllImport(PsApi, EntryPoint = nameof(EnumProcessModulesEx), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern uint EnumProcessModulesEx(
            SafeProcessHandle hProcess,
            void** lphModule,
            uint cb,
            uint* lpcbNeeded,
            uint dwFilterFlag);
    }

    /// <summary>
    ///     Retrieves the fully qualified path for the file containing the specified module.<br/>
    ///     For further information see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/psapi/nf-psapi-getmodulefilenameexw">
    ///         GetModuleFileNameExW function (psapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process that contains the module.
    /// </param>
    /// <param name="moduleHandle">
    ///     A handle to the module.
    /// </param>
    /// <param name="fileName">
    ///     A buffer that receives the fully qualified path to the module.
    /// </param>
    /// <returns>
    ///     The length of the string copied to the buffer, if the function succeeds;
    ///     otherwise, <c>0</c>.
    /// </returns>
    public static int GetModuleFileName(SafeProcessHandle processHandle, nuint moduleHandle, Span<char> fileName)
    {
        fixed (char* pFileName = fileName)
        {
            return (int)GetModuleFileNameExW(processHandle, (void*)moduleHandle, (ushort*)pFileName, (uint)fileName.Length);
        }

        [DllImport(PsApi, EntryPoint = nameof(GetModuleFileNameExW), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern uint GetModuleFileNameExW(
            SafeProcessHandle hProcess,
            void* hModule,
            ushort* lpFilename,
            uint nSize);
    }

    /// <summary>
    ///     Retrieves information about the specified module in the <see cref="ModuleInfo"/> structure.<br/>
    ///     For further information see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/psapi/nf-psapi-getmoduleinformation">
    ///         GetModuleInformation function (psapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process that contains the module.
    /// </param>
    /// <param name="moduleHandle">
    ///     A handle to the module.
    /// </param>
    /// <param name="moduleInfo">
    ///     The <see cref="ModuleInfo"/> structure that receives information about the module.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool GetModuleInformation(SafeProcessHandle processHandle, nuint moduleHandle, out ModuleInfo moduleInfo)
    {
        fixed (ModuleInfo* pModuleInfo = &moduleInfo)
        {
            return GetModuleInformation(processHandle, (void*)moduleHandle, pModuleInfo, (uint)sizeof(ModuleInfo)) != 0;
        }

        [DllImport(PsApi, EntryPoint = nameof(GetModuleInformation), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int GetModuleInformation(
            SafeProcessHandle hProcess,
            void* hModule,
            ModuleInfo* lpmodinfo,
            uint cb);
    }
}
