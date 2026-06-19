using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AslHelp;

/// <summary>
///     Polyfills the <see cref="ArgumentException"/> throw helper for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ArgumentExceptionExtensions
{
    extension(ArgumentException)
    {
        /// <summary>
        ///     Throws an <see cref="ArgumentException"/> with a specified error message
        ///     and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="ArgumentException"/>
        [DoesNotReturn]
        public static void Throw(string paramName, string message)
        {
            throw new ArgumentException(message, paramName);
        }
    }
}

/// <summary>
///     Polyfills the <see cref="ArgumentNullException"/> throw and null-check helpers for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ArgumentNullExceptionExtensions
{
    extension(ArgumentNullException)
    {
        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> with a specified error message
        ///     and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="ArgumentNullException"/>
        [DoesNotReturn]
        public static void Throw(string paramName, string message)
        {
            throw new ArgumentNullException(paramName, message);
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The argument to validate as non-<see langword="null"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        public static void ThrowIfNull(
            [NotNull] object? argument,
            string? message = null,
            [CallerArgumentExpression(nameof(argument))] string paramName = "")
        {
            if (argument is null)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, "Value cannot be null");
                }
                else
                {
                    Throw(paramName, message!);
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
        /// <param name="paramName">The name of the parameter with which <paramref name="collection"/> corresponds.</param>
        public static void ThrowIfNullOrEmpty<T>(
            [NotNull] IEnumerable<T>? collection,
            string? message = null,
            [CallerArgumentExpression(nameof(collection))] string paramName = "")
        {
            if (collection is null)
            {
                Throw(paramName, "Value cannot be null");
            }

            if (!collection.Any())
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    ArgumentException.Throw(paramName, "Collection cannot be empty");
                }
                else
                {
                    ArgumentException.Throw(paramName, message!);
                }
            }
        }
    }
}

