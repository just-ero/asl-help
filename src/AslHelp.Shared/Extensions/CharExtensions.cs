namespace AslHelp.Shared.Extensions;

/// <summary>
///     The <see cref="CharExtensions"/> class
///     provides useful extension methods for the <see cref="char"/> type.
/// </summary>
public static class CharExtensions
{
    public const byte InvalidHexValue = 0xFF;

    /// <summary>
    ///     Converts a hexadecimal character to its decimal value.
    /// </summary>
    /// <param name="c">The hexadecimal character to convert.</param>
    /// <returns>
    ///     The decimal value of <paramref name="c"/> if it is a valid hexadecimal character;
    ///     otherwise, <c>0xFF</c>.
    /// </returns>
    public static byte ToHexValue(this char c)
    {
        int digit = c - '0';

        if (digit is >= 0 and < 10)
        {
            return (byte)digit;
        }

        int letter = (c | 0x20) - 'a';

        if (letter is >= 0 and < 6)
        {
            return (byte)(letter + 10);
        }

        return InvalidHexValue;
    }
}
