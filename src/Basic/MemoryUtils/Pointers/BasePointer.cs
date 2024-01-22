using LiveSplit.ComponentUtil;
using System.Text;

namespace AslHelp.MemUtils.Pointers;

public abstract class BasePointer<T> : MemoryWatcher
{
    protected readonly nint _base;
    protected readonly int[] _offsets;

    protected uint _tick;

    protected BasePointer(nint baseAddress, int[] offsets)
        : base(IntPtr.Zero)
    {
        _base = baseAddress;
        _offsets = offsets;

        Current = Default;
        Old = Default;
    }

    protected virtual T Default { get; } = default;

    public new T Current
    {
        get => (T)(base.Current ?? Default);
        set => base.Current = value;
    }

    public new T Old
    {
        get => (T)(base.Old ?? Default);
        set => base.Old = value;
    }

    public abstract bool Write(T value);
    protected abstract bool TryUpdate(out T result);
    protected abstract bool HasChanged(T old, T current);

    public delegate void OnChangedHandler(T old, T current);
    public OnChangedHandler OnChanged { get; set; }

    public sealed override bool Update(Process process)
    {
        if (_tick == Basic.Instance.Tick)
        {
            return false;
        }

        _tick = Basic.Instance.Tick;
        Changed = false;

        if (!Enabled || !CheckInterval())
        {
            return false;
        }

        Old = Current;

        if (!TryUpdate(out T result))
        {
            if (FailAction == ReadFailAction.DontUpdate)
            {
                return false;
            }

            Current = Default;
        }
        else
        {
            Current = result;
        }

        if (!InitialUpdate)
        {
            InitialUpdate = true;
            return false;
        }

        if (HasChanged(Old, Current))
        {
            Changed = true;
            OnChanged?.Invoke(Old, Current);
            return true;
        }

        return false;
    }

    public sealed override void Reset()
    {
        Old = Default;
        Current = Default;
        InitialUpdate = false;
    }

    public abstract override string ToString();

    protected string OffsetsToString()
    {
        StringBuilder sb = new();

        sb.Append($"0x{_base.ToString("X")}");

        foreach (int offset in _offsets)
        {
            sb.Append($", 0x{offset:X}");
        }

        return sb.ToString();
    }
}
