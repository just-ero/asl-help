namespace AslHelp.Memory.Monitoring;

public interface IWatcher
{
    object? Old { get; }
    object? Current { get; }

    bool Changed { get; }

    bool IsEnabled { get; set; }
    bool UpdateOnFail { get; set; }
}

public interface IWatcher<T> : IWatcher
{
    new T? Old { get; }
    new T? Current { get; }
}
