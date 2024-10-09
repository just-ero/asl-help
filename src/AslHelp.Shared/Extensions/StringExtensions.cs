using System;
using System.Buffers;

namespace AslHelp.Shared.Extensions;

/// <summary>
///     Provides useful extension methods for the <see cref="string"/> type.
/// </summary>
public static class StringExtensions
{
    public static unsafe string RemoveWhiteSpace(this string value)
    {
        int length = value.Length;

        char[]? rented = null;
        Span<char> buffer =
            length <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(length));

        fixed (char* pValue = value, pBuffer = buffer)
        {
            int offset = 0;
            for (int i = 0; i < length; i++)
            {
                char c = pValue[i];

                if (!char.IsWhiteSpace(c))
                {
                    pBuffer[offset++] = c;
                }
            }

            string result = buffer[..offset].ToString();

            ArrayPool<char>.Shared.ReturnIfNotNull(rented);

            return result;
        }
    }

    /// <summary>
    ///     Provides an optimized method to concatenate the elements of a specified <see cref="string"/> array.
    /// </summary>
    /// <param name="values">The collection of strings to concatenate.</param>
    /// <returns>
    ///     The concatenated elements of <paramref name="values"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="values"/> is <see langword="null"/>.
    /// </exception>
    public static string Concat(this string[] values)
    {
        if (values.Length == 0)
        {
            return "";
        }

        int length = 0;
        foreach (string s in values)
        {
            length += s.Length;
        }

        ReadOnlySpan<string> strings = values;

        char[]? rented = null;
        Span<char> buffer =
            length <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(length));

        for (int i = 0, offset = 0; i < strings.Length; i++)
        {
            strings[i].AsSpan().CopyTo(buffer[offset..]);
            offset += strings[i].Length;
        }

        string result = buffer[..length].ToString();

        ArrayPool<char>.Shared.ReturnIfNotNull(rented);

        return result;
    }
}
