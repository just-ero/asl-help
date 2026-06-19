using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace AslHelp;

/// <summary>
///     The outcome of an operation that yields a <typeparamref name="TValue"/> on success.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
public interface IResult<out TValue> : IResult
{
    /// <summary>
    ///     Gets the success value, or <see langword="default"/> when the result is a failure.
    /// </summary>
    TValue? Value { get; }
}

/// <summary>
///     Represents the outcome of an operation that yields a <typeparamref name="TValue"/> on
///     success (<see cref="IsOk"/>) or carries an <see cref="IResult.Error"/> on failure.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
/// <remarks>
///     The presence of an <see cref="IResult.Error"/> is the single source of truth, so
///     <c>default(Result&lt;TValue&gt;)</c> is an <c>Ok</c> holding <c>default(TValue)</c>.
/// </remarks>
public readonly struct Result<TValue> : IResult<TValue>
{
    internal Result(TValue value)
    {
        Value = value;
        Error = null;
    }

    internal Result(IResultError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        Value = default;
        Error = error;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsOk => Error is null;

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsErr => Error is not null;

    /// <inheritdoc/>
    public TValue? Value { get; }

    /// <inheritdoc/>
    public IResultError? Error { get; }

    /// <summary>
    ///     Wraps <paramref name="value"/> as a successful result.
    /// </summary>
    /// <param name="value">The success value.</param>
    public static implicit operator Result<TValue>(TValue value)
    {
        return new(value);
    }

    /// <summary>
    ///     Wraps <paramref name="error"/> as a failed result.
    /// </summary>
    /// <param name="error">The failure to carry.</param>
    public static implicit operator Result<TValue>(ResultError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }

    /// <summary>
    ///     Captures <paramref name="exception"/> as a failed result.
    /// </summary>
    /// <param name="exception">The exception to wrap.</param>
    public static implicit operator Result<TValue>(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        ExceptionError error = exception;
        return new(error);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (IsOk)
        {
            return $"Result<{typeof(TValue).Name}>.Ok({Value})";
        }

        return $"Result<{typeof(TValue).Name}>.Err('{Error.Message}')";
    }
}

public static partial class ResultExtensions
{
    extension<TValue>(Result<TValue> self)
    {
        /// <summary>
        ///     Returns <paramref name="res"/> when <c>Ok</c>; otherwise propagates this error.
        /// </summary>
        /// <param name="res">The result to return on success.</param>
        [Pure]
        public Result And(Result res)
        {
            return self.IsOk
                ? res
                : Result.Err(self.Error);
        }

        /// <summary>
        ///     Returns <paramref name="res"/> when <c>Ok</c>; otherwise propagates this error.
        /// </summary>
        /// <typeparam name="TOther">The success type of <paramref name="res"/>.</typeparam>
        /// <param name="res">The result to return on success.</param>
        [Pure]
        public Result<TOther> And<TOther>(Result<TOther> res)
        {
            return self.IsOk
                ? res
                : Result.Err<TOther>(self.Error);
        }

        /// <summary>
        ///     Binds the value through <paramref name="fn"/> when <c>Ok</c>; otherwise propagates this
        ///     error without calling it.
        /// </summary>
        /// <param name="fn">The continuation applied to the value on success.</param>
        [Pure]
        public Result AndThen(Func<TValue, Result> fn)
        {
            if (!self.IsOk)
            {
                return Result.Err(self.Error);
            }

            ArgumentNullException.ThrowIfNull(fn);
            return fn(self.Value);
        }

        /// <summary>
        ///     Binds the value through <paramref name="fn"/> when <c>Ok</c>; otherwise propagates this
        ///     error without calling it.
        /// </summary>
        /// <typeparam name="TOther">The success type produced by <paramref name="fn"/>.</typeparam>
        /// <param name="fn">The continuation applied to the value on success.</param>
        [Pure]
        public Result<TOther> AndThen<TOther>(Func<TValue, Result<TOther>> fn)
        {
            if (!self.IsOk)
            {
                return Result.Err<TOther>(self.Error);
            }

            ArgumentNullException.ThrowIfNull(fn);
            return fn(self.Value);
        }

        /// <summary>
        ///     Maps the value through <paramref name="fn"/> when <c>Ok</c>; otherwise propagates this
        ///     error.
        /// </summary>
        /// <typeparam name="TOther">The mapped value type.</typeparam>
        /// <param name="fn">The value transform.</param>
        [Pure]
        public Result<TOther> Map<TOther>(Func<TValue, TOther> fn)
        {
            if (!self.IsOk)
            {
                return Result.Err<TOther>(self.Error);
            }

            ArgumentNullException.ThrowIfNull(fn);
            return Result.Ok(fn(self.Value));
        }

        /// <summary>
        ///     Transforms the error with <paramref name="fn"/> when <c>Err</c>; otherwise leaves the
        ///     value unchanged.
        /// </summary>
        /// <typeparam name="TOtherErr">The replacement error type.</typeparam>
        /// <param name="fn">The error transform.</param>
        [Pure]
        public Result<TValue> MapErr<TOtherErr>(Func<IResultError, TOtherErr> fn)
            where TOtherErr : IResultError
        {
            if (!self.IsErr)
            {
                return self;
            }

            ArgumentNullException.ThrowIfNull(fn);
            return Result.Err<TValue>(fn(self.Error));
        }

        /// <summary>
        ///     Maps the value through <paramref name="fn"/> when <c>Ok</c>; otherwise returns
        ///     <paramref name="default"/>.
        /// </summary>
        /// <typeparam name="TOther">The mapped value type.</typeparam>
        /// <param name="default">The fallback used on failure.</param>
        /// <param name="fn">The value transform.</param>
        /// <returns>
        ///     The mapped value or the fallback.
        /// </returns>
        [Pure]
        public TOther MapOr<TOther>(TOther @default, Func<TValue, TOther> fn)
        {
            if (!self.IsOk)
            {
                return @default;
            }

            ArgumentNullException.ThrowIfNull(fn);
            return fn(self.Value);
        }

        /// <summary>
        ///     Maps the value with <paramref name="fn"/> when <c>Ok</c>; otherwise computes a fallback
        ///     from the error with <paramref name="fallback"/>.
        /// </summary>
        /// <remarks>
        ///     The success mapper comes first, the reverse of Rust's <c>map_or_else</c>.
        /// </remarks>
        /// <typeparam name="TOther">The folded value type.</typeparam>
        /// <param name="fn">The value transform applied on success.</param>
        /// <param name="fallback">The fallback computed from the error.</param>
        /// <returns>
        ///     The mapped value or the computed fallback.
        /// </returns>
        [Pure]
        public TOther MapOrElse<TOther>(Func<TValue, TOther> fn, Func<IResultError, TOther> fallback)
        {
            if (self.IsOk)
            {
                ArgumentNullException.ThrowIfNull(fn);
                return fn(self.Value);
            }

            ArgumentNullException.ThrowIfNull(fallback);
            return fallback(self.Error);
        }

        /// <summary>
        ///     Folds the result into a single value, invoking <paramref name="onOk"/> with the value
        ///     when successful or <paramref name="onErr"/> with the error otherwise.
        /// </summary>
        /// <typeparam name="TOut">The folded result type.</typeparam>
        /// <param name="onOk">The success projection.</param>
        /// <param name="onErr">The error projection.</param>
        /// <returns>
        ///     The value produced by whichever branch ran.
        /// </returns>
        [Pure]
        public TOut Match<TOut>(Func<TValue, TOut> onOk, Func<IResultError, TOut> onErr)
        {
            if (self.IsOk)
            {
                ArgumentNullException.ThrowIfNull(onOk);
                return onOk(self.Value);
            }

            ArgumentNullException.ThrowIfNull(onErr);
            return onErr(self.Error);
        }

        /// <summary>
        ///     Returns this result when <c>Ok</c>; otherwise returns <paramref name="res"/>.
        /// </summary>
        /// <param name="res">The fallback result.</param>
        [Pure]
        public Result<TValue> Or(Result<TValue> res)
        {
            return self.IsOk
                ? self
                : res;
        }

        /// <summary>
        ///     Returns this result when <c>Ok</c>; otherwise recovers via <paramref name="fn"/>.
        /// </summary>
        /// <param name="fn">The recovery from the error.</param>
        [Pure]
        public Result<TValue> OrElse(Func<IResultError, Result<TValue>> fn)
        {
            if (self.IsOk)
            {
                return self;
            }

            ArgumentNullException.ThrowIfNull(fn);
            return fn(self.Error);
        }

        /// <summary>
        ///     Runs <paramref name="action"/> with the value when <c>Ok</c> and returns the result
        ///     unchanged, for chaining side effects.
        /// </summary>
        /// <param name="action">The side effect to run on success.</param>
        public Result<TValue> Inspect(Action<TValue> action)
        {
            if (self.IsOk)
            {
                ArgumentNullException.ThrowIfNull(action);
                action(self.Value);
            }

            return self;
        }

        /// <summary>
        ///     Runs <paramref name="action"/> with the error when <c>Err</c> and returns the result
        ///     unchanged, for chaining side effects.
        /// </summary>
        /// <param name="action">The side effect to run on failure.</param>
        public Result<TValue> InspectErr(Action<IResultError> action)
        {
            if (self.IsErr)
            {
                ArgumentNullException.ThrowIfNull(action);
                action(self.Error);
            }

            return self;
        }

        /// <summary>
        ///     Returns the value when <c>Ok</c>; otherwise throws an
        ///     <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <returns>
        ///     The value.
        /// </returns>
        /// <exception cref="InvalidOperationException">The result is <c>Err</c>.</exception>
        public TValue Unwrap()
        {
            if (!self.IsOk)
            {
                return InvalidOperationException.Throw<TValue>($"Cannot unwrap value on {self}.");
            }

            return self.Value;
        }

        /// <summary>
        ///     Returns the value when <c>Ok</c>; otherwise throws an
        ///     <see cref="InvalidOperationException"/> with <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to prefix the thrown exception with.</param>
        /// <returns>
        ///     The value.
        /// </returns>
        /// <exception cref="InvalidOperationException">The result is <c>Err</c>.</exception>
        public TValue Expect(string message)
        {
            if (!self.IsOk)
            {
                return InvalidOperationException.Throw<TValue>($"{message}: {self.Error}");
            }

            return self.Value;
        }

        /// <summary>
        ///     Returns the value when <c>Ok</c>; otherwise returns <paramref name="default"/>.
        /// </summary>
        /// <param name="default">The fallback used on failure.</param>
        /// <returns>
        ///     The value or the fallback.
        /// </returns>
        public TValue UnwrapOr(TValue @default)
        {
            return self.IsOk
                ? self.Value
                : @default;
        }

        /// <summary>
        ///     Returns the value when <c>Ok</c>; otherwise returns <see langword="default"/>.
        /// </summary>
        /// <returns>
        ///     The value or <see langword="default"/>.
        /// </returns>
        public TValue? UnwrapOrDefault()
        {
            return self.IsOk
                ? self.Value
                : default;
        }

        /// <summary>
        ///     Returns the value when <c>Ok</c>; otherwise computes one from the error via
        ///     <paramref name="fn"/>.
        /// </summary>
        /// <param name="fn">The fallback computed from the error.</param>
        /// <returns>
        ///     The value or the computed fallback.
        /// </returns>
        public TValue UnwrapOrElse(Func<IResultError, TValue> fn)
        {
            if (self.IsOk)
            {
                return self.Value;
            }

            ArgumentNullException.ThrowIfNull(fn);
            return fn(self.Error);
        }

        /// <summary>
        ///     Returns the value and <see langword="true"/> when <c>Ok</c>; otherwise
        ///     <see langword="false"/>.
        /// </summary>
        /// <param name="value">The value when <c>Ok</c>; otherwise <see langword="default"/>.</param>
        /// <returns>
        ///     <see langword="true"/> when <c>Ok</c>.
        /// </returns>
        public bool TryUnwrap([MaybeNullWhen(false)] out TValue value)
        {
            value = self.Value;
            return self.IsOk;
        }

        /// <summary>
        ///     Returns the value and <see langword="true"/> when <c>Ok</c>; otherwise the
        ///     <paramref name="error"/> and <see langword="false"/>.
        /// </summary>
        /// <param name="value">The value when <c>Ok</c>; otherwise <see langword="default"/>.</param>
        /// <param name="error">The error when <c>Err</c>; otherwise <see langword="null"/>.</param>
        /// <returns>
        ///     <see langword="true"/> when <c>Ok</c>.
        /// </returns>
        public bool TryUnwrap([MaybeNullWhen(false)] out TValue value, [NotNullWhen(false)] out IResultError? error)
        {
            value = self.Value;
            error = self.Error;
            return self.IsOk;
        }

        /// <summary>
        ///     Returns the error when <c>Err</c>; otherwise throws an
        ///     <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <returns>
        ///     The error.
        /// </returns>
        /// <exception cref="InvalidOperationException">The result is <c>Ok</c>.</exception>
        public IResultError UnwrapErr()
        {
            if (!self.IsErr)
            {
                return InvalidOperationException.Throw<IResultError>($"Cannot unwrap error on {self}.");
            }

            return self.Error;
        }

        /// <summary>
        ///     Returns the error when <c>Err</c>; otherwise throws an
        ///     <see cref="InvalidOperationException"/> with <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to prefix the thrown exception with.</param>
        /// <returns>
        ///     The error.
        /// </returns>
        /// <exception cref="InvalidOperationException">The result is <c>Ok</c>.</exception>
        public IResultError ExpectErr(string message)
        {
            if (!self.IsErr)
            {
                return InvalidOperationException.Throw<IResultError>($"{message}: {self}");
            }

            return self.Error;
        }

        /// <summary>
        ///     Returns the error and <see langword="true"/> when <c>Err</c>; otherwise
        ///     <see langword="false"/>.
        /// </summary>
        /// <param name="error">The error when <c>Err</c>; otherwise <see langword="null"/>.</param>
        /// <returns>
        ///     <see langword="true"/> when <c>Err</c>.
        /// </returns>
        public bool TryUnwrapErr([NotNullWhen(true)] out IResultError? error)
        {
            error = self.Error;
            return self.IsErr;
        }
    }
}
