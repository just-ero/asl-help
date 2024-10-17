using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using AslHelp.Memory.Native.Enums;
using AslHelp.Memory.Native.Structs;
using AslHelp.Shared;

namespace AslHelp.Memory.Native;

internal static unsafe class WinInteropWrapper
{
    public static string GetLastWin32ErrorMessage(this Module module)
    {
        return GetLastWin32ErrorMessage(module.Base);
    }

    public static string GetLastWin32ErrorMessage(nint moduleHandle = 0)
    {
        const int ERROR_INSUFFICIENT_BUFFER = 0x7A;

        int errorCode = Marshal.GetLastWin32Error();
        if (errorCode == 0)
        {
            return string.Empty;
        }

        FormatMessageFlags flags = FormatMessageFlags.IgnoreInserts | FormatMessageFlags.FromSystem | FormatMessageFlags.ArgumentArray;
        if (moduleHandle != 0)
        {
            flags |= FormatMessageFlags.FromModuleHandle;
        }

        Span<char> buffer = stackalloc char[256];

        fixed (char* pBuffer = buffer)
        {
            int length = WinInterop.FormatMessage(flags, moduleHandle, (uint)errorCode, 0, pBuffer, (uint)buffer.Length, null);
            if (length > 0)
            {
                return $"{getAndTrimString(buffer[..length])} (0x{errorCode:X})";
            }
        }

        if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
        {
            flags |= FormatMessageFlags.AllocateBuffer;

            nint nativeMsgPtr;
            int length = WinInterop.FormatMessage(flags, moduleHandle, (uint)errorCode, 0, (char*)&nativeMsgPtr, 0, null);

            if (length > 0)
            {
                string message = $"{getAndTrimString(new((char*)nativeMsgPtr, length))} (0x{errorCode:X})";

                Marshal.FreeHGlobal(nativeMsgPtr);

                return message;
            }
        }

        return $"Unknown error (0x{errorCode:X})";

        static string getAndTrimString(ReadOnlySpan<char> buffer)
        {
            int length = buffer.Length;
            while (length > 0 && buffer[length - 1] <= ' ')
            {
                length--;
            }

            return buffer[..length].ToString();
        }
    }

    public static bool ProcessIs64Bit(nint processHandle)
    {
        if (!WinInterop.IsWow64Process(processHandle, out bool isWow64))
        {
            ThrowHelper.ThrowWin32Exception();
        }

        return Environment.Is64BitOperatingSystem && !isWow64;
    }

    public static bool ReadMemory(nint processHandle, nint address, void* buffer, nint bufferSize)
    {
        return WinInterop.ReadProcessMemory(processHandle, address, buffer, bufferSize, out nint nRead)
            && nRead == bufferSize;
    }

    public static bool WriteMemory(nint processHandle, nint address, void* data, nint dataSize)
    {
        return WinInterop.WriteProcessMemory(processHandle, address, data, dataSize, out nint nWritten)
            && nWritten == dataSize;
    }

    public static IEnumerable<MemoryRange> GetMemoryPages(nint processHandle, bool is64Bit)
    {
        nint address = 0x10000, max = (nint)(is64Bit ? 0x7FFFFFFEFFFF : 0x7FFEFFFF);

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
}
