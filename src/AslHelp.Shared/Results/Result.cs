using System;
using System.Diagnostics.CodeAnalysis;

using AslHelp.Shared.Results.Errors;

namespace AslHelp.Shared.Results;

public readonly struct Result : IResult
{
    private Result(IResultError? error, IResult? innerResult)
    {
        Error = error;
        InnerResult = innerResult;

        IsOk = Error is null;
        IsErr = Error is not null;
    }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsOk { get; }

    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsErr { get; }

    public IResultError? Error { get; }
    public IResult? InnerResult { get; }

    // Construction
    public static Result Ok()
    {
        return new(default, default);
    }

    public static Result Err(IResultError error, IResult? innerResult = null)
    {
        return new(error, innerResult);
    }

    public static Result Combine(params IResult[] results)
    {
        foreach (IResult result in results)
        {
            if (result is { IsErr: true, Error: { } err })
            {
                return Err(err);
            }
        }

        return Ok();
    }

    // Operators
    public static implicit operator Result(ResultError error)
    {
        return Err(error);
    }

    public static implicit operator Result(Exception exception)
    {
        ExceptionError error = exception;
        return Err(error);
    }

    // Rust impl
    public Result And(Result res)
    {
        return IsOk
            ? res
            : this;
    }

    public Result<TValue> And<TValue>(Result<TValue> res)
    {
        return IsOk
            ? res
            : Result<TValue>.Err(Error);
    }

    public Result AndThen(Func<Result> op)
    {
        return IsOk
            ? op()
            : this;
    }

    public Result<TValue> AndThen<TValue>(Func<Result<TValue>> op)
    {
        return IsOk
            ? op()
            : Result<TValue>.Err(Error);
    }

    public Result<TValue> Map<TValue>(TValue value)
    {
        return IsOk
            ? Result<TValue>.Ok(value)
            : Result<TValue>.Err(Error);
    }

    public Result MapErr<TError>(Func<IResultError, TError> op)
        where TError : IResultError
    {
        return IsErr
            ? Err(op(Error), this)
            : this;
    }

    public TValue MapOrElse<TValue>(TValue @default, Func<IResultError, TValue> err)
    {
        return IsOk
            ? @default
            : err(Error);
    }

    public Result Or(Result res)
    {
        return IsOk
            ? this
            : res;
    }

    public Result OrElse(Func<IResultError, Result> op)
    {
        return IsOk
            ? this
            : op(Error);
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

    public bool TryUnwrapErr([NotNullWhen(true)] out IResultError? error)
    {
        error = Error;

        return IsErr;
    }

    public override string ToString()
    {
        if (IsOk)
        {
            return $"Result.Ok()";
        }
        else
        {
            if (InnerResult is not null)
            {
                return $"""
                    Result.Err({Error})
                      -> {InnerResult}
                    """;
            }
            else
            {
                return $"Result.Err({Error})";
            }
        }
    }
}
