using System.Runtime.InteropServices;
using System.Security;

namespace AslHelp.Memory.Win32;

internal static unsafe partial class PInvoke
{
    /// <summary>
    ///     Initializes the symbol handler for a process.<br/>
    ///     For further information, see:
    ///     <i><see href="https://docs.microsoft.com/windows/win32/api/dbghelp/nf-dbghelp-syminitializew">
    ///         SymInitializeW function (dbghelp.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle that identifies the caller.
    /// </param>
    /// <param name="userSearchPath">
    ///     The path, or series of paths separated by a semicolon, that is used to search for symbol files.
    /// </param>
    /// <param name="invadeProcess">
    ///     If <see langword="true"/>, enumerates the loaded modules for the process and effectively calls the
    ///     <see cref="SymLoadModule"/> function for each module.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool SymInitialize(SafeProcessHandle processHandle, string? userSearchPath, bool invadeProcess)
    {
        fixed (char* pUserSearchPath = userSearchPath)
        {
            return SymInitializeW(processHandle, (ushort*)pUserSearchPath, invadeProcess ? 1 : 0) != 0;
        }

        [DllImport(DbgHelp, EntryPoint = nameof(SymInitializeW), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int SymInitializeW(
            SafeProcessHandle hProcess,
#pragma warning disable IDE1006 // Naming Styles
            ushort* UserSearchPath,
#pragma warning restore IDE1006
            int fInvadeProcess);
    }

    /// <summary>
    ///     Loads the symbol table for the specified module.<br/>
    ///     For further information, see:
    ///     <i><see href="https://docs.microsoft.com/windows/win32/api/dbghelp/nf-dbghelp-symloadmoduleexw">
    ///         SymLoadModuleExW function (dbghelp.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process that was originally passed to the <see cref="SymInitialize"/> function.
    /// </param>
    /// <param name="moduleFileName">
    ///     The name of the executable image.
    /// </param>
    /// <param name="moduleBase">
    ///     The load address of the module.
    /// </param>
    /// <param name="moduleMemorySize">
    ///     The size of the module, in bytes.
    /// </param>
    /// <returns>
    ///     The base address of the loaded module if the function succeeds;
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    public static nuint SymLoadModule(
        SafeProcessHandle processHandle,
        string moduleFileName,
        nuint moduleBase,
        uint moduleMemorySize)
    {
        fixed (char* pImageName = moduleFileName)
        {
            return (nuint)SymLoadModuleExW(processHandle, null, (ushort*)pImageName, null, moduleBase, moduleMemorySize, null, 0);
        }

        [DllImport(DbgHelp, EntryPoint = nameof(SymLoadModuleExW), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern ulong SymLoadModuleExW(
            SafeProcessHandle hProcess,
            void* hFile,
#pragma warning disable IDE1006 // Naming Styles
            ushort* ImageName,
            ushort* ModuleName,
            ulong BaseOfDll,
            uint DllSize,
            void* Data,
            uint Flags);
#pragma warning restore IDE1006
    }

    /// <summary>
    ///     Retrieves symbol information for the specified name.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/dbghelp/nf-dbghelp-symfromnamew">
    ///         SymFromNameW function (dbghelp.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process that was originally passed to the <see cref="SymInitialize"/> function.
    /// </param>
    /// <param name="name">
    ///     The name of the symbol to be located.
    /// </param>
    /// <param name="symbol">
    ///     The <see cref="SymbolInfo"/> structure that provides information about the symbol.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool SymFromName(SafeProcessHandle processHandle, string name, out SymbolInfo symbol)
    {
        var tSym = new SymbolInfo { SizeOfStruct = (uint)sizeof(SymbolInfo) };

        fixed (char* pName = name)
        {
            if (SymFromNameW(processHandle, (ushort*)pName, &tSym) != 0)
            {
                symbol = tSym;
                return true;
            }

            symbol = default;
            return false;
        }

        [DllImport(DbgHelp, EntryPoint = nameof(SymFromNameW), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int SymFromNameW(
            SafeProcessHandle hProcess,
#pragma warning disable IDE1006 // Naming Styles
            ushort* Name,
            SymbolInfo* Symbol);
#pragma warning restore IDE1006
    }

    /// <summary>
    ///     Deallocates all resources associated with the process handle.<br/>
    ///     For further information, see:
    ///     <i><see href="https://docs.microsoft.com/windows/win32/api/dbghelp/nf-dbghelp-symcleanup">
    ///         SymCleanup function (dbghelp.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process that was originally passed to the <see cref="SymInitialize"/> function.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool SymCleanup(SafeProcessHandle processHandle)
    {
        return SymCleanup(processHandle) != 0;

        [DllImport(DbgHelp, EntryPoint = nameof(SymCleanup), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int SymCleanup(
            SafeProcessHandle hProcess);
    }
}
