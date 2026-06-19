using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace AslHelp;

/// <summary>
///     The outcome of an operation that yields no value.
/// </summary>
public interface IResult
{
    /// <summary>
    ///     Gets whether the result is a success.
    /// </summary>
    bool IsOk { get; }

    /// <summary>
    ///     Gets whether the result is a failure.
    /// </summary>
    bool IsErr { get; }

    /// <summary>
    ///     Gets the failure, or <see langword="null"/> when the result is a success.
    /// </summary>
    IResultError? Error { get; }
}

/// <summary>
///     Represents the outcome of an operation that yields no value: either success
///     (<see cref="IsOk"/>) or failure carrying an <see cref="Error"/>.
/// </summary>
/// <remarks>
///     The presence of an <see cref="Error"/> is the single source of truth, so
///     <c>default(Result)</c> is <c>Ok</c>.
/// </remarks>
public readonly struct Result : IResult
{
    internal Result(IResultError? error)
    {
        Error = error;
    }

    /// <inheritdoc/>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsOk => Error is null;

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsErr => Error is not null;

    /// <inheritdoc/>
    public IResultError? Error { get; }

    /// <summary>
    ///     Wraps <paramref name="error"/> as a failed result.
    /// </summary>
    /// <param name="error">The failure to carry.</param>
    public static implicit operator Result(ResultError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }

    /// <summary>
    ///     Captures <paramref name="exception"/> as a failed result.
    /// </summary>
    /// <param name="exception">The exception to wrap.</param>
    public static implicit operator Result(Exception exception)
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
            return "Result.Ok()";
        }

        return $"Result.Err('{Error.Message}')";
    }
}

/// <summary>
///     Railway-oriented combinators and extractors for <see cref="Result"/> and
///     <see cref="Result{TValue}"/>.
/// </summary>
public static partial class ResultExtensions
{
    extension(Result self)
    {
        /// <summary>
        ///     Creates a successful result.
        /// </summary>
        [Pure]
        public static Result Ok()
        {
            return new(null);
        }

        /// <summary>
        ///     Creates a failed result carrying <paramref name="error"/>.
        /// </summary>
        /// <param name="error">The failure to carry.</param>
        [Pure]
        public static Result Err(IResultError error)
        {
            ArgumentNullException.ThrowIfNull(error);
            return new(error);
        }

        /// <summary>
        ///     Creates a failed result carrying a <see cref="ResultError"/> with <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The failure message.</param>
        [Pure]
        public static Result Err(string message)
        {
            ArgumentNullException.ThrowIfNull(message);
            return new(new ResultError(message));
        }

        /// <summary>
        ///     Creates a successful result holding <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The success value.</param>
        [Pure]
        public static Result<TValue> Ok<TValue>(TValue value)
        {
            return new(value);
        }

        /// <summary>
        ///     Creates a failed <see cref="Result{TValue}"/> carrying <paramref name="error"/>.
        /// </summary>
        /// <typeparam name="TValue">The success type the result would have held.</typeparam>
        /// <param name="error">The failure to carry.</param>
        [Pure]
        public static Result<TValue> Err<TValue>(IResultError error)
        {
            ArgumentNullException.ThrowIfNull(error);
            return new(error);
        }

        /// <summary>
        ///     Creates a failed <see cref="Result{TValue}"/> carrying a <see cref="ResultError"/> with
        ///     <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="TValue">The success type the result would have held.</typeparam>
        /// <param name="message">The failure message.</param>
        [Pure]
        public static Result<TValue> Err<TValue>(string message)
        {
            ArgumentNullException.ThrowIfNull(message);
            return new(new ResultError(message));
        }

        /// <summary>
        ///     Returns <paramref name="other"/> when <c>Ok</c>; otherwise propagates this error.
        /// </summary>
        /// <param name="other">The result to return on success.</param>
        [Pure]
        public Result And(Result other)
        {
            return self.IsOk
                ? other
                : self;
        }

        /// <summary>
        ///     Returns <paramref name="other"/> when <c>Ok</c>; otherwise propagates this error.
        /// </summary>
        /// <typeparam name="TValue">The success type of <paramref name="other"/>.</typeparam>
        /// <param name="other">The result to return on success.</param>
        [Pure]
        public Result<TValue> And<TValue>(Result<TValue> other)
        {
            return self.IsOk
                ? other
                : Result.Err<TValue>(self.Error);
        }

        /// <summary>
        ///     Invokes <paramref name="fn"/> when <c>Ok</c>; otherwise propagates this error without
        ///     calling it.
        /// </summary>
        /// <param name="fn">The continuation to run on success.</param>
        [Pure]
        public Result AndThen(Func<Result> fn)
        {
            if (!self.IsOk)
            {
                return self;
            }

            ArgumentNullException.ThrowIfNull(fn);
            return fn();
        }

