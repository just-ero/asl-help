using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace AslHelp.Memory.Win32;

/// <summary>
///     Provides extension members for <see cref="Marshal"/>.
/// </summary>
internal static unsafe class MarshalExtensions
{
    extension(Marshal)
    {
        /// <summary>
        ///     Retrieves the human-readable message associated with the calling thread's last Win32 error code
        ///     by formatting it via <see cref="PInvoke.FormatMessage"/>.
        /// </summary>
        /// <param name="moduleHandle">
        ///     An optional handle to the module whose message-table resource(s) should be searched for the message.
        ///     When <c>0</c>, only the system message table is searched.
        /// </param>
        /// <returns>
        ///     The trimmed error message if one was found;
        ///     otherwise, a fallback string containing the hexadecimal error code.
        /// </returns>
        [Pure]
        public static string GetLastWin32ErrorMessage(nint moduleHandle = 0)
        {
            const int ERROR_INSUFFICIENT_BUFFER = 0x7A;

            int errorCode = Marshal.GetLastWin32Error();
            var flags = FormatMessageFlags.IgnoreInserts
                | FormatMessageFlags.FromSystem
                | FormatMessageFlags.ArgumentArray;

            if (moduleHandle != 0)
            {
                flags |= FormatMessageFlags.FromModuleHandle;
            }

            Span<char> buffer = stackalloc char[256];
            fixed (char* pBuffer = buffer)
            {
                int length = PInvoke.FormatMessage(
                    flags,
                    (nuint)moduleHandle,
                    unchecked((uint)errorCode),
                    0,
                    pBuffer,
                    (uint)buffer.Length,
                    null);

                if (length > 0)
                {
                    return getAndTrimString(buffer[..length]);
                }
            }

            if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
            {
                flags |= FormatMessageFlags.AllocateBuffer;

                nint nativeMsgPtr = default;
                try
                {
                    int length = PInvoke.FormatMessage(
                        flags,
                        (nuint)moduleHandle,
                        unchecked((uint)errorCode),
                        0,
                        (char*)&nativeMsgPtr,
                        0,
                        null);

                    if (length > 0)
                    {
                        return getAndTrimString(new((char*)nativeMsgPtr, length));
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(nativeMsgPtr);
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
}
