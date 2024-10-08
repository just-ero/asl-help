namespace AslHelp.Memory.Monitoring;

public sealed class TickCounter
{
    public TickCounter(uint tick)
    {
        Tick = tick;
    }

    public uint Tick { get; set; }

    public static implicit operator uint(TickCounter counter)
    {
        return counter.Tick;
    }

    public static TickCounter operator ++(TickCounter counter)
    {
        counter.Tick++;
        return counter;
    }

    public static TickCounter operator --(TickCounter counter)
    {
        counter.Tick--;
        return counter;
    }

    public static bool operator ==(TickCounter counter, uint tick)
    {
        return counter.Tick == tick;
    }

    public static bool operator !=(TickCounter counter, uint tick)
    {
        return counter.Tick != tick;
    }

    public static bool operator >(TickCounter counter, uint tick)
    {
        return counter.Tick > tick;
    }

    public static bool operator <(TickCounter counter, uint tick)
    {
        return counter.Tick < tick;
    }

    public static bool operator >=(TickCounter counter, uint tick)
    {
        return counter.Tick >= tick;
    }

    public static bool operator <=(TickCounter counter, uint tick)
    {
        return counter.Tick <= tick;
    }

    public override bool Equals(object obj)
    {
        return obj is TickCounter counter
            && counter.Tick == Tick;
    }

    public override int GetHashCode()
    {
        return Tick.GetHashCode();
    }
}
