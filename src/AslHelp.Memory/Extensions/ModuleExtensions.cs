using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using AslHelp.Memory.Errors;
using AslHelp.Memory.Native;
using AslHelp.Memory.Native.Enums;
using AslHelp.Memory.Native.Structs;
using AslHelp.Memory.Utils;
using AslHelp.Shared.Results;
using AslHelp.Shared.Results.Errors;

namespace AslHelp.Memory.Extensions;

public static class ModuleExtensions
{
    public static IEnumerable<MemoryRange> GetMemoryPages(this Module module)
    {
        nint address = module.Base, max = module.Base + module.MemorySize;

        do
        {
            if (WinInterop.VirtualQuery(module.ContainingProcess.Handle, address, out MemoryBasicInformation mbi) == 0)
            {
                break;
            }

            address += mbi.RegionSize;

            if (mbi.State != MemoryRangeState.Commit)
            {
                continue;
            }

            if ((mbi.Protect & MemoryRangeProtect.NoAccess) != 0)
            {
                continue;
            }

            yield return new(mbi);
        } while (address < max);
    }

    public static unsafe Result<DebugSymbol> GetSymbol(this Module module, string symbolName, string? pdbDirectory = null)
    {
        nint processHandle = module.ContainingProcess.Handle;
        if (!WinInterop.SymInitialize(processHandle, pdbDirectory, false))
        {
            return MemoryError.FromLastWin32Error();
        }

        nint symLoadBase = WinInterop.SymLoadModule(processHandle, 0, module.FileName, null, module.Base, module.MemorySize, null, 0);
        if (symLoadBase == 0)
        {
            WinInterop.SymCleanup(processHandle);
            return MemoryError.FromLastWin32Error();
        }

        if (!WinInterop.SymFromName(processHandle, symbolName, out SymbolInfo symbol))
        {
            WinInterop.SymCleanup(processHandle);
            return MemoryError.FromLastWin32Error();
        }

        WinInterop.SymCleanup(processHandle);

        return new DebugSymbol(symbol);
    }

    public static unsafe Result<List<DebugSymbol>> GetSymbols(this Module module, string? symbolMask = "*", string? pdbDirectory = null)
    {
        nint processHandle = module.ContainingProcess.Handle;

        var callback =
            (delegate* unmanaged[Stdcall]<SymbolInfo*, uint, void*, int>)Marshal.GetFunctionPointerForDelegate(enumSymbolsCallback);

        List<DebugSymbol> symbols = [];
        void* pSymbols = Unsafe.AsPointer(ref symbols);

        if (!WinInterop.SymInitialize(processHandle, pdbDirectory, false))
        {
            return MemoryError.FromLastWin32Error();
        }

        nint symLoadBase = WinInterop.SymLoadModule(processHandle, 0, module.FileName, null, module.Base, module.MemorySize, null, 0);
        if (symLoadBase == 0)
        {
            WinInterop.SymCleanup(processHandle);
            return MemoryError.FromLastWin32Error();
        }

        if (!WinInterop.SymEnumSymbols(processHandle, symLoadBase, symbolMask, callback, pSymbols))
        {
            WinInterop.SymCleanup(processHandle);
            return MemoryError.FromLastWin32Error();
        }

        WinInterop.SymCleanup(processHandle);

        return symbols;

        static int enumSymbolsCallback(SymbolInfo* pSymInfo, uint symbolSize, void* userContext)
        {
            Unsafe.AsRef<List<DebugSymbol>>(userContext).Add(new(*pSymInfo));

            return 1;
        }
    }

    public static Result Eject(this Module module)
    {
        return module.ContainingProcess.GetModule(Lib.Kernel32)
            .AndThen(kernel32 => GetSymbol(kernel32, "FreeLibrary"))
            .AndThen(sym => module.ContainingProcess.CreateRemoteThreadAndGetExitCode(sym.Address, module.Base))
            .AndThen(exitCode => exitCode != 0
                ? Result.Ok()
                : MemoryError.FromLastWin32Error());
    }

    public static Result<uint> CallRemoteFunction<T>(this Module module, string functionName, T arg)
        where T : unmanaged
    {
        return module.ContainingProcess.Is64Bit()
            .AndThen(is64Bit => is64Bit
                ? CallRemoteFunction64(module, functionName, arg)
                : CallRemoteFunction32(module, functionName, arg));
    }

    private static unsafe Result<uint> CallRemoteFunction32<T>(this Module module, string functionName, T arg)
        where T : unmanaged
    {
        if (!GetSymbol(module, functionName)
            .TryUnwrap(out DebugSymbol function, out IResultError? err))
        {
            return Result<uint>.Err(err);
        }

        if (!module.ContainingProcess.Allocate(arg)
            .TryUnwrap(out nint pArg, out err))
        {
            return Result<uint>.Err(err);
        }

        Result<uint> result = module.ContainingProcess.CreateRemoteThreadAndGetExitCode(function.Address, pArg);

        module.ContainingProcess.Free(pArg);
        return result;
    }

    private static unsafe Result<uint> CallRemoteFunction64<T>(this Module module, string functionName, T arg)
        where T : unmanaged
    {
        nint pFunction = WinInterop.GetProcAddress(module.Base, Encoding.UTF8.GetBytes(functionName));
        if (pFunction == 0)
        {
            return MemoryError.FromLastWin32Error();
        }

        if (!module.ContainingProcess.Allocate(arg)
            .TryUnwrap(out nint pArg, out IResultError? err))
        {
            return Result<uint>.Err(err);
        }

        Result<uint> result = module.ContainingProcess.CreateRemoteThreadAndGetExitCode(pFunction, pArg);

        module.ContainingProcess.Free(pArg);
        return result;
    }
}
