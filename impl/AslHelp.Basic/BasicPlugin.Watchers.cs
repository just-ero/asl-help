extern alias Ls;

using System.Collections.Generic;

using AslHelp.Memory;
using AslHelp.Memory.Monitoring;
using AslHelp.Shared;

using Ls::LiveSplit.ComponentUtil;

public partial class Basic
{
    private TickCounter _tick = new(1);
    protected readonly Dictionary<string, IWatcher> _watchers = [];

    public IWatcher? this[string name]
    {
        get => _watchers[name];
        set
        {
            if (value is null)
            {
                _watchers.Remove(name);
            }
            else
            {
                _watchers[name] = value;
            }
        }
    }

    public void Update()
    {
        _tick++;
    }

    public void MapPointers()
    {
        ThrowHelper.ThrowIfNull(_asl.Current);

        foreach (var watcher in _watchers)
        {
            _asl.Current[watcher.Key] = watcher.Value.Current;
        }
    }

    public IWatcher<T> Make<T>(int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return Make<T>(MainModule, baseOffset, offsets);
    }

    public IWatcher<T> Make<T>(string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return Make<T>(Modules[moduleName], baseOffset, offsets);
    }

    public IWatcher<T> Make<T>(Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return Make<T>(module.Base + baseOffset, offsets);
    }

    public IWatcher<T> Make<T>(nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfLessThan((long)baseAddress, 1);

        return new Watcher<T>(Memory, _tick, baseAddress, offsets);
    }

    public IWatcher<T[]> MakeArray<T>(int length, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return MakeArray<T>(length, MainModule, baseOffset, offsets);
    }

    public IWatcher<T[]> MakeArray<T>(int length, string moduleName, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return MakeArray<T>(length, Modules[moduleName], baseOffset, offsets);
    }

    public IWatcher<T[]> MakeArray<T>(int length, Module module, int baseOffset, params int[] offsets)
        where T : unmanaged
    {
        return MakeArray<T>(length, module.Base + baseOffset, offsets);
    }

    public IWatcher<T[]> MakeArray<T>(int length, nint baseAddress, params int[] offsets)
        where T : unmanaged
    {
        ThrowHelper.ThrowIfLessThan(length, 0);
        ThrowHelper.ThrowIfLessThan((long)baseAddress, 1);

        return new ArrayWatcher<T>(length, Memory, _tick, baseAddress, offsets);
    }

    public IWatcher<string> MakeString(int baseOffset, params int[] offsets)
    {
        return MakeString(128, ReadStringType.AutoDetect, MainModule, baseOffset, offsets);
    }

    public IWatcher<string> MakeString(int length, int baseOffset, params int[] offsets)
    {
        return MakeString(length, ReadStringType.AutoDetect, MainModule, baseOffset, offsets);
    }

    public IWatcher<string> MakeString(ReadStringType stringType, int baseOffset, params int[] offsets)
    {
        return MakeString(128, stringType, MainModule, baseOffset, offsets);
    }

    public IWatcher<string> MakeString(int length, ReadStringType stringType, int baseOffset, params int[] offsets)
    {
        return MakeString(length, stringType, MainModule, baseOffset, offsets);
    }

    public IWatcher<string> MakeString(string module, int baseOffset, params int[] offsets)
    {
        return MakeString(128, ReadStringType.AutoDetect, Modules[module], baseOffset, offsets);
    }

    public IWatcher<string> MakeString(int length, string module, int baseOffset, params int[] offsets)
    {
        return MakeString(length, ReadStringType.AutoDetect, Modules[module], baseOffset, offsets);
    }

    public IWatcher<string> MakeString(ReadStringType stringType, string module, int baseOffset, params int[] offsets)
    {
        return MakeString(128, stringType, Modules[module], baseOffset, offsets);
    }

    public IWatcher<string> MakeString(int length, ReadStringType stringType, string module, int baseOffset, params int[] offsets)
    {
        return MakeString(length, stringType, Modules[module], baseOffset, offsets);
    }

    public IWatcher<string> MakeString(Module module, int baseOffset, params int[] offsets)
    {
        return MakeString(128, ReadStringType.AutoDetect, module, baseOffset, offsets);
    }

    public IWatcher<string> MakeString(int length, Module module, int baseOffset, params int[] offsets)
    {
        return MakeString(length, ReadStringType.AutoDetect, module, baseOffset, offsets);
    }

    public IWatcher<string> MakeString(ReadStringType stringType, Module module, int baseOffset, params int[] offsets)
    {
        return MakeString(128, stringType, module, baseOffset, offsets);
    }

    public IWatcher<string> MakeString(int length, ReadStringType stringType, Module module, int baseOffset, params int[] offsets)
    {
        return MakeString(length, stringType, module.Base + baseOffset, offsets);
    }

    public IWatcher<string> MakeString(nint baseAddress, params int[] offsets)
    {
        return MakeString(128, ReadStringType.AutoDetect, baseAddress, offsets);
    }

    public IWatcher<string> MakeString(int length, nint baseAddress, params int[] offsets)
    {
        return MakeString(length, ReadStringType.AutoDetect, baseAddress, offsets);
    }

    public IWatcher<string> MakeString(ReadStringType stringType, nint baseAddress, params int[] offsets)
    {
        return MakeString(128, stringType, baseAddress, offsets);
    }

    public IWatcher<string> MakeString(int length, ReadStringType stringType, nint baseAddress, params int[] offsets)
    {
        ThrowHelper.ThrowIfLessThan(length, 0);
        ThrowHelper.ThrowIfLessThan((long)baseAddress, 1);

        return new AslHelp.Memory.Monitoring.StringWatcher(length, stringType, Memory, _tick, baseAddress, offsets);
    }
}
