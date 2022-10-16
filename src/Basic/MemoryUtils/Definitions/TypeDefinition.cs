using System;
using System.Runtime.InteropServices;

namespace AslHelp.MemUtils.Definitions;

public class TypeDefinition
{
    public TypeDefinition(Type type)
    {
        Type = type;
        Size = Marshal.SizeOf(type);
        Default = Activator.CreateInstance(type);
    }

    public Type Type { get; }
    public int Size { get; }

    public dynamic Default { get; }
}
