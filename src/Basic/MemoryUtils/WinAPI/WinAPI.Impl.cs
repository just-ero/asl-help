using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AslHelp.MemUtils;

#pragma warning disable IDE1006
internal static unsafe partial class WinAPI
{
    public static bool Is64Bit(this Process process)
    {
        if (process is null || process.HasExited)
        {
            return false;
        }

        int Wow64Process;
        if (IsWow64Process(process.Handle, &Wow64Process) == 0)
        {
            throw new Win32Exception();
        }

        return Environment.Is64BitProcess && Wow64Process == 0;
    }

    public static bool Read(this Process process, nint address, void* buffer, int bufferSize)
    {
        if (process is null || process.HasExited)
        {
            return false;
        }

        nuint nSize = (nuint)bufferSize, nRead;

        return ReadProcessMemory(process.Handle, address, buffer, nSize, &nRead) != 0
               && nRead == nSize;
    }

    public static bool Write(this Process process, nint address, void* data, int dataSize)
    {
        if (process is null || process.HasExited)
        {
            return false;
        }

        nuint nSize = (nuint)dataSize, nWritten;

        return WriteProcessMemory(process.Handle, address, data, nSize, &nWritten) != 0
               && nWritten == nSize;
    }

    public static IEnumerable<Module> Modules(this Process process)
    {
        if (process is null || process.HasExited)
        {
            yield break;
        }

        nint hProcess = process.Handle;

        if (!EnumProcessModulesEx(hProcess, null, 0, out uint cbNeeded))
        {
            yield break;
        }

        int numModules = (int)(cbNeeded / Marshal.SizeOf<nint>());
        nint[] hModule = new nint[numModules];

        if (!EnumProcessModulesEx(hProcess, hModule, cbNeeded, out _))
        {
            yield break;
        }

        for (int i = 0; i < numModules; i++)
        {
            if (!GetModuleBaseNameW(hProcess, hModule[i], out string baseName))
            {
                yield break;
            }

            if (!GetModuleFileNameExW(hProcess, hModule[i], out string fileName))
            {
                yield break;
            }

            if (!GetModuleInformation(hProcess, hModule[i], out MODULEINFO moduleInfo))
            {
                yield break;
            }

            yield return new(baseName, fileName, moduleInfo);
        }
    }

    public static IEnumerable<MemoryPage> MemoryPages(this Process process, bool is64Bit)
    {
        if (process is null || process.HasExited)
        {
            yield break;
        }

        nint addr = 0x10000, max = (nint)(is64Bit ? 0x7FFFFFFEFFFF : 0x7FFEFFFF);

        while (VirtualQueryEx(process.Handle, addr, out MEMORY_BASIC_INFORMATION mbi))
        {
            addr += (nint)mbi.RegionSize;

            if (mbi.State != MemPageState.MEM_COMMIT)
            {
                continue;
            }

            yield return new(mbi);

            if (addr >= max)
            {
                break;
            }
        }
    }

    public static List<DebugSymbol> Symbols(this Module module, Process process)
    {
        if (process is null || process.HasExited || module is null)
        {
            return new();
        }

        nint hProcess = process.Handle;

        List<DebugSymbol> syms = new();
        void* pSyms = Unsafe.AsPointer(ref syms);

        getSymbols(null);
        getSymbols(Path.GetDirectoryName(module.FilePath));

        return syms;

        void getSymbols(string pdbDirectory)
        {
            if (!SymInitializeW(hProcess, pdbDirectory))
            {
                return;
            }

            try
            {
                if (!SymLoadModuleExW(hProcess, module))
                {
                    return;
                }

                if (!SymEnumSymbolsW(hProcess, module, pSyms))
                {
                    return;
                }
            }
            finally
            {
                SymCleanup(hProcess);
            }
        }
    }

    public static bool IsPointerType<T>() where T : unmanaged
    {
        return typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(IntPtr) || typeof(T) == typeof(UIntPtr);
    }

    public static int GetTypeSize<T>(bool is64Bit) where T : unmanaged
    {
        return IsPointerType<T>() ? (is64Bit ? 0x8 : 0x4) : sizeof(T);
    }
}
#pragma warning restore IDE1006
