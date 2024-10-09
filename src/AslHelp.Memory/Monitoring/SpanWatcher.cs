using System.Diagnostics.CodeAnalysis;

using AslHelp.Memory.Ipc;

namespace AslHelp.Memory.Monitoring;

public sealed class SpanWatcher<T> : WatcherBase<T[], IMemoryReader>
    where T : unmanaged
{
    private readonly int _length;

    public SpanWatcher(int length, IMemoryReader memory, nint baseAddress, params int[] offsets)
        : base(memory, baseAddress, offsets)
    {
        _length = length;
    }

    public SpanWatcher(int length, IMemoryReader memory, TickCounter counter, nint baseAddress, params int[] offsets)
        : base(memory, counter, baseAddress, offsets)
    {
        _length = length;
    }

    protected override bool TryRead(IMemoryReader memory, nint baseAddress, int[] offsets, [NotNullWhen(true)] out T[]? value)
    {
        return memory.TryReadSpan(out value, _length, baseAddress, offsets);
    }

    protected override bool Equals(T[]? old, T[]? current)
    {
        if (old is null || current is null)
        {
            return false;
        }

        if (old.Length != current.Length)
        {
            return false;
        }

        for (int i = 0; i < old.Length; i++)
        {
            if (!old[i].Equals(current[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override string ToString()
    {
        return $"ArrayWatcher<{typeof(T).Name}>({FormatPath()})";
    }
}