/// <summary>
///     Polyfills the <see cref="ArgumentOutOfRangeException"/> throw and range-check helpers for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ArgumentOutOfRangeExceptionExtensions
{
    extension(ArgumentOutOfRangeException)
    {
        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> with a specified error message
        ///     and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [DoesNotReturn]
        public static void Throw(string paramName, string message)
        {
            throw new ArgumentOutOfRangeException(paramName, message);
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to be validated.</typeparam>
        /// <param name="value">The argument to validate as not equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfEqual<T>(
            T value,
            T other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (EqualityComparer<T>.Default.Equals(value, other))
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must be not be equal to {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is not equal to <paramref name="other"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to be validated.</typeparam>
        /// <param name="value">The argument to validate as equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfNotEqual<T>(
            T value,
            T other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(value, other))
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must be equal to {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to be validated.</typeparam>
        /// <param name="value">The argument to validate as not greater than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfGreaterThan<T>(
            T value,
            T other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
            where T : IComparable<T>
        {
            if (value.CompareTo(other) > 0)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must not be greater than {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal to <paramref name="other"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to be validated.</typeparam>
        /// <param name="value">The argument to validate as not greater than or equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfGreaterThanOrEqual<T>(
            T value,
            T other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
            where T : IComparable<T>
        {
            if (value.CompareTo(other) >= 0)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must not be greater than or equal to {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to be validated.</typeparam>
        /// <param name="value">The argument to validate as not less than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfLessThan<T>(
            T value,
            T other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
            where T : IComparable<T>
        {
            if (value.CompareTo(other) < 0)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must not be less than {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal to <paramref name="other"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to be validated.</typeparam>
        /// <param name="value">The argument to validate as greater than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfLessThanOrEqual<T>(
            T value,
            T other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
            where T : IComparable<T>
        {
            if (value.CompareTo(other) <= 0)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must be greater than {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as not equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfEqual(
            nint value,
            nint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value == other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must be not be equal to {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfNotEqual(
            nint value,
            nint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value != other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must be equal to {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as not greater than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfGreaterThan(
            nint value,
            nint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value > other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must not be greater than {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as not greater than or equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfGreaterThanOrEqual(
            nint value,
            nint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value >= other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must not be greater than or equal to {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as not less than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfLessThan(
            nint value,
            nint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value < other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must not be less than {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as greater than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfLessThanOrEqual(
            nint value,
            nint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value <= other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must be greater than {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as not equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfEqual(
            nuint value,
            nuint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value == other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must be not be equal to {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfNotEqual(
            nuint value,
            nuint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value != other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must be equal to {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as not greater than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfGreaterThan(
            nuint value,
            nuint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value > other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must not be greater than {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as not greater than or equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfGreaterThanOrEqual(
            nuint value,
            nuint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value >= other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must not be greater than or equal to {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as not less than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfLessThan(
            nuint value,
            nuint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value < other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must not be less than {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as greater than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="message">An optional message to include in the exception.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void ThrowIfLessThanOrEqual(
            nuint value,
            nuint other,
            string? message = null,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value <= other)
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Throw(paramName, $"'{paramName}' ({value}) must be greater than {other}");
                }
                else
                {
                    Throw(paramName, message!);
                }
            }
        }
    }
}

/// <summary>
///     Polyfills the <see cref="DllNotFoundException"/> throw helper for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DllNotFoundExceptionExtensions
{
    extension(DllNotFoundException)
    {
        /// <summary>
        ///     Throws an <see cref="DllNotFoundException"/> with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="DllNotFoundException"/>
        [DoesNotReturn]
        public static void Throw(string message)
        {
            throw new DllNotFoundException(message);
        }
    }
}

/// <summary>
///     Polyfills the <see cref="EndOfStreamException"/> throw helper for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class EndOfStreamExceptionExtensions
{
    extension(EndOfStreamException)
    {
        /// <summary>
        ///     Throws an <see cref="EndOfStreamException"/> with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="EndOfStreamException"/>
        [DoesNotReturn]
        public static void Throw(string message)
        {
            throw new EndOfStreamException(message);
        }
    }
}

/// <summary>
///     Polyfills the <see cref="FormatException"/> throw helper for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class FormatExceptionExtensions
{
    extension(FormatException)
    {
        /// <summary>
        ///     Throws a <see cref="FormatException"/> with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="FormatException"/>
        [DoesNotReturn]
        public static void Throw(string message)
        {
            throw new FormatException(message);
        }
    }
}

/// <summary>
///     Polyfills the <see cref="InvalidOperationException"/> throw helpers for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class InvalidOperationExceptionExtensions
{
    extension(InvalidOperationException)
    {
        /// <summary>
        ///     Throws a <see cref="InvalidOperationException"/> with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="InvalidOperationException"/>
        [DoesNotReturn]
        public static void Throw(string message)
        {
            throw new InvalidOperationException(message);
        }

        /// <summary>
        ///     Throws a <see cref="InvalidOperationException"/> with a specified error message.
        /// </summary>
        /// <typeparam name="T">The type of the value that would have been returned.</typeparam>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="InvalidOperationException"/>
        [DoesNotReturn]
        public static T Throw<T>(string message)
        {
            throw new InvalidOperationException(message);
        }
    }
}

/// <summary>
///     Polyfills the <see cref="KeyNotFoundException"/> throw helper for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class KeyNotFoundExceptionExtensions
{
    extension(KeyNotFoundException)
    {
        /// <summary>
        ///     Throws a <see cref="KeyNotFoundException"/> with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="KeyNotFoundException"/>
        [DoesNotReturn]
        public static void Throw(string message)
        {
            throw new KeyNotFoundException(message);
        }
    }
}

/// <summary>
///     Polyfills the <see cref="MissingFieldException"/> throw helper for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class MissingFieldExceptionExtensions
{
    extension(MissingFieldException)
    {
        /// <summary>
        ///     Throws a <see cref="MissingFieldException"/> for the field <paramref name="fieldName"/>
        ///     on <paramref name="className"/>.
        /// </summary>
        /// <param name="className">The name of the type declaring the field.</param>
        /// <param name="fieldName">The name of the missing field.</param>
        /// <exception cref="MissingFieldException"/>
        [DoesNotReturn]
        public static void Throw(string className, string fieldName)
        {
            throw new MissingFieldException(className, fieldName);
        }
    }
}

/// <summary>
///     Polyfills the <see cref="NotImplementedException"/> throw helpers for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class NotImplementedExceptionExtensions
{
    extension(NotImplementedException)
    {
        /// <summary>
        ///     Throws a <see cref="NotImplementedException"/> with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="NotImplementedException"/>
        [DoesNotReturn]
        public static void Throw(string message)
        {
            throw new NotImplementedException(message);
        }

        /// <summary>
        ///     Throws a <see cref="NotImplementedException"/> with a specified error message.
        /// </summary>
        /// <typeparam name="T">The type of the value that would have been returned.</typeparam>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="NotImplementedException"/>
        [DoesNotReturn]
        public static T Throw<T>(string message)
        {
            throw new NotImplementedException(message);
        }
    }
}

/// <summary>
///     Polyfills the <see cref="ObjectDisposedException"/> throw and disposed-check helpers for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ObjectDisposedExceptionExtensions
{
    extension(ObjectDisposedException)
    {
        /// <summary>
        ///     Throws an <see cref="ObjectDisposedException"/> for <paramref name="objectName"/> with a
        ///     specified error message.
        /// </summary>
        /// <param name="objectName">The name of the disposed object.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="ObjectDisposedException"/>
        [DoesNotReturn]
        public static void Throw(string objectName, string message)
        {
            throw new ObjectDisposedException(objectName, message);
        }

        /// <summary>
        ///     Throws an <see cref="ObjectDisposedException"/> for <typeparamref name="T"/> when
        ///     <paramref name="condition"/> is <see langword="true"/>.
        /// </summary>
        /// <typeparam name="T">The type of the possibly-disposed object.</typeparam>
        /// <param name="condition">When <see langword="true"/>, the exception is thrown.</param>
        /// <param name="obj">The instance whose type names the disposed object.</param>
        /// <param name="message">An optional message; a default is used when <see langword="null"/>.</param>
        /// <exception cref="ObjectDisposedException"><paramref name="condition"/> is <see langword="true"/>.</exception>
        public static void ThrowIf<T>(
            [DoesNotReturnIf(true)] bool condition,
            T? obj = default,
            string? message = null)
        {
            if (condition)
            {
                if (message is null)
                {
                    Throw(typeof(T).FullName, "Cannot access a disposed object.");
                }
                else
                {
                    Throw(typeof(T).FullName, message);
                }
            }
        }
    }
}

/// <summary>
///     Polyfills the <see cref="Win32Exception"/> throw helpers for netstandard2.0.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Win32ExceptionExtensions
{
    extension(Win32Exception)
    {
        /// <summary>
        ///     Throws a <see cref="Win32Exception"/> with the last Win32 error that occurred.
        /// </summary>
        /// <exception cref="Win32Exception"/>
        [DoesNotReturn]
        public static void Throw()
        {
            throw new Win32Exception();
        }

        /// <summary>
        ///     Throws a <see cref="Win32Exception"/> with a specified error.
        /// </summary>
        /// <param name="error">The Win32 error code that explains the reason for the exception.</param>
        /// <exception cref="Win32Exception"/>
        [DoesNotReturn]
        public static void Throw(int error)
        {
            throw new Win32Exception(error);
        }
    }
}
