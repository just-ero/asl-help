using System.Buffers;

namespace AslHelp.Shared.Extensions;

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
