using System.Runtime.InteropServices;

namespace AslHelp.MemUtils;

#pragma warning disable IDE1006
internal static unsafe partial class WinAPI
{
    [DllImport("kernel32")]
    private static extern int IsWow64Process(
        nint hProces,
        int* Wow64Process);

    [DllImport("kernel32")]
    private static extern int ReadProcessMemory(
        nint hProcess,
        nint lpBaseAddress,
        void* lpBuffer,
        nuint nSize,
        nuint* lpNumberOfBytesRead);

    [DllImport("kernel32")]
    private static extern int WriteProcessMemory(
        nint hProcess,
        nint lpBaseAddress,
        void* lpBuffer,
        nuint nSize,
        nuint* lpNumberOfBytesWritten);

    [DllImport("kernel32")]
    private static extern nuint VirtualQueryEx(
        nint hProcess,
        nint lpAddress,
        MEMORY_BASIC_INFORMATION* lpBuffer,
        nuint dwLength);

    [DllImport("psapi")]
    private static extern int EnumProcessModulesEx(
        nint hProcess,
        nint* lphModule,
        uint cb,
        uint* lpcbNeeded,
        uint dwFilterFlag);

    [DllImport("psapi")]
    private static extern uint GetModuleBaseNameW(
        nint hProcess,
        nint hModule,
        ushort* lpBaseName,
        uint nSize);

    [DllImport("psapi")]
    private static extern uint GetModuleFileNameExW(
        nint hProcess,
        nint hModule,
        ushort* lpFilename,
        uint nSize);

    [DllImport("psapi")]
    private static extern int GetModuleInformation(
        nint hProcess,
        nint hModule,
        MODULEINFO* lpmodinfo,
        uint cb);

    [DllImport("dbghelp")]
    private static extern int SymInitializeW(
        nint hProcess,
        ushort* UserSearchPath,
        int fInvadeProcess);

    [DllImport("dbghelp")]
    private static extern ulong SymLoadModuleExW(
        nint hProcess,
        nint hFile,
        ushort* ImageName,
        ushort* module,
        ulong BaseOfDll,
        uint DllSize,
        nint Data,
        uint Flags);

    [DllImport("dbghelp")]
    private static extern int SymEnumSymbolsW(
        nint hProcess,
        ulong BaseOfDll,
        ushort* Mask,
        delegate*<SYMBOL_INFOW*, uint, void*, int> EnumSymbolsCallback,
        void* UserContext);

    private delegate int PSYM_ENUMERATESYMBOLS_CALLBACK(
        SYMBOL_INFOW* pSymInfo,
        uint SymbolSize,
        void* UserContext);

    [DllImport("dbghelp")]
    private static extern int SymCleanup(
        nint hProcess);
}
#pragma warning restore IDE1006
