namespace AslHelp.Tasks;

internal class ExceptionStage<TResult, TException> :
    IExceptionStage<TResult, TException>,
    IExceptionStage
    where TException : Exception
{
    private readonly ICatchStage<TResult> _builder;

    public ExceptionStage(ICatchStage<TResult> builder)
    {
        _builder = builder;
    }

    public Type ExceptionType { get; } = typeof(TException);
    public FailureBehavior FailureBehavior { get; protected set; }

    public BuilderMessage<Exception> FailureMessage { get; protected set; }

    IExceptionStage<TResult, TException> IExceptionStage<TResult, TException>.WithFailureMessage(string message)
    {
        if (FailureMessage is not null)
        {
            string msg = "Failure message was already set.";
            throw new InvalidOperationException(msg);
        }

        FailureMessage = new(message);
        return this;
    }

    IExceptionStage<TResult, TException> IExceptionStage<TResult, TException>.WithFailureMessage(Func<TException, string> message)
    {
        if (FailureMessage is not null)
        {
            string msg = "Failure message was already set.";
            throw new InvalidOperationException(msg);
        }

        FailureMessage = new((Func<Exception, string>)message);
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
