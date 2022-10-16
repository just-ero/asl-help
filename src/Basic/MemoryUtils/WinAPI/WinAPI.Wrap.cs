using System.Runtime.CompilerServices;

namespace AslHelp.MemUtils;

// This file contains various wrappers around the native API, making it possible to use some of the
// unsafe functions in a safe context.

#pragma warning disable IDE1006
internal static unsafe partial class WinAPI
{
    private const uint MAX_PATH = 260;
    private const int LIST_MODULES_ALL = 3;

    private static bool EnumProcessModulesEx(
        nint hProcess,
        Span<nint> hModule,
        uint cb,
        out uint cbNeeded)
    {
        fixed (nint* lphModule = hModule)
        fixed (uint* lpcbNeeded = &cbNeeded)
        {
            return EnumProcessModulesEx(
                hProcess,
                lphModule,
                cb,
                lpcbNeeded,
                LIST_MODULES_ALL) != 0;
        }
    }

    private static bool GetModuleBaseNameW(nint hProcess, nint hModule, out string baseName)
    {
        ushort* buffer = stackalloc ushort[520];

        if (GetModuleBaseNameW(hProcess, hModule, buffer, MAX_PATH) == 0)
        {
            baseName = null;
            return false;
        }

        baseName = new((char*)buffer);
        return true;
    }

    private static bool GetModuleFileNameExW(nint hProcess, nint hModule, out string fileName)
    {
        ushort* buffer = stackalloc ushort[520];

        if (GetModuleFileNameExW(hProcess, hModule, buffer, MAX_PATH) == 0)
        {
            fileName = null;
            return false;
        }

        fileName = new((char*)buffer);
        return true;
    }

    private static bool GetModuleInformation(
        nint hProcess,
        nint hModule,
        out MODULEINFO moduleInfo)
    {
        fixed (MODULEINFO* lpmodinfo = &moduleInfo)
        {
            return GetModuleInformation(hProcess, hModule, lpmodinfo, MODULEINFO.SelfSize) != 0;
        }
    }

    private static bool VirtualQueryEx(
        nint hProcess,
        nint address,
        out MEMORY_BASIC_INFORMATION mbi)
    {
        fixed (MEMORY_BASIC_INFORMATION* lpBuffer = &mbi)
        {
            return VirtualQueryEx(
                hProcess,
                address,
                lpBuffer,
                MEMORY_BASIC_INFORMATION.SelfSize) != 0;
        }
    }

    private static bool SymInitializeW(nint hProcess, string userSearchPath)
    {
        fixed (char* lpUserSearchPath = userSearchPath)
        {
            return SymInitializeW(hProcess, (ushort*)lpUserSearchPath, 0) != 0;
        }
    }

    private static bool SymLoadModuleExW(nint hProcess, Module module)
    {
        fixed (char* lpImageName = module.Name)
        {
            return SymLoadModuleExW(
                hProcess,
                0,
                (ushort*)lpImageName,
                null,
                (ulong)module.Base,
                (uint)module.MemorySize,
                0,
                0) != 0;
        }
    }

    private static bool SymEnumSymbolsW(nint hProcess, Module module, void* pSymbols)
    {
        ushort* mask = stackalloc ushort[2] { '*', '\0' };
        return SymEnumSymbolsW(hProcess, (ulong)module.Base, mask, &EnumSymbolsCallback, pSymbols) != 0;
    }

    private static int EnumSymbolsCallback(SYMBOL_INFOW* pSymInfo, uint SymbolSize, void* UserContext)
    {
        Unsafe.AsRef<List<DebugSymbol>>(UserContext).Add(new(*pSymInfo));
        return 1;
    }
}
#pragma warning restore IDE1006
