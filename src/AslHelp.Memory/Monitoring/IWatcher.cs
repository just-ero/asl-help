extern alias Ls;

using Ls::LiveSplit.ComponentUtil;

namespace AslHelp.Memory.Monitoring;

public interface IWatcher
{
    object? Old { get; }
    object? Current { get; }

    bool Changed { get; }

    bool Enabled { get; set; }
    MemoryWatcher.ReadFailAction FailAction { get; set; }
}

public interface IWatcher<T> : IWatcher
{
    new T? Old { get; }
    new T? Current { get; }
}
