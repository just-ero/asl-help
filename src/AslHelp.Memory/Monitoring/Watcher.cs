using AslHelp.Memory.Ipc;

namespace AslHelp.Memory.Monitoring;

public sealed class Watcher<T> : WatcherBase<T, IMemoryReader>
    where T : unmanaged
{
    public Watcher(IMemoryReader memory, nint baseAddress, params int[] offsets)
        : base(memory, baseAddress, offsets) { }

    public Watcher(IMemoryReader memory, TickCounter counter, nint baseAddress, params int[] offsets)
        : base(memory, counter, baseAddress, offsets) { }

    protected override bool Equals(T old, T current)
    {
        return old.Equals(current);
    }

    protected override bool TryRead(IMemoryReader memory, nint baseAddress, int[] offsets, out T value)
    {
        return memory.TryRead(out value, baseAddress, offsets);
    }

    public override string ToString()
    {
        return $"Watcher<{typeof(T).Name}>({FormatPath()})";
    }
}
