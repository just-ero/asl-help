namespace AslHelp.MemUtils.Models;

public record DebugSymbol
{
    internal unsafe DebugSymbol(SYMBOL_INFOW symbol)
    {
        Name = new((char*)symbol.Name);
        Address = (nint)symbol.Address;
        Size = (int)symbol.Size;
    }

    public string Name { get; }
    public nint Address { get; }
    public int Size { get; }

    public override string ToString()
    {
        return Name;
    }
}
