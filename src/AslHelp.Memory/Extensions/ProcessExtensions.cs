using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

using AslHelp.Collections.Extensions;
using AslHelp.Memory.Errors;
using AslHelp.Memory.Native;
using AslHelp.Memory.Native.Enums;
using AslHelp.Memory.Native.Structs;
using AslHelp.Memory.Utils;
using AslHelp.Shared.Results;
using AslHelp.Shared.Results.Errors;

namespace AslHelp.Memory.Extensions;

public static class ProcessExtensions
{
    public static Result<bool> Is64Bit(this Process process)
    {
        nuint processHandle = (nuint)(nint)process.Handle;
        if (!WinInterop.IsWow64Process(processHandle, out bool isWow64))
        {
            return MemoryError.FromLastWin32Error();
        }

        return Environment.Is64BitOperatingSystem && !isWow64;
    }

    public static unsafe bool ReadMemory(this Process process, nuint address, void* buffer, uint bufferSize)
    {
        nuint processHandle = (nuint)(nint)process.Handle;
        return WinInterop.ReadProcessMemory(processHandle, address, buffer, bufferSize, out nuint nRead)
            && nRead == bufferSize;
    }

    public static unsafe bool WriteMemory(this Process process, nuint address, void* data, uint dataSize)
    {
        nuint processHandle = (nuint)(nint)process.Handle;
        return WinInterop.WriteProcessMemory(processHandle, address, data, dataSize, out nuint nWritten)
            && nWritten == dataSize;
    }

    public static Result<Module> GetModule(this Process process, string moduleName)
    {
        Func<Module, bool> filter =
            Path.IsPathRooted(moduleName)
            ? (module => module.FileName.Equals(moduleName, StringComparison.InvariantCultureIgnoreCase))
            : (module => module.Name.Equals(moduleName, StringComparison.InvariantCultureIgnoreCase));

        foreach (Module module in GetModules(process))
        {
            if (filter(module))
            {
                return module;
            }
        }

        return MemoryError.ModuleNotFound(moduleName);
    }

    public static IEnumerable<Module> GetModules(this Process process)
    {
        uint processId = (uint)process.Id;
        nuint snapshot = WinInterop.CreateToolhelp32Snapshot(processId, ThFlags.Module | ThFlags.Module32);

        try
        {
            ModuleEntry32 me = new() { Size = (uint)Unsafe.SizeOf<ModuleEntry32>() };
            if (!WinInterop.Module32First(snapshot, ref me))
            {
                yield break;
            }

            do
            {
                yield return new(me)
                {
                    ContainingProcess = process
                };
            } while (WinInterop.Module32Next(snapshot, ref me));
        }
        finally
        {
            WinInterop.CloseHandle(snapshot);
        }
    }

    public static unsafe Result<List<Module>> GetModulesLongPathSafe(this Process process)
    {
        nuint processHandle = (nuint)(nint)process.Handle;
        if (!WinInterop.EnumProcessModules(processHandle, [], 0, out uint bytesNeeded, ListModulesFilter.ListAll)
            || bytesNeeded == 0)
        {
            return MemoryError.FromLastWin32Error();
        }

        int count = (int)bytesNeeded / sizeof(nuint);
        nuint[]? mhRented = null;
        Span<nuint> moduleHandles =
            count <= 128
            ? stackalloc nuint[128]
            : (mhRented = ArrayPool<nuint>.Shared.Rent(count));

        if (!WinInterop.EnumProcessModules(processHandle, moduleHandles[..count], bytesNeeded, out _, ListModulesFilter.ListAll))
        {
            ArrayPool<nuint>.Shared.ReturnIfNotNull(mhRented);
            return MemoryError.FromLastWin32Error();
        }

        List<Module> modules = new(count);

        char[] fnRented;
        Span<char> fileName = fnRented = ArrayPool<char>.Shared.Rent(WinInterop.UnicodeStringMaxChars);

        foreach (nuint moduleHandle in moduleHandles[..count])
        {
            uint length = WinInterop.GetModuleFileName(processHandle, moduleHandle, fileName);
            if (length == 0)
            {
                Console.WriteLine(WinInteropWrapper.GetLastWin32ErrorMessage());
                continue;
            }

            if (!WinInterop.GetModuleInformation(processHandle, moduleHandle, out ModuleInfo moduleInfo))
            {
                continue;
            }

            modules.Add(new(moduleInfo, fileName[..(int)length].ToString())
            {
                ContainingProcess = process
            });
        }

        ArrayPool<nuint>.Shared.ReturnIfNotNull(mhRented);
        ArrayPool<char>.Shared.Return(fnRented);

        return modules;
    }