        /// <summary>
        ///     Invokes <paramref name="fn"/> when <c>Ok</c>; otherwise propagates this error without
        ///     calling it.
        /// </summary>
        /// <typeparam name="TValue">The success type produced by <paramref name="fn"/>.</typeparam>
        /// <param name="fn">The continuation to run on success.</param>
        [Pure]
        public Result<TValue> AndThen<TValue>(Func<Result<TValue>> fn)
        {
            if (!self.IsOk)
            {
                return Result.Err<TValue>(self.Error);
            }

            ArgumentNullException.ThrowIfNull(fn);
            return fn();
        }

        /// <summary>
        ///     Lifts <paramref name="value"/> into a success when <c>Ok</c>; otherwise propagates this
        ///     error.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to lift on success.</param>
        [Pure]
        public Result<TValue> Map<TValue>(TValue value)
        {
            return self.IsOk
                ? Result.Ok(value)
                : Result.Err<TValue>(self.Error);
        }

        /// <summary>
        ///     Transforms the error with <paramref name="fn"/> when <c>Err</c>; otherwise leaves the
        ///     success unchanged.
        /// </summary>
        /// <typeparam name="TError">The replacement error type.</typeparam>
        /// <param name="fn">The error transform.</param>
        [Pure]
        public Result MapErr<TError>(Func<IResultError, TError> fn)
            where TError : IResultError
        {
            if (!self.IsErr)
            {
                return self;
            }

            ArgumentNullException.ThrowIfNull(fn);
            return Result.Err(fn(self.Error));
        }

        /// <summary>
        ///     Returns this result when <c>Ok</c>; otherwise returns <paramref name="res"/>.
        /// </summary>
        /// <param name="res">The fallback result.</param>
        [Pure]
        public Result Or(Result res)
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
        public Result OrElse(Func<IResultError, Result> fn)
        {
            if (self.IsOk)
            {
                return self;
            }

            ArgumentNullException.ThrowIfNull(fn);
            return fn(self.Error);
        }

        /// <summary>
        ///     Folds the result into a single value, invoking <paramref name="onOk"/> when successful
        ///     or <paramref name="onErr"/> with the error otherwise.
        /// </summary>
        /// <typeparam name="TOut">The folded result type.</typeparam>
        /// <param name="onOk">The success projection.</param>
        /// <param name="onErr">The error projection.</param>
        /// <returns>
        ///     The value produced by whichever branch ran.
        /// </returns>
        [Pure]
        public TOut Match<TOut>(Func<TOut> onOk, Func<IResultError, TOut> onErr)
        {
            if (self.IsOk)
            {
                ArgumentNullException.ThrowIfNull(onOk);
                return onOk();
            }

            ArgumentNullException.ThrowIfNull(onErr);
            return onErr(self.Error);
        }

        /// <summary>
        ///     Runs <paramref name="action"/> when <c>Ok</c> and returns the result unchanged, for
        ///     chaining side effects.
        /// </summary>
        /// <param name="action">The side effect to run on success.</param>
        public Result Inspect(Action action)
        {
            if (self.IsOk)
            {
                ArgumentNullException.ThrowIfNull(action);
                action();
            }

            return self;
        }

        /// <summary>
        ///     Runs <paramref name="action"/> with the error when <c>Err</c> and returns the result
        ///     unchanged, for chaining side effects.
        /// </summary>
        /// <param name="action">The side effect to run on failure.</param>
        public Result InspectErr(Action<IResultError> action)
        {
            if (self.IsErr)
            {
                ArgumentNullException.ThrowIfNull(action);
                action(self.Error);
            }

            return self;
        }

        /// <summary>
        ///     Asserts the result is <c>Ok</c>, throwing an <see cref="InvalidOperationException"/> if
        ///     it is <c>Err</c>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The result is <c>Err</c>.</exception>
        public void Unwrap()
        {
            if (!self.IsOk)
            {
                InvalidOperationException.Throw($"Called Unwrap on {self}.");
            }
        }

        /// <summary>
        ///     Asserts the result is <c>Ok</c>, throwing an <see cref="InvalidOperationException"/>
        ///     with <paramref name="message"/> if it is <c>Err</c>.
        /// </summary>
        /// <param name="message">The message to prefix the thrown exception with.</param>
        /// <exception cref="InvalidOperationException">The result is <c>Err</c>.</exception>
        public void Expect(string message)
        {
            if (!self.IsOk)
            {
                InvalidOperationException.Throw($"{message}: {self.Error}");
            }
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
