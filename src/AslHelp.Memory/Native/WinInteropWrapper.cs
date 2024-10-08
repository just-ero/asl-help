using System;
using System.Runtime.InteropServices;

using AslHelp.Memory.Native.Enums;

namespace AslHelp.Memory.Native;

internal static unsafe partial class WinInteropWrapper
{
    public static string GetLastWin32ErrorMessage(this Module module)
    {
        return GetLastWin32ErrorMessage(module.Base);
    }

    public static string GetLastWin32ErrorMessage(nuint moduleHandle = 0)
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
}
