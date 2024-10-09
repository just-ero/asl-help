using System;
using System.Threading.Tasks;

namespace AslHelp.Shared.Tasks;

public class BuilderFunc<TResult>
{
    private enum FuncType
    {
        Sync,
        Async,
        SyncWithArgs,
        AsyncWithArgs
    }

    private readonly FuncType _type;

    private readonly Func<TaskBuilderContext<TResult>, bool>? _sync;
    private readonly Func<TaskBuilderContext<TResult>, Task<bool>>? _async;
    private readonly Func<TaskBuilderContext<TResult>, object[], bool>? _syncWithArgs;
    private readonly Func<TaskBuilderContext<TResult>, object[], Task<bool>>? _asyncWithArgs;

    public BuilderFunc(Func<TaskBuilderContext<TResult>, bool> func)
    {
        _type = FuncType.Sync;
        _sync = func;
    }

    public BuilderFunc(Func<TaskBuilderContext<TResult>, Task<bool>> func)
    {
        _type = FuncType.Async;
        _async = func;
    }

    public BuilderFunc(Func<TaskBuilderContext<TResult>, object[], bool> func)
    {
        _type = FuncType.SyncWithArgs;
        _syncWithArgs = func;
    }

    public BuilderFunc(Func<TaskBuilderContext<TResult>, object[], Task<bool>> func)
    {
        _type = FuncType.AsyncWithArgs;
        _asyncWithArgs = func;
    }

    public Task<bool> Invoke(TaskBuilderContext<TResult> ctx, object[] args)
    {
        return _type switch
        {
            FuncType.Sync => Task.FromResult(_sync!(ctx)),
            FuncType.Async => _async!(ctx),
            FuncType.SyncWithArgs => Task.FromResult(_syncWithArgs!(ctx, args)),
            FuncType.AsyncWithArgs => _asyncWithArgs!(ctx, args),
            _ => throw new InvalidOperationException()
        };
    }
}
