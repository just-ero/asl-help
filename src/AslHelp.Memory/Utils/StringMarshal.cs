using System;
using System.Text;

namespace AslHelp.Memory.Utils;

internal static class StringMarshal
{
    public static unsafe string CreateStringFromNullTerminated(char* chars, int length)
    {
        var span = new ReadOnlySpan<char>(chars, length);
        return CreateStringFromNullTerminated(span);
    }

    public static string CreateStringFromNullTerminated(ReadOnlySpan<char> chars)
    {
        int i = chars.IndexOf('\0');
        return i == -1
            ? chars.ToString()
            : chars[..i].ToString();
    }

    public static unsafe string CreateStringFromNullTerminated(byte* bytes, int length)
    {
        var span = new ReadOnlySpan<byte>(bytes, length);
        return CreateStringFromNullTerminated(span);
    }

    public static unsafe string CreateStringFromNullTerminated(ReadOnlySpan<byte> bytes)
    {
        int i = bytes.IndexOf((byte)'\0');
        fixed (byte* pBytes = bytes)
        {
            return i == -1
                ? Encoding.UTF8.GetString(pBytes, bytes.Length)
                : Encoding.UTF8.GetString(pBytes, i);
        }
    }
}
