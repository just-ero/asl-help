namespace AslHelp.Tasks;

public interface IFinalizeStage<TResult>
{
    IFinalizeStage<TResult> WithFailureMessage(string message);
    IFinalizeStage<TResult> WithCompletionMessage(string message);
    IFinalizeStage<TResult> WithCompletionMessage(Func<TResult, string> func);

    Task<TResult> RunAsync();
}