    public static IEnumerable<MemoryRange> GetMemoryPages(this Process process)
    {
        if (!Is64Bit(process)
            .TryUnwrap(out bool is64Bit, out _))
        {
            yield break;
        }

        nuint processHandle = (nuint)(nint)process.Handle;
        nuint address = 0x10000, max = (nuint)(is64Bit ? 0x7FFFFFFEFFFF : 0x7FFEFFFF);

        do
        {
            if (WinInterop.VirtualQuery(processHandle, address, out MemoryBasicInformation mbi) == 0)
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

    public static Result<Module> Inject(this Process process, string dllToInject)
    {
        if (GetModule(process, dllToInject)
            .TryUnwrap(out Module? module, out _)
            && module is not null)
        {
            return module;
        }

        return Is64Bit(process)
            .AndThen(is64Bit => is64Bit
                ? Inject64(process, dllToInject)
                : Inject32(process, dllToInject))
            .AndThen(() => GetModule(process, dllToInject));
    }

    private static unsafe Result Inject32(this Process process, string dllToInject)
    {
        if (!GetModule(process, Lib.Kernel32)
            .AndThen(kernel32 => kernel32.GetSymbol("LoadLibraryW"))
            .TryUnwrap(out DebugSymbol loadLibraryW, out IResultError? err))
        {
            return Result.Err(err);
        }

        if (!Allocate(process, dllToInject)
            .TryUnwrap(out nuint pDll, out err))
        {
            return Result.Err(err);
        }

        if (!CreateRemoteThreadAndGetExitCode(process, loadLibraryW.Address, pDll)
            .TryUnwrap(out uint exitCode, out err))
        {
            Free(process, pDll);
            return Result.Err(err);
        }

        Free(process, pDll);
        return exitCode != 0
            ? Result.Ok()
            : MemoryError.FromLastWin32Error();
    }

    private static unsafe Result Inject64(this Process process, string dllToInject)
    {
        nuint kernel32 = WinInterop.GetModuleHandle(Lib.Kernel32);
        if (kernel32 == 0)
        {
            return MemoryError.FromLastWin32Error();
        }

        nuint loadLibraryW = WinInterop.GetProcAddress(kernel32, "LoadLibraryW"u8);
        if (loadLibraryW == 0)
        {
            WinInterop.CloseHandle(kernel32);
            return MemoryError.FromLastWin32Error();
        }

        if (!Allocate(process, dllToInject)
            .TryUnwrap(out nuint pDll, out IResultError? err))
        {
            WinInterop.CloseHandle(kernel32);
            return Result.Err(err);
        }

        if (!CreateRemoteThreadAndGetExitCode(process, loadLibraryW, pDll)
            .TryUnwrap(out uint exitCode, out err))
        {
            Free(process, pDll);
            return Result.Err(err);
        }

        Free(process, pDll);
        return exitCode != 0
            ? Result.Ok()
            : MemoryError.FromLastWin32Error();
    }

    public static unsafe Result<uint> CreateRemoteThreadAndGetExitCode(this Process process, nuint startAddress, nuint arg)
    {
        nuint processHandle = (nuint)(nint)process.Handle;
        nuint threadHandle = WinInterop.CreateRemoteThread(processHandle, null, 0, startAddress, (void*)arg, 0, out _);
        if (threadHandle == 0)
        {
            return MemoryError.FromLastWin32Error();
        }

        if (WinInterop.WaitForSingleObject(threadHandle, uint.MaxValue) != 0)
        {
            return MemoryError.FromLastWin32Error();
        }

        if (!WinInterop.GetExitCodeThread(threadHandle, out uint result))
        {
            return MemoryError.FromLastWin32Error();
        }

        WinInterop.CloseHandle(threadHandle);

        return result;
    }

    public static unsafe Result<nuint> Allocate<T>(this Process process, T data)
        where T : unmanaged
    {
        return Allocate(process, &data, (uint)sizeof(T));
    }

    public static unsafe Result<nuint> Allocate(this Process process, string value)
    {
        fixed (char* pValue = value)
        {
            uint length = (uint)((value.Length + 1) * sizeof(char));
            return Allocate(process, pValue, length);
        }
    }

    public static unsafe Result<nuint> Allocate(this Process process, ReadOnlySpan<byte> value)
    {
        fixed (byte* pValue = value)
        {
            uint length = (uint)(value.Length + 1);
            return Allocate(process, pValue, length);
        }
    }

    public static unsafe Result<nuint> Allocate(this Process process, void* data, uint dataSize)
    {
        nuint processHandle = (nuint)(nint)process.Handle;
        nuint pRemote = WinInterop.VirtualAlloc(
            processHandle,
            0,
            dataSize,
            MemoryRangeState.Commit | MemoryRangeState.Reserve,
            MemoryRangeProtect.ExecuteReadWrite);

        if (pRemote == 0)
        {
            return MemoryError.FromLastWin32Error();
        }

        if (!WriteMemory(process, pRemote, data, dataSize))
        {
            return MemoryError.FromLastWin32Error();
        }

        return pRemote;
    }

    public static Result Free(this Process process, nuint address)
    {
        nuint processHandle = (nuint)(nint)process.Handle;
        return WinInterop.VirtualFree(processHandle, address, 0, MemoryRangeState.Release)
            ? Result.Ok()
            : MemoryError.FromLastWin32Error();
    }
}
