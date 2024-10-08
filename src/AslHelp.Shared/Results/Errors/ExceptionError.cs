using System;

namespace AslHelp.Shared.Results.Errors;

public sealed record ExceptionError : ResultError
{
    public ExceptionError(Exception exception)
        : base(exception.Message)
    {
        Exception = exception;
    }

    public ExceptionError(Exception exception, string message)
        : base(message)
    {
        Exception = exception;
    }

    public Exception Exception { get; }

    public static implicit operator ExceptionError(Exception exception)
    {
        return new(exception);
    }
}
