using AslHelp.MemUtils.Exceptions;
using AslHelp.MemUtils.Pointers;

namespace AslHelp.Mono.Models;

public partial class MonoClass
{
    public Pointer<T> Make<T>(string staticField, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {Name} was 0.");
        }

        return new(Static + this[staticField], offsets);
    }

    public Pointer<T> Make<T>(string staticField, string nextField, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {Name} was 0.");
        }

        return new(Static + this[staticField], offsets.Prepend(this[nextField]).ToArray());
    }

    public UnityStringPointer MakeString(string staticField, params int[] offsets)
    {
        if (Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {Name} was 0.");
        }

        return new(Static + this[staticField], offsets);
    }

    public UnityStringPointer MakeString(string staticField, string nextField, params int[] offsets)
    {
        if (Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {Name} was 0.");
        }

        return new(Static + this[staticField], offsets.Prepend(this[nextField]).ToArray());
    }

    public ArrayPointer<T> MakeArray<T>(string staticField, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {Name} was 0.");
        }

        return new(Static + this[staticField], offsets);
    }

    public ArrayPointer<T> MakeArray<T>(string staticField, string nextField, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {Name} was 0.");
        }

        return new(Static + this[staticField], offsets.Prepend(this[nextField]).ToArray());
    }

    public ListPointer<T> MakeList<T>(string staticField, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {Name} was 0.");
        }

        return new(Static + this[staticField], offsets);
    }

    public ListPointer<T> MakeList<T>(string staticField, string nextField, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
        {
            throw new InvalidAddressException($"Address to static fields for class {Name} was 0.");
        }

        return new(Static + this[staticField], offsets.Prepend(this[nextField]).ToArray());
    }
}
