namespace AslHelp.Tasks;

public interface ICreationStage<TResult>
{
    ICreationStage<TResult> WithStartupMessage(string message);
    ICreationStage<TResult> WithArgs(params object[] args);
    ICatchStage<TResult> Exec(Func<TaskBuilderContext<TResult>, bool> func);
    ICatchStage<TResult> Exec(Func<TaskBuilderContext<TResult>, Task<bool>> func);
    ICatchStage<TResult> Exec(Func<TaskBuilderContext<TResult>, object[], bool> func);
    ICatchStage<TResult> Exec(Func<TaskBuilderContext<TResult>, object[], Task<bool>> func);
}
