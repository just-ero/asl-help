using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AslHelp.Shared;

/// <summary>
///     The <see cref="ThrowHelper"/> class
///     provides helper methods for throwing exceptions.
/// </summary>
public static partial class ThrowHelper
{
    /// <summary>
    ///     Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="argument">The argument to validate as non-<see langword="null"/>.</param>
    /// <param name="message">An optional message to include in the exception.</param>
    /// <param name="paramName">
    ///     The name of the parameter with which <paramref name="argument"/> corresponds.
    ///     If this parameter is omitted, the name of <paramref name="argument"/> is used.
    /// </param>
    public static void ThrowIfNull(
        [NotNull] object? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string paramName = "")
    {
        if (argument is null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                ThrowArgumentNullException(paramName, "Value cannot be null.");
            }
            else
            {
                ThrowArgumentNullException(paramName, message!);
            }
        }
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentNullException"/> if <paramref name="collection"/> is <see langword="null"/>,
    ///     or an <see cref="ArgumentException"/> if <paramref name="collection"/> is empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to validate as non-<see langword="null"/> and non-empty.</param>
    /// <param name="message">An optional message to include in the exception.</param>
    /// <param name="paramName">
    ///     The name of the parameter with which <paramref name="collection"/> corresponds.
    ///     If this parameter is omitted, the name of <paramref name="collection"/> is used.
    /// </param>
    public static void ThrowIfNullOrEmpty<T>(
        [NotNull] IEnumerable<T>? collection,
        string? message = null,
        [CallerArgumentExpression(nameof(collection))] string paramName = "")
    {
        if (collection is null)
        {
            ThrowArgumentNullException(paramName, "Value cannot be null.");
        }

        if (!collection.Any())
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                ThrowArgumentException(paramName, "Collection cannot be empty.");
            }
            else
            {
                ThrowArgumentException(paramName, message!);
            }
        }
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="min"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to be validated.</typeparam>
    /// <param name="value">The value to be validated.</param>
    /// <param name="min">The minimum value <paramref name="value"/> can be.</param>
    /// <param name="message">An optional message to include in the exception.</param>
    /// <param name="paramName">
    ///     The name of the parameter with which <paramref name="value"/> corresponds.
    ///     If this parameter is omitted, the name of <paramref name="value"/> is used.
    /// </param>
    public static void ThrowIfLessThan<T>(
        T value,
        T min,
        string? message = null,
        [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : unmanaged, IComparable<T>
    {
        if (value.CompareTo(min) < 0)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                ThrowArgumentOutOfRangeException(paramName, $"'{paramName}' must be >= {min}.");
            }
            else
            {
                ThrowArgumentOutOfRangeException(paramName, message!);
            }
        }
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is larger than <paramref name="max"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to be validated.</typeparam>
    /// <param name="value">The value to be validated.</param>
    /// <param name="max">The maximum value <paramref name="value"/> can be.</param>
    /// <param name="message">An optional message to include in the exception.</param>
    /// <param name="paramName">
    ///     The name of the parameter with which <paramref name="value"/> corresponds.
    ///     If this parameter is omitted, the name of <paramref name="value"/> is used.
    /// </param>
    public static void ThrowIfLargerThan<T>(
        T value,
        T max,
        string? message = null,
        [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : unmanaged, IComparable<T>
    {
        if (value.CompareTo(max) > 0)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                ThrowArgumentOutOfRangeException(paramName, $"'{paramName}' must be <= {max}.");
            }
            else
            {
                ThrowArgumentOutOfRangeException(paramName, message!);
            }
        }
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is
    ///     not in the range of <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to be validated.</typeparam>
    /// <param name="value">The value to be validated.</param>
    /// <param name="min">The minimum value <paramref name="value"/> can be.</param>
    /// <param name="max">The maximum value <paramref name="value"/> can be.</param>
    /// <param name="message">An optional message to include in the exception.</param>
    /// <param name="paramName">
    ///     The name of the parameter with which <paramref name="value"/> corresponds.
    ///     If this parameter is omitted, the name of <paramref name="value"/> is used.
    /// </param>
    public static void ThrowIfNotInRange<T>(
        T value,
        T min,
        T max,
        string? message = null,
        [CallerArgumentExpression(nameof(value))] string paramName = "")
        where T : unmanaged, IComparable<T>
    {
        ThrowIfLessThan(value, min, message, paramName);
        ThrowIfLargerThan(value, max, message, paramName);
    }
}
