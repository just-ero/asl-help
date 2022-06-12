using ASLHelper.UnityHelper;

namespace ASLHelper;

public partial class Unity
{
    private readonly MemoryWatcherList _memWatchers = new();

    public object this[string name]
    {
        get
        {
            if (_memWatchers.FirstOrDefault(w => w.Name == name) is MemoryWatcher watcher)
                return watcher;

            throw new InvalidOperationException($"The specified item '{name}' could not be found in the Helper!");
        }
    }

    public void RemoveWatcher(string name)
    {
        var index = _memWatchers.FindIndex(w => w.Name == name);

        if (index >= 0)
            _memWatchers.RemoveAt(index);
    }

    public void Update()
    {
        UpdateAll(Game);
    }

    public void UpdateAll(Process game)
    {
        _memWatchers.UpdateAll(game);
    }

    public MemoryWatcher<T> Make<T>(IntPtr staticFieldTable, int staticFieldOffset, params int[] offsets) where T : unmanaged
    {
        if (staticFieldTable == IntPtr.Zero)
            throw new NullReferenceException("Static field table address was null!");

        var watcher = new MemoryWatcher<T>(new DeepPointer(staticFieldTable + staticFieldOffset, offsets));
        _memWatchers.Add(watcher);

        return watcher;
    }

    public StringWatcher MakeString(int length, IntPtr staticFieldTable, int staticFieldOffset, params int[] offsets)
    {
        if (staticFieldTable == IntPtr.Zero)
            throw new NullReferenceException("Static field table address was null!");

        var watcher = new StringWatcher(new DeepPointer(staticFieldTable + staticFieldOffset, offsets), ReadStringType.UTF16, length * 2);
        _memWatchers.Add(watcher);

        return watcher;
    }

    public MemoryString MakeString(IntPtr staticFieldTable, int staticFieldOffset, params int[] offsets)
    {
        if (staticFieldTable == IntPtr.Zero)
            throw new NullReferenceException("Static field table address was null!");

        var str = new MemoryString(staticFieldTable, staticFieldOffset, offsets);
        _memWatchers.Add(str);

        return str;
    }

    public MemoryList<T> MakeList<T>(IntPtr staticFieldTable, int staticFieldOffset, params int[] offsets) where T : unmanaged
    {
        if (staticFieldTable == IntPtr.Zero)
            throw new NullReferenceException("Static field table address was null!");

        var list = new MemoryList<T>(staticFieldTable, staticFieldOffset, offsets);
        _memWatchers.Add(list);

        return list;
    }

    public MemoryArray<T> MakeArray<T>(IntPtr staticFieldTable, int staticFieldOffset, params int[] offsets) where T : unmanaged
    {
        if (staticFieldTable == IntPtr.Zero)
            throw new NullReferenceException("Static field table address was null!");

        var list = new MemoryArray<T>(staticFieldTable, staticFieldOffset, offsets);
        _memWatchers.Add(list);

        return list;
    }
}
