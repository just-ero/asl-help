using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory.Ipc;

namespace AslHelp.Memory.Monitoring;

public sealed class StringWatcher : WatcherBase<string, IMemoryReader>
{
    private readonly int _length;
    private readonly StringType _type;

    public StringWatcher(int length, StringType type, IMemoryReader memory, nuint baseAddress, params int[] offsets)
        : base(memory, baseAddress, offsets)
    {
        _length = length;
        _type = type;
    }

    public StringWatcher(int length, StringType type, IMemoryReader memory, TickCounter counter, nuint baseAddress, params int[] offsets)
        : base(memory, counter, baseAddress, offsets)
    {
        _length = length;
        _type = type;
    }

    protected override bool TryRead(IMemoryReader memory, nuint baseAddress, int[] offsets, [NotNullWhen(true)] out string? value)
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
