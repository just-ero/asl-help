using System.Diagnostics.CodeAnalysis;

using AslHelp.Shared;
using AslHelp.Memory.Ipc;

namespace AslHelp.Memory.Monitoring;

public sealed class ArrayWatcher<T> : WatcherBase<T[], IMemoryReader>
    where T : unmanaged
{
    private readonly nuint _length;

    public ArrayWatcher(nuint length, IMemoryReader memory, nuint baseAddress, params int[] offsets)
        : base(memory, baseAddress, offsets)
    {
        _length = length;
    }

    public ArrayWatcher(nuint length, IMemoryReader memory, TickCounter counter, nuint baseAddress, params int[] offsets)
        : base(memory, counter, baseAddress, offsets)
    {
        _length = length;
    }

    protected override bool TryRead(IMemoryReader memory, nuint baseAddress, int[] offsets, [NotNullWhen(true)] out T[]? value)
    {
        return memory.TryReadArray(out value, _length, baseAddress, offsets);
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

        for (var i = 0; i < old.Length; i++)
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
