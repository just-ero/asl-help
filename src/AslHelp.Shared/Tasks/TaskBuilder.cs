using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AslHelp.Shared.Logging;

namespace AslHelp.Shared.Tasks;

public class TaskBuilder<TResult> :
    ICreationStage<TResult>,
    ICatchStage<TResult>,
    IFinalizeStage<TResult>
{
    private readonly ILogger _logger;
    private readonly CancellationToken _ct;

    private readonly TaskBuilderContext<TResult> _context = new();
    private readonly List<object> _args = [];
    private readonly List<IExceptionStage> _exceptions = [];

    private string? _startupMessage;
    private string? _failureMessage;
    private Func<TResult?, string>? _completionMessage;

    private BuilderFunc<TResult>? _func;

    private int _retries = -1;
    private int _timeout;

    private TaskBuilder(ILogger logger, CancellationToken ct = default)
    {
        _logger = logger;
        _ct = ct;
    }

    public static ICreationStage<TResult> Create(ILogger logger, CancellationToken ct = default)
    {
        return new TaskBuilder<TResult>(logger, ct);
    }

    ICreationStage<TResult> ICreationStage<TResult>.WithStartupMessage(string message)
    {
        _startupMessage = message;
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
        _failureMessage = message;
        return this;
    }

    IFinalizeStage<TResult> IFinalizeStage<TResult>.WithCompletionMessage(string message)
    {
        _completionMessage = (_) => message;
        return this;
    }

    IFinalizeStage<TResult> IFinalizeStage<TResult>.WithCompletionMessage(Func<TResult?, string> func)
    {
        _completionMessage = func;
        return this;
    }

    async Task<TResult?> IFinalizeStage<TResult>.RunAsync()
    {
        if (_startupMessage is not null)
        {
            _logger.Log(_startupMessage);
        }

        while (!_ct.IsCancellationRequested)
        {
            try
            {
                if (await _func!.Invoke(_context, [.. _args])) // Cannot be null.
                {
                    if (_completionMessage is not null)
                    {
                        _logger.Log(_completionMessage(_context.Result));
                    }

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

                    if (exs.FailureMessage is not null)
                    {
                        _logger.Log(exs.FailureMessage(ex));
                    }

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
            if (_failureMessage is not null)
            {
                _logger.Log(_failureMessage);
            }

            if (_retries == 0)
            {
                break;
            }

            if (_retries > 0)
            {
                string times = _retries == 1 ? "time" : "times";

                _logger.Log($"  => Retrying {_retries} more {times} in {_timeout}ms...");
            }
            else
            {
                _logger.Log($"  => Retrying in {_timeout}ms...");
            }

            _retries--;

            await Task.Delay(_timeout, _ct);
        }

    Break:
        return default;
    }
}
