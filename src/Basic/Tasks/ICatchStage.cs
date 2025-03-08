namespace AslHelp.Tasks;

public interface ICatchStage<TResult>
{
    IExceptionStage<TResult, TException> Catch<TException>() where TException : Exception;
    // ICatchStage<TResult, TException> When(Func<TException, bool> func);
    ICatchStage<TResult> WithRetries(uint retries);
    IFinalizeStage<TResult> WithTimeout(uint msTimeout);
}
