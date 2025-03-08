namespace AslHelp.Tasks;

public interface IExceptionStage
{
    Type ExceptionType { get; }
    FailureBehavior FailureBehavior { get; }
    BuilderMessage<Exception> FailureMessage { get; }
    Action OnFailure { get; }
}

public interface IExceptionStage<TResult, TException>
    where TException : Exception
{
    // IExceptionStage<TResult, TException> When(Func<TException, bool> func);
    IExceptionStage<TResult, TException> WithFailureMessage(string message);
    IExceptionStage<TResult, TException> WithFailureMessage(Func<Exception, string> message);

    IExceptionStage<TResult, TException> DoOnFailure(Action action);

    ICatchStage<TResult> RetryOnFailure();
    ICatchStage<TResult> BreakOnFailure();
    ICatchStage<TResult> ThrowOnFailure();
}
