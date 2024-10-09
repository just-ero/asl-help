using System;
using System.Runtime.InteropServices;
using System.Security;

using AslHelp.Memory.Native.Enums;
using AslHelp.Memory.Native.Structs;
using AslHelp.Memory.Utils;

namespace AslHelp.Memory.Native;

internal static unsafe partial class WinInterop
{
    public const int UnicodeStringMaxChars = 32767;

    public static bool EnumProcessModules(
        nint processHandle,
        Span<nint> modules,
        uint size,
        out uint bytesNeeded,
        ListModulesFilter filter)
    {
        fixed (nint* pModules = modules)
        fixed (uint* pBytesNeeded = &bytesNeeded)
        {
            return EnumProcessModulesEx((void*)processHandle, (void**)pModules, size, pBytesNeeded, (uint)filter) != 0;
        }

        [DllImport(Lib.PsApi, EntryPoint = nameof(EnumProcessModulesEx), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern uint EnumProcessModulesEx(
            void* hProcess,
            void** lphModule,
            uint cb,
            uint* lpcbNeeded,
            uint dwFilterFlag);
    }

    public static uint GetModuleFileName(nint processHandle, nint moduleHandle, Span<char> fileName)
    {
        fixed (char* pFileName = fileName)
        {
            return GetModuleFileNameExW((void*)processHandle, (void*)moduleHandle, (ushort*)pFileName, (uint)fileName.Length);
        }

        [DllImport(Lib.PsApi, EntryPoint = nameof(GetModuleFileNameExW), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern uint GetModuleFileNameExW(
            void* hProcess,
            void* hModule,
            ushort* lpFilename,
            uint nSize);
    }

    public static bool GetModuleInformation(nint processHandle, nint moduleHandle, out ModuleInfo moduleInfo)
    {
        fixed (ModuleInfo* pModuleInfo = &moduleInfo)
        {
            return GetModuleInformation((void*)processHandle, (void*)moduleHandle, pModuleInfo, (uint)sizeof(ModuleInfo)) != 0;
        }

        [DllImport(Lib.PsApi, EntryPoint = nameof(GetModuleInformation), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int GetModuleInformation(
            void* hProcess,
            void* hModule,
            ModuleInfo* lpmodinfo,
            uint cb);
    }
}
