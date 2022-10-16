namespace AslHelp.Extensions;

internal static class CharExt
{
    public static bool IsHexDigit(this char value, out byte result)
    {
        result = value switch
        {
            '0' => 0x0,
            '1' => 0x1,
            '2' => 0x2,
            '3' => 0x3,
            '4' => 0x4,
            '5' => 0x5,
            '6' => 0x6,
            '7' => 0x7,
            '8' => 0x8,
            '9' => 0x9,
            'a' or 'A' => 0xA,
            'b' or 'B' => 0xB,
            'c' or 'C' => 0xC,
            'd' or 'D' => 0xD,
            'e' or 'E' => 0xE,
            'f' or 'F' => 0xF,
            _ => 0xFF
        };

        return result != 0xFF;
    }
}
