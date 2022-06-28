using ASLHelper.UnityHelper;

namespace ASLHelper;

public partial class Unity
{
    private readonly MemoryWatcherList _memWatchers = new();

    public MemoryWatcher this[string name]
    {
        get
        {
            if (_memWatchers.FirstOrDefault(w => w.Name == name) is MemoryWatcher watcher)
                return watcher;

            throw new InvalidOperationException($"The specified item '{name}' could not be found in the Helper!");
        }
        set
        {
            RemoveWatcher(name);

            value.Name = name;
            _memWatchers.Add(value);
        }
    }

    public void RemoveWatcher(string name)
    {
        var index = _memWatchers.FindIndex(w => w.Name == name);

        if (index >= 0)
            _memWatchers.RemoveAt(index);
    }

    public bool Update()
    {
        if (!Loaded)
            return false;

        UpdateAll(Game);
        return true;
    }

    public bool UpdateAll(Process game)
    {
        if (!Loaded)
            return false;

        _memWatchers.UpdateAll(game);
        return true;
    }

    public MemoryWatcher<T> Add<T>(nint staticFieldTable, int staticFieldOffset, params int[] offsets) where T : unmanaged
    {
        if (staticFieldTable == 0)
            throw new NullReferenceException("Static field table address was null!");

        var watcher = new MemoryWatcher<T>(new DeepPointer(staticFieldTable + staticFieldOffset, offsets));
        _memWatchers.Add(watcher);

        return watcher;
    }

    public StringWatcher AddString(int length, nint staticFieldTable, int staticFieldOffset, params int[] offsets)
    {
        if (staticFieldTable == 0)
            throw new NullReferenceException("Static field table address was null!");

        var watcher = new StringWatcher(new DeepPointer(staticFieldTable + staticFieldOffset, offsets), ReadStringType.UTF16, length * 2);
        _memWatchers.Add(watcher);

        return watcher;
    }

    public MemoryString AddString(nint staticFieldTable, int staticFieldOffset, params int[] offsets)
    {
        if (staticFieldTable == 0)
            throw new NullReferenceException("Static field table address was null!");

        var str = new MemoryString(staticFieldTable, staticFieldOffset, offsets);
        _memWatchers.Add(str);

        return str;
    }

    public MemoryList<T> AddList<T>(nint staticFieldTable, int staticFieldOffset, params int[] offsets) where T : unmanaged
    {
        if (staticFieldTable == 0)
            throw new NullReferenceException("Static field table address was null!");

        var list = new MemoryList<T>(staticFieldTable, staticFieldOffset, offsets);
        _memWatchers.Add(list);

        return list;
    }

    public MemoryArray<T> AddArray<T>(nint staticFieldTable, int staticFieldOffset, params int[] offsets) where T : unmanaged
    {
        if (staticFieldTable == 0)
            throw new NullReferenceException("Static field table address was null!");

        var list = new MemoryArray<T>(staticFieldTable, staticFieldOffset, offsets);
        _memWatchers.Add(list);

        return list;
    }
}
