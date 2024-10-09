using System;

namespace AslHelp.Shared.Tasks;

internal class ExceptionStage<TResult, TException> : IExceptionStage, IExceptionStage<TResult, TException>
    where TException : Exception
{
    private readonly ICatchStage<TResult> _builder;

    public ExceptionStage(ICatchStage<TResult> builder)
    {
        _builder = builder;
    }

    public Type ExceptionType { get; } = typeof(TException);
    public FailureBehavior FailureBehavior { get; protected set; }

    public Func<Exception, string>? FailureMessage { get; protected set; }

    IExceptionStage<TResult, TException> IExceptionStage<TResult, TException>.WithFailureMessage(string message)
    {
        FailureMessage = (_) => message;
        return this;
    }

    IExceptionStage<TResult, TException> IExceptionStage<TResult, TException>.WithFailureMessage(Func<TException, string> message)
    {
        FailureMessage = (Func<Exception, string>)message;
        return this;
    }

    ICatchStage<TResult> IExceptionStage<TResult, TException>.RetryOnFailure()
    {
        FailureBehavior = FailureBehavior.Retry;
        return _builder;
    }

    ICatchStage<TResult> IExceptionStage<TResult, TException>.BreakOnFailure()
    {
        FailureBehavior = FailureBehavior.Break;
        return _builder;
    }

    ICatchStage<TResult> IExceptionStage<TResult, TException>.ThrowOnFailure()
    {
        FailureBehavior = FailureBehavior.Throw;
        return _builder;
    }
}
