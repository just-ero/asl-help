using System.Diagnostics.CodeAnalysis;
using System.Text;

using AslHelp.Memory.Ipc;

namespace AslHelp.Memory.Monitoring;

public abstract class WatcherBase<T, TMemory> : IWatcher<T>
    where TMemory : IMemoryReader
{
    private readonly TMemory _memory;

    private readonly nint _baseAddress;
    private readonly int[] _offsets;

    private readonly TickCounter? _counter;
    private uint _tick;

    private T? _old;
    private T? _current;

    public WatcherBase(TMemory memory, nint baseAddress, params int[] offsets)
    {
        _memory = memory;
        _baseAddress = baseAddress;
        _offsets = offsets;
    }

    public WatcherBase(TMemory memory, TickCounter counter, nint baseAddress, params int[] offsets)
    {
        _memory = memory;
        _counter = counter;
        _baseAddress = baseAddress;
        _offsets = offsets;
    }

    public T? Old
    {
        get
        {
            Update();
            return _old;
        }
    }

    public T? Current
    {
        get
        {
            Update();
            return _current;
        }
    }

    public bool Changed { get; private set; }

    public bool IsEnabled { get; set; }
    public bool UpdateOnFail { get; set; }

    protected abstract bool TryRead(TMemory memory, nint baseAddress, int[] offsets, [NotNullWhen(true)] out T? value);
    protected abstract bool Equals(T? old, T? current);

    public abstract override string ToString();

    private void Update()
    {
        if (!IsEnabled)
        {
            return;
        }

        if (_counter is not null)
        {
            if (_counter == _tick)
            {
                return;
            }

            _tick = _counter;
        }

        if (TryRead(_memory, _baseAddress, _offsets, out T? value))
        {
            _old = _current;
            _current = value;
        }
        else if (UpdateOnFail)
        {
            _old = _current;
            _current = default;
        }

        Changed = !Equals(_old, _current);
    }

    protected string FormatPath()
    {
        StringBuilder sb = new();

        sb.Append($"0x{(ulong)_baseAddress:X}");

        foreach (int offset in _offsets)
        {
            sb.Append($", 0x{offset:X}");
        }

        return sb.ToString();
    }

    object? IWatcher.Old => Old;
    object? IWatcher.Current => Current;
}
