using System.Buffers;

namespace AslHelp.Collections.Extensions;

public static class ArrayPoolExtensions
{
    public static void ReturnIfNotNull<T>(this ArrayPool<T> pool, T[]? array)
    {
        if (array is not null)
        {
            pool.Return(array);
        }
    }
}
