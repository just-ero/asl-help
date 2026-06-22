using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace AslHelp;

/// <summary>
///     Provides extension members for <see cref="ArrayPool{T}"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ArrayPoolExtensions
{
    extension<T>(ArrayPool<T> self)
    {
        /// <summary>
        ///     Returns <paramref name="array"/> to the pool, treating <see langword="null"/> as a no-op.
        /// </summary>
        /// <param name="array">The buffer to return, or <see langword="null"/> to do nothing.</param>
        /// <param name="clearArray">Whether to clear the buffer's contents before returning it.</param>
        public void ReturnIfNotNull(T[]? array, bool clearArray = false)
        {
            if (array is not null)
            {
                self.Return(array, clearArray);
            }
        }
    }
}
