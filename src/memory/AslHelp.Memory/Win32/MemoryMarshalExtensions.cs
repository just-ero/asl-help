using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;

namespace AslHelp.Memory.Win32;

/// <summary>
///     Provides extension members for <see cref="MemoryMarshal"/>.
/// </summary>
internal static unsafe class MemoryMarshalExtensions
{
    extension(MemoryMarshal)
    {
        /// <summary>
        ///     Creates a <see cref="string"/> from a buffer of UTF-16 characters, stopping at the first null character.
        /// </summary>
        /// <param name="chars">
        ///     A pointer to the buffer of UTF-16 characters.
        /// </param>
        /// <param name="maxLength">
        ///     The maximum number of characters to read from the buffer.
        /// </param>
        /// <returns>
        ///     The decoded string, excluding the null terminator and everything that follows it.
        /// </returns>
        [Pure]
        public static string CreateStringFromNullTerminated(char* chars, int maxLength)
        {
            var span = new ReadOnlySpan<char>(chars, maxLength);
            return CreateStringFromNullTerminated(span);
        }

        /// <summary>
        ///     Creates a <see cref="string"/> from a span of UTF-16 characters, stopping at the first null character.
        /// </summary>
        /// <param name="chars">
        ///     The span of UTF-16 characters.
        /// </param>
        /// <returns>
        ///     The decoded string, excluding the null terminator and everything that follows it.
        /// </returns>
        [Pure]
        public static string CreateStringFromNullTerminated(ReadOnlySpan<char> chars)
        {
            int i = chars.IndexOf('\0');
            return i == -1
                ? chars.ToString()
                : chars[..i].ToString();
        }

        /// <summary>
        ///     Creates a <see cref="string"/> from a buffer of UTF-8 bytes, stopping at the first null byte.
        /// </summary>
        /// <param name="bytes">
        ///     A pointer to the buffer of UTF-8 bytes.
        /// </param>
        /// <param name="maxLength">
        ///     The maximum number of bytes to read from the buffer.
        /// </param>
        /// <returns>
        ///     The decoded string, excluding the null terminator and everything that follows it.
        /// </returns>
        [Pure]
        public static string CreateStringFromNullTerminated(byte* bytes, int maxLength)
        {
            var span = new ReadOnlySpan<byte>(bytes, maxLength);
            return CreateStringFromNullTerminated(span);
        }

        /// <summary>
        ///     Creates a <see cref="string"/> from a span of UTF-8 bytes, stopping at the first null byte.
        /// </summary>
        /// <param name="bytes">
        ///     The span of UTF-8 bytes.
        /// </param>
        /// <returns>
        ///     The decoded string, excluding the null terminator and everything that follows it.
        /// </returns>
        [Pure]
        public static string CreateStringFromNullTerminated(ReadOnlySpan<byte> bytes)
        {
            if (bytes.IsEmpty)
            {
                // An empty span pins to a null pointer, which Encoding.GetString rejects on .NET Core.
                return "";
            }

            var i = bytes.IndexOf((byte)'\0');
            fixed (byte* pBytes = bytes)
            {
                return i == -1
                    ? Encoding.UTF8.GetString(pBytes, bytes.Length)
                    : Encoding.UTF8.GetString(pBytes, i);
            }
        }
    }
}
