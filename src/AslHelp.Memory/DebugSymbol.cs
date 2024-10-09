using System;

using AslHelp.Memory.Native.Structs;

namespace AslHelp.Memory;

public readonly struct DebugSymbol
{
    public DebugSymbol(string name, nint address, uint size)
    {
        Name = name;
        Address = address;
        Size = size;
    }

    internal unsafe DebugSymbol(SymbolInfo symbol)
    {
        Name = new Span<char>(symbol.Name, (int)symbol.NameLength).ToString();
        Address = (nint)symbol.Address;
        Size = symbol.Size;
    }

    public string Name { get; }
    public nint Address { get; }
    public uint Size { get; }

    public override string ToString()
    {
        return $"{nameof(DebugSymbol)} {{ {nameof(Name)} = {Name}, {nameof(Address)} = 0x{(ulong)Address:X}, {nameof(Size)} = 0x{Size:X} }}";
    }
}
