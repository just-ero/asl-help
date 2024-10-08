using System;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Shared.Results.Errors;

namespace AslHelp.Shared.Results;

public readonly struct Result<TValue> : IResult<TValue>
{
    private Result(TValue? value, IResultError? error, IResult? innerResult)
    {
        Value = value;
        Error = error;
        InnerResult = innerResult;

        IsOk = Error is null;
        IsErr = Error is not null;
    }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsOk { get; }

    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsErr { get; }

    public TValue? Value { get; }

    public IResultError? Error { get; }
    public IResult? InnerResult { get; }

    // Construction
    public static Result<TValue> Ok(TValue value)
    {
        return new(value, default, default);
    }

    public static Result<TValue> Err(IResultError error, IResult? innerResult = null)
    {
        return new(default, error, innerResult);
    }

    // Operators
    public static implicit operator Result<TValue>(TValue value)
    {
        return Ok(value);
    }

    public static implicit operator Result<TValue>(ResultError error)
    {
        return Err(error);
    }

    public static implicit operator Result<TValue>(Exception exception)
    {
        ExceptionError error = exception;
        return Err(error);
    }

    // Rust impl
    public Result And(Result res)
    {
        return IsOk
            ? res
            : Result.Err(Error);
    }

    public Result<TOther> And<TOther>(Result<TOther> res)
    {
        return IsOk
            ? res
            : Result<TOther>.Err(Error);
    }

    public Result AndThen(Func<TValue, Result> op)
    {
        return IsOk
            ? op(Value)
            : Result.Err(Error);
    }

    public Result<TOther> AndThen<TOther>(Func<TValue, Result<TOther>> op)
    {
        return IsOk
            ? op(Value)
            : Result<TOther>.Err(Error);
    }

    public Result<TOther> Map<TOther>(Func<TValue, TOther> op)
    {
        return IsOk
            ? Result<TOther>.Ok(op(Value))
            : Result<TOther>.Err(Error);
    }

    public Result<TValue> MapErr<TOtherErr>(Func<IResultError, TOtherErr> op)
        where TOtherErr : IResultError
    {
        return IsErr
            ? Result<TValue>.Err(op(Error), this)
            : this;
    }

    public TOther MapOr<TOther>(TOther @default, Func<TValue, TOther> op)
    {
        return IsOk
            ? op(Value)
            : @default;
    }

    public TOther MapOrElse<TOther>(Func<TValue, TOther> op, Func<IResultError, TOther> @default)
    {
        return IsOk
            ? op(Value)
            : @default(Error);
    }

    public Result<TValue> Or(Result<TValue> res)
    {
        return IsOk
            ? this
            : res;
    }

    public Result<TValue> OrElse(Func<IResultError, Result<TValue>> op)
    {
        return IsOk
            ? this
            : op(Error);
    }

    public TValue Unwrap()
    {
        if (!IsOk)
        {
            string msg = $"Attempted to unwrap Ok: {this}";
            ThrowHelper.ThrowInvalidOperationException(msg);
        }

        return Value;
    }

    public bool TryUnwrap([NotNullWhen(true)] out TValue? value, [NotNullWhen(false)] out IResultError? error)
    {
        value = Value;
        error = Error;

        return IsOk;
    }

    public IResultError UnwrapErr()
    {
        if (!IsErr)
        {
            string msg = $"Attempted to unwrap Err: {this}";
            ThrowHelper.ThrowInvalidOperationException(msg);
        }

        return Error;
    }

    public bool TryUnwrapErr([NotNullWhen(false)] out TValue? value, [NotNullWhen(true)] out IResultError? error)
    {
        value = Value;
        error = Error;

        return IsErr;
    }

    public TValue UnwrapOr(TValue @default)
    {
        return IsOk
            ? Value
            : @default;
    }

    public TValue? UnwrapOrDefault()
    {
        return IsOk
            ? Value
            : default;
    }

    public TValue UnwrapOrElse(Func<IResultError, TValue> op)
    {
        return IsOk
            ? Value
            : op(Error);
    }

    public override string ToString()
    {
        if (IsOk)
        {
            return $"Result<{typeof(TValue).FullName}>.Ok({Value})";
        }
        else
        {
            if (InnerResult is not null)
            {
                return $"""
                    Result<{typeof(TValue).FullName}>.Err({Error})
                      -> {InnerResult}
                    """;
            }
            else
            {
                return $"Result<{typeof(TValue).FullName}>.Err({Error})";
            }
        }
    }
}
