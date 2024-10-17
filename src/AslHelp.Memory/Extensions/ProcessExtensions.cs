using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

using AslHelp.Memory.Errors;
using AslHelp.Memory.Native;
using AslHelp.Memory.Native.Enums;
using AslHelp.Memory.Native.Structs;
using AslHelp.Memory.Utils;
using AslHelp.Shared.Extensions;
using AslHelp.Shared.Results;
using AslHelp.Shared.Results.Errors;

namespace AslHelp.Memory.Extensions;

public static class ProcessExtensions
{
    public static Result<bool> Is64Bit(this Process process)
    {
        if (!WinInterop.IsWow64Process(process.Handle, out bool isWow64))
        {
            return MemoryError.FromLastWin32Error();
        }

        return Environment.Is64BitOperatingSystem && !isWow64;
    }

    public static unsafe bool ReadMemory(this Process process, nint address, void* buffer, nint bufferSize)
    {
        return WinInterop.ReadProcessMemory(process.Handle, address, buffer, bufferSize, out nint nRead)
            && nRead == bufferSize;
    }

    public static unsafe bool WriteMemory(this Process process, nint address, void* data, nint dataSize)
    {
        return WinInterop.WriteProcessMemory(process.Handle, address, data, dataSize, out nint nWritten)
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
        nint snapshot = WinInterop.CreateToolhelp32Snapshot(process.Id, ThFlags.Module | ThFlags.Module32);

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
        nint processHandle = process.Handle;
        if (!WinInterop.EnumProcessModules(processHandle, [], 0, out uint bytesNeeded, ListModulesFilter.ListAll)
            || bytesNeeded == 0)
        {
            return MemoryError.FromLastWin32Error();
        }

        int count = (int)bytesNeeded / sizeof(nint);
        nint[]? mhRented = null;
        Span<nint> moduleHandles =
            count <= 128
            ? stackalloc nint[128]
            : (mhRented = ArrayPool<nint>.Shared.Rent(count));

        if (!WinInterop.EnumProcessModules(processHandle, moduleHandles[..count], bytesNeeded, out _, ListModulesFilter.ListAll))
        {
            ArrayPool<nint>.Shared.ReturnIfNotNull(mhRented);
            return MemoryError.FromLastWin32Error();
        }

        List<Module> modules = new(count);

        char[] fnRented;
        Span<char> fileName = fnRented = ArrayPool<char>.Shared.Rent(WinInterop.UnicodeStringMaxChars);

        foreach (nint moduleHandle in moduleHandles[..count])
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

        ArrayPool<nint>.Shared.ReturnIfNotNull(mhRented);
        ArrayPool<char>.Shared.Return(fnRented);

        return modules;
    }

    public static IEnumerable<MemoryRange> GetMemoryPages(this Process process)
    {
        if (!Is64Bit(process)
            .TryUnwrap(out bool is64Bit, out _))
        {
            return [];
        }

        return WinInteropWrapper.GetMemoryPages(process.Handle, is64Bit);
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
            .TryUnwrap(out nint pDll, out err))
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
        nint kernel32 = WinInterop.GetModuleHandle(Lib.Kernel32);
        if (kernel32 == 0)
        {
            return MemoryError.FromLastWin32Error();
        }

        nint loadLibraryW = WinInterop.GetProcAddress(kernel32, "LoadLibraryW"u8);
        if (loadLibraryW == 0)
        {
            WinInterop.CloseHandle(kernel32);
            return MemoryError.FromLastWin32Error();
        }

        if (!Allocate(process, dllToInject)
            .TryUnwrap(out nint pDll, out IResultError? err))
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

    public static unsafe Result<uint> CreateRemoteThreadAndGetExitCode(this Process process, nint startAddress, nint arg)
    {
        nint threadHandle = WinInterop.CreateRemoteThread(process.Handle, null, 0, startAddress, (void*)arg, 0, out _);
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

    public static unsafe Result<nint> Allocate<T>(this Process process, T data)
        where T : unmanaged
    {
        return Allocate(process, &data, sizeof(T));
    }

    public static unsafe Result<nint> Allocate(this Process process, string value)
    {
        fixed (char* pValue = value)
        {
            int length = (value.Length + 1) * sizeof(char);
            return Allocate(process, pValue, length);
        }
    }

    public static unsafe Result<nint> Allocate(this Process process, ReadOnlySpan<byte> value)
    {
        fixed (byte* pValue = value)
        {
            int length = value.Length + 1;
            return Allocate(process, pValue, length);
        }
    }

    public static unsafe Result<nint> Allocate(this Process process, void* data, int dataSize)
    {
        nint pRemote = WinInterop.VirtualAlloc(
            process.Handle,
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

    public static Result Free(this Process process, nint address)
    {
        return WinInterop.VirtualFree(process.Handle, address, 0, MemoryRangeState.Release)
            ? Result.Ok()
            : MemoryError.FromLastWin32Error();
    }
}
