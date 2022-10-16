using System.Globalization;
using System.Runtime.InteropServices;

namespace AslHelp.Extensions;

internal static class StringExt
{
    public static unsafe string RemoveWhiteSpace(this string value)
    {
        if (value is null)
        {
            return null;
        }

        char* output = stackalloc char[value.Length];

        fixed (char* pValue = value)
        {
            for (int i = 0, j = 0; i < value.Length; i++)
            {
                char c = pValue[i];

                if (!char.IsWhiteSpace(c))
                {
                    output[j++] = c;
                }
            }

            return new(output);
        }
    }

    public static unsafe string Concat(this string[] source)
    {
        source.ThrowIfNullOrEmpty();

        int offset = 0;
        char[] buffer = new char[512];

        for (int i = 0; i < source.Length; i++)
        {
            string current = source[i];
            int length = current.Length;
            int next = offset + length;

            if (next > 512)
            {
                string msg = "Input may not be longer than 512 characters.";
                throw new ArgumentException(msg);
            }

            current.CopyTo(0, buffer, offset, current.Length);

            offset = next;
        }

        fixed (char* pBuffer = buffer)
        {
            return new(pBuffer);
        }
    }

    public static unsafe string ToValidIdentifier(this string value)
    {
        if (value is null)
        {
            return "";
        }

        if (value.Length is 0 or > 256)
        {
            return "";
        }

        Span<char> outBuf = stackalloc char[256];
        char* pNorm = stackalloc char[256];

        fixed (char* pValue = value, pOut = outBuf)
        {
            int dLength = NormalizeString(2, (ushort*)pValue, value.Length, (ushort*)pNorm, 256);

            int start = 255, length = 0;
            char first = default;

            for (int i = dLength - 1; i >= 0; i--)
            {
                char c = pNorm[i];

                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                start--;

                if (c is '/')
                {
                    start++;
                    break;
                }

                pOut[start] = first = c switch
                {
                    (>= '0' and <= '9') or (>= 'A' and <= 'Z') or (>= 'a' and <= 'z') or '.' => c,
                    _ => '_'
                };

                length++;
            }

            if (first is >= '0' and <= '9')
            {
                pOut[--start] = '_';
                length++;
            }

            return outBuf.Slice(start, length).ToString();
        }

        [DllImport("normaliz")]
        static extern int NormalizeString(
            int normForm,
            ushort* source,
            int sourceLength,
            ushort* destination,
            int destinationLength);
    }

    public static unsafe string ToValidIdentifierUnity(this string value)
    {
        int genericLeft = value.IndexOf('<');
        if (genericLeft != -1)
        {
            int genericRight = value.IndexOf('>');
            if (genericRight > genericLeft + 1)
            {
                value = value[(genericLeft + 1)..genericRight];
            }
        }

        return ToValidIdentifier(value);
    }

    public static void ThrowIfNullOrEmpty(this string source, string message = null)
    {
        if (source is null || source.Length == 0)
        {
            message ??= "String was null or empty.";
            throw new ArgumentException(message);
        }
    }
}
