namespace AslHelp.MemUtils.Reflect;

internal static class ReflectUtils
{
    public const int MIN_ALIGN = 1;

    public static int Align(int offset, int alignment)
    {
        if (alignment <= 0)
        {
            return 0;
        }

        int mod = offset % alignment;
        if (mod > 0)
        {
            offset += alignment - mod;
        }

        return offset;
    }

    public static bool IsNativeType(string typeName, out int size)
    {
        size = typeName switch
        {
            "byte" or "sbyte" or "bool" => 0x01,
            "ushort" or "short" or "char" => 0x02,
            "uint" or "int" or "float" => 0x04,
            "ulong" or "long" or "double" => 0x08,
            "decimal" => 0x10,
            _ => -1
        };

        return size != -1;
    }

    public static string RemoveGeneric(string name)
    {
        int i = name.IndexOf('<');

        if (i != -1)
        {
            name = name[..i];
        }

        return name;
    }
}
