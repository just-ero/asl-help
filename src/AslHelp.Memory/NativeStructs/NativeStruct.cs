using System.Text;

using AslHelp.Collections;

namespace AslHelp.Memory.NativeStructs;

public sealed class NativeStruct : OrderedDictionary<string, NativeField>
{
    internal NativeStruct(string name, string? super = null)
    {
        Name = name;
        Super = super;
    }

    public string Name { get; }
    public string? Super { get; }

    public uint Size => this[^1].Offset + this[^1].Size;
    public uint Alignment => this[0].Alignment;

    public uint SelfAlignedSize => NativeStructCollectionInitializer.Align(Size, Alignment);

    protected override string GetKeyForItem(NativeField item)
    {
        return item.Name;
    }

    public sealed override string ToString()
    {
        StringBuilder sb = new();

        if (Super is null)
        {
            sb.AppendLine($"{Name} (0x{SelfAlignedSize:X3})");
        }
        else
        {
            sb.AppendLine($"{Name} : {Super} (0x{SelfAlignedSize:X3})");
        }

        foreach (NativeField field in this)
        {
            sb.AppendLine($"    {field}");
        }

        return sb.ToString().TrimEnd();
    }
}
