extern alias Ls;

using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory.Ipc;

using Ls::LiveSplit.ComponentUtil;

namespace AslHelp.Memory.Monitoring;

public sealed class StringWatcher : WatcherBase<string, IMemoryReader>
{
    private readonly int _length;
    private readonly ReadStringType _type;

    public StringWatcher(int length, ReadStringType type, IMemoryReader memory, nint baseAddress, params int[] offsets)
        : base(memory, baseAddress, offsets)
    {
        _length = length;
        _type = type;
    }

    public StringWatcher(int length, ReadStringType type, IMemoryReader memory, TickCounter counter, nint baseAddress, params int[] offsets)
        : base(memory, counter, baseAddress, offsets)
    {
        _length = length;
        _type = type;
    }

    protected override bool TryRead(IMemoryReader memory, nint baseAddress, int[] offsets, [NotNullWhen(true)] out string? value)
    {
        return memory.TryReadString(out value, _length, _type, baseAddress, offsets);
    }

    protected override bool Equals(string? old, string? current)
    {
        return old == current;
    }

    public override string ToString()
    {
        return $"StringWatcher({FormatPath()})";
    }
}
