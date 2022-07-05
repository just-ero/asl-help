namespace ASLHelper;

public partial class Main
{
    private protected readonly MemoryWatcherList _watchers = new();

    public MemoryWatcher this[string name]
    {
        get
        {
            if (_watchers.FirstOrDefault(w => w.Name == name) is MemoryWatcher watcher)
                return watcher;

            throw new InvalidOperationException($"The specified item '{name}' could not be found in the Helper!");
        }
        set
        {
            RemoveWatcher(name);

            value.Name = name;
            _watchers.Add(value);
        }
    }

    public void RemoveWatcher(string name)
    {
        var index = _watchers.FindIndex(w => w.Name == name);

        if (index > -1)
            _watchers.RemoveAt(index);
    }

    public virtual bool Update()
    {
        UpdateAll(Game);
        return true;
    }

    public virtual bool UpdateAll(Process game)
    {
        _watchers.UpdateAll(game);
        return true;
    }

    public ReadWriteWatcher<T> Add<T>(nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (baseAddress == 0)
            throw new NullReferenceException("Static field table address was null!");

        var watcher = new ReadWriteWatcher<T>(baseAddress, offsets);
        _watchers.Add(watcher);

        return watcher;
    }

    public StringWatcher AddString(int length, nint staticFieldTable, int staticFieldOffset, params int[] offsets)
    {
        if (staticFieldTable == 0)
            throw new NullReferenceException("Static field table address was null!");

        var watcher = new StringWatcher(new DeepPointer(staticFieldTable + staticFieldOffset, offsets), ReadStringType.UTF16, length * 2);
        _watchers.Add(watcher);

        return watcher;
    }
}
