using System;

namespace AslHelp;

/// <summary>
///     The failure carried by a failed <see cref="IResult"/>.
/// </summary>
public interface IResultError
{
    /// <summary>
    ///     Gets the message describing the failure.
    /// </summary>
    string Message { get; }
}

/// <summary>
///     A failure described by a <see cref="Message"/>.
/// </summary>
/// <param name="Message">The message describing the failure.</param>
public record ResultError(
    string Message) : IResultError
{
    /// <summary>
    ///     Formats the error as <c>{TypeName}: {Message}</c>.
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name}: {Message}";
    }
}

/// <summary>
///     A <see cref="ResultError"/> that wraps a caught <see cref="System.Exception"/>.
/// </summary>
public sealed record ExceptionError : ResultError
{
    /// <summary>
    ///     Wraps <paramref name="exception"/>, taking its message as the error message.
    /// </summary>
    /// <param name="exception">The exception to wrap.</param>
    public ExceptionError(Exception exception)
        : base(exception?.Message ?? throw new ArgumentNullException(nameof(exception)))
    {
        Exception = exception;
    }

    /// <summary>
    ///     Wraps <paramref name="exception"/> under a custom <paramref name="message"/>.
    /// </summary>
    /// <param name="exception">The exception to wrap.</param>
    /// <param name="message">The message to use instead of the exception's own.</param>
    public ExceptionError(Exception exception, string message)
        : base(message)
    {
        Exception = exception;
    }

    /// <summary>
    ///     Gets the wrapped exception.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    ///     Wraps <paramref name="exception"/> as an <see cref="ExceptionError"/>.
    /// </summary>
    /// <param name="exception">The exception to wrap.</param>
    public static implicit operator ExceptionError(Exception exception)
    {
        return new(exception);
    }

    /// <summary>
    ///     Formats the error as <c>{ExceptionType}: {Message}</c>.
    /// </summary>
    public override string ToString()
    {
        return $"{Exception.GetType().Name}: {Message}";
    }
}
