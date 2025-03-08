namespace AslHelp.Tasks;

public interface ICatchStage<TResult>
{
    IExceptionStage<TResult, TException> Catch<TException>() where TException : Exception;
    ICatchStage<TResult> WithRetries(uint retries);
    IFinalizeStage<TResult> WithTimeout(uint msTimeout);
}
