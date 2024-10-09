using System;

namespace AslHelp.Shared.Tasks;

public interface IExceptionStage
{
    Type ExceptionType { get; }
    FailureBehavior FailureBehavior { get; }
    Func<Exception, string>? FailureMessage { get; }
}

public interface IExceptionStage<TResult, TException>
    where TException : Exception
{
    IExceptionStage<TResult, TException> WithFailureMessage(string message);
    IExceptionStage<TResult, TException> WithFailureMessage(Func<TException, string> message);

    ICatchStage<TResult> RetryOnFailure();
    ICatchStage<TResult> BreakOnFailure();
    ICatchStage<TResult> ThrowOnFailure();
}
