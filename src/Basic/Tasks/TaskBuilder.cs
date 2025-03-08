namespace AslHelp.Tasks;

public class TaskBuilder<TResult> :
    ICreationStage<TResult>,
    ICatchStage<TResult>,
    IFinalizeStage<TResult>
{
    private readonly Process _game;
    private readonly CancellationTokenSource _cts;

    private readonly TaskBuilderContext<TResult> _context = new();
    private readonly List<object> _args = new();
    private readonly List<IExceptionStage> _exceptions = new();

    private BuilderMessage<TResult> _startupMessage;
    private BuilderMessage<TResult> _failureMessage;
    private BuilderMessage<TResult> _completionMessage;

    private BuilderFunc<TResult> _func;

    private int _retries = -1;
    private int _timeout;

    private TaskBuilder(CancellationTokenSource cts)
    {
        _game = Basic.Instance.Game;
        _cts = cts;
    }

    public static ICreationStage<TResult> Create(CancellationTokenSource cts)
    {
        return new TaskBuilder<TResult>(cts);
    }

    ICreationStage<TResult> ICreationStage<TResult>.WithStartupMessage(string message)
    {
        if (_startupMessage is not null)
        {
            string msg = "Startup message was already set.";
            throw new InvalidOperationException(msg);
        }

        _startupMessage = new(message);
        return this;
    }

    ICreationStage<TResult> ICreationStage<TResult>.WithArgs(params object[] args)
    {
        _args.AddRange(args);
        return this;
    }

    ICatchStage<TResult> ICreationStage<TResult>.Exec(Func<TaskBuilderContext<TResult>, bool> func)
    {
        _func = new(func);
        return this;
    }

    ICatchStage<TResult> ICreationStage<TResult>.Exec(Func<TaskBuilderContext<TResult>, Task<bool>> func)
    {
        _func = new(func);
        return this;
    }

    ICatchStage<TResult> ICreationStage<TResult>.Exec(Func<TaskBuilderContext<TResult>, object[], bool> func)
    {
        _func = new(func);
        return this;
    }

    ICatchStage<TResult> ICreationStage<TResult>.Exec(Func<TaskBuilderContext<TResult>, object[], Task<bool>> func)
    {
        _func = new(func);
        return this;
    }

    IExceptionStage<TResult, TException> ICatchStage<TResult>.Catch<TException>()
    {
        ExceptionStage<TResult, TException> exception = new(this);
        _exceptions.Add(exception);

        return exception;
    }

    ICatchStage<TResult> ICatchStage<TResult>.WithRetries(uint retries)
    {
        _retries = (int)retries;
        return this;
    }

    IFinalizeStage<TResult> ICatchStage<TResult>.WithTimeout(uint msTimeout)
    {
        _timeout = (int)msTimeout;
        return this;
    }

    IFinalizeStage<TResult> IFinalizeStage<TResult>.WithFailureMessage(string message)
    {
        if (_failureMessage is not null)
        {
            string msg = "Failure message was already set.";
            throw new InvalidOperationException(msg);
        }

        _failureMessage = new(message);
        return this;
    }

    IFinalizeStage<TResult> IFinalizeStage<TResult>.WithCompletionMessage(string message)
    {
        if (_completionMessage is not null)
        {
            string msg = "Startup message was already set.";
            throw new InvalidOperationException(msg);
        }

        _completionMessage = new(message);
        return this;
    }

    IFinalizeStage<TResult> IFinalizeStage<TResult>.WithCompletionMessage(Func<TResult, string> func)
    {
        if (_completionMessage is not null)
        {
            string msg = "Startup message was already set.";
            throw new InvalidOperationException(msg);
        }

        _completionMessage = new(func);
        return this;
    }

    async Task<TResult> IFinalizeStage<TResult>.RunAsync()
    {
        if (_cts is null)
        {
            return default;
        }

        _startupMessage?.Send();

        while (!_cts.IsCancellationRequested && _game is not null && !_game.HasExited)
        {
            try
            {
                if (await _func.Invoke(_context, _args.ToArray()))
                {
                    _completionMessage?.Send(_context.Result);
                    return _context.Result;
                }
            }
            catch (Exception ex)
            {
                foreach (IExceptionStage exs in _exceptions)
                {
                    if (exs.ExceptionType != ex.GetType())
                    {
                        continue;
                    }

                    exs.FailureMessage?.Send(ex);

                    switch (exs.FailureBehavior)
                    {
                        case FailureBehavior.Throw: throw;
                        case FailureBehavior.Retry: goto Continue;
                        case FailureBehavior.Break: goto Break;
                    }
                }

                throw;
            }

        Continue:
            _failureMessage?.Send();

            if (_retries == 0)
            {
                break;
            }

            if (_retries > 0)
            {
                string times = _retries == 1 ? "time" : "times";

                Debug.Info($"  => Retrying {_retries} more {times} in {_timeout}ms...");
            }
            else
            {
                Debug.Info($"  => Retrying in {_timeout}ms...");
            }

            _retries--;

            await Task.Delay(_timeout, _cts.Token);
        }

    Break:
        return default;
    }
}
