namespace AslHelp.Extensions;

internal static class EnumerableExt
{
    public static void ThrowIfNullOrEmpty<T>(this T[] source, string message = null)
    {
        if (source is null || source.Length == 0)
        {
            message ??= "Array was null or empty.";
            throw new ArgumentException(message);
        }
    }

    public static void ThrowIfNullOrEmpty<T>(this List<T> source, string message = null)
    {
        if (source is null || source.Count == 0)
        {
            message ??= "List was null or empty.";
            throw new ArgumentException(message);
        }
    }

    public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> source, string message = null)
    {
        if (source is null || !source.Any())
        {
            message ??= "Collection was null or empty.";
            throw new ArgumentException(message);
        }
    }
}
