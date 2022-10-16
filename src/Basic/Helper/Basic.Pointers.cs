using AslHelp.MemUtils.Exceptions;
using AslHelp.MemUtils.Pointers;
using LiveSplit.ComponentUtil;

public partial class Basic
{
    internal uint Tick { get; private set; } = 1;
    protected readonly MemoryWatcherList _watchers = new();

    public void MapPointers()
    {
        foreach (MemoryWatcher watcher in _watchers)
        {
            current[watcher.Name] = watcher.Current;
        }
    }

    public MemoryWatcher this[string name]
    {
        get
        {
            if (_watchers.FirstOrDefault(w => w.Name == name) is MemoryWatcher watcher)
            {
                return watcher;
            }

            throw new KeyNotFoundException($"The given watcher '{name}' was not present in the helper.");
        }
        set
        {
            RemoveWatcher(name);

            if (value is null)
            {
                return;
            }

            value.Name = name;
            _watchers.Add(value);
        }
    }

    public void RemoveWatcher(string name)
    {
        int index = _watchers.FindIndex(w => w.Name == name);

        if (index > -1)
        {
            _watchers.RemoveAt(index);
        }
    }

    public void Update()
    {
        Tick++;
        _watchers.UpdateAll(Game);
    }

    public Pointer<T> Make<T>(int baseOffset, params int[] offsets) where T : unmanaged
    {
        return Make<T>(MainModule, baseOffset, offsets);
    }

    public Pointer<T> Make<T>(string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return Make<T>(Modules[module], baseOffset, offsets);
    }

    public Pointer<T> Make<T>(Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            string msg = $"[Make<{typeof(T).Name}>] Module was not found.";
            throw new FatalNotFoundException(msg);
        }

        return new(module.Base + baseOffset, offsets);
    }

    public Pointer<T> Make<T>(nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (baseAddress <= 0)
        {
            string msg = $"[Make<{typeof(T).Name}>] The base address was 0.";
            throw new InvalidAddressException(msg);
        }

        return new(baseAddress, offsets);
    }

    public SpanPointer<T> MakeSpan<T>(int length, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return MakeSpan<T>(length, MainModule, baseOffset, offsets);
    }

    public SpanPointer<T> MakeSpan<T>(int length, string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return MakeSpan<T>(length, Modules[module], baseOffset, offsets);
    }

    public SpanPointer<T> MakeSpan<T>(int length, Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            string msg = $"[Make<{typeof(T).Name}>] Module was not found.";
            throw new FatalNotFoundException(msg);
        }

        return new(length, module.Base + baseOffset, offsets);
    }

    public SpanPointer<T> MakeSpan<T>(int length, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (baseAddress <= 0)
        {
            string msg = $"[Make<{typeof(T).Name}>] The base address was 0.";
            throw new InvalidAddressException(msg);
        }

        return new(length, baseAddress, offsets);
    }

    public StringPointer MakeString(int baseOffset, params int[] offsets)
    {
        return MakeString(128, ReadStringType.AutoDetect, MainModule, baseOffset, offsets);
    }

    public StringPointer MakeString(int length, int baseOffset, params int[] offsets)
    {
        return MakeString(length, ReadStringType.AutoDetect, MainModule, baseOffset, offsets);
    }

    public StringPointer MakeString(ReadStringType stringType, int baseOffset, params int[] offsets)
    {
        return MakeString(128, stringType, MainModule, baseOffset, offsets);
    }

    public StringPointer MakeString(int length, ReadStringType stringType, int baseOffset, params int[] offsets)
    {
        return MakeString(length, stringType, MainModule, baseOffset, offsets);
    }

    public StringPointer MakeString(string module, int baseOffset, params int[] offsets)
    {
        return MakeString(128, ReadStringType.AutoDetect, Modules[module], baseOffset, offsets);
    }

    public StringPointer MakeString(int length, string module, int baseOffset, params int[] offsets)
    {
        return MakeString(length, ReadStringType.AutoDetect, Modules[module], baseOffset, offsets);
    }

    public StringPointer MakeString(ReadStringType stringType, string module, int baseOffset, params int[] offsets)
    {
        return MakeString(128, stringType, Modules[module], baseOffset, offsets);
    }

    public StringPointer MakeString(int length, ReadStringType stringType, string module, int baseOffset, params int[] offsets)
    {
        return MakeString(length, stringType, Modules[module], baseOffset, offsets);
    }

    public StringPointer MakeString(Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            string msg = $"[MakeString] Module was not found.";
            throw new FatalNotFoundException(msg);
        }

        return new(128, ReadStringType.AutoDetect, module.Base + baseOffset, offsets);
    }

    public StringPointer MakeString(int length, Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            string msg = $"[MakeString] Module was not found.";
            throw new FatalNotFoundException(msg);
        }

        return new(length, ReadStringType.AutoDetect, module.Base + baseOffset, offsets);
    }

    public StringPointer MakeString(ReadStringType stringType, Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            string msg = $"[MakeString] Module was not found.";
            throw new FatalNotFoundException(msg);
        }

        return new(128, stringType, module.Base + baseOffset, offsets);
    }

    public StringPointer MakeString(int length, ReadStringType stringType, Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            string msg = $"[MakeString] Module was not found.";
            throw new FatalNotFoundException(msg);
        }

        return new(length, stringType, module.Base + baseOffset, offsets);
    }

    public StringPointer MakeString(nint baseAddress, params int[] offsets)
    {
        if (baseAddress <= 0)
        {
            string msg = $"[MakeString] The base address was 0.";
            throw new InvalidAddressException(msg);
        }

        return new(128, ReadStringType.AutoDetect, baseAddress, offsets);
    }

    public StringPointer MakeString(int length, nint baseAddress, params int[] offsets)
    {
        if (baseAddress <= 0)
        {
            string msg = $"[MakeString] The base address was 0.";
            throw new InvalidAddressException(msg);
        }

        return new(length, ReadStringType.AutoDetect, baseAddress, offsets);
    }

    public StringPointer MakeString(ReadStringType stringType, nint baseAddress, params int[] offsets)
    {
        if (baseAddress <= 0)
        {
            string msg = $"[MakeString] The base address was 0.";
            throw new InvalidAddressException(msg);
        }

        return new(128, stringType, baseAddress, offsets);
    }

    public StringPointer MakeString(int length, ReadStringType stringType, nint baseAddress, params int[] offsets)
    {
        if (baseAddress <= 0)
        {
            string msg = $"[MakeString] The base address was 0.";
            throw new InvalidAddressException(msg);
        }

        return new(length, stringType, baseAddress, offsets);
    }
}
