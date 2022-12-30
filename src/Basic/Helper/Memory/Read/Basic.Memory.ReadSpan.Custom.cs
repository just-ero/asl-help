using AslHelp.MemUtils.Definitions;
using System.Runtime.InteropServices;

using static AslHelp.MemUtils.WinAPI;

public partial class Basic
{
    #region ReadSpanCustom
    public dynamic[] ReadSpanCustom(TypeDefinition definition, int length, int baseOffset, params int[] offsets)
    {
        dynamic[] items = new dynamic[length];
        TryReadSpanCustom(definition, items, MainModule, baseOffset, offsets);

        return items;
    }

    public dynamic[] ReadSpanCustom(TypeDefinition definition, int length, string module, int baseOffset, params int[] offsets)
    {
        dynamic[] items = new dynamic[length];
        TryReadSpanCustom(definition, items, Modules[module], baseOffset, offsets);

        return items;
    }

    public dynamic[] ReadSpanCustom(TypeDefinition definition, int length, Module module, int baseOffset, params int[] offsets)
    {
        dynamic[] items = new dynamic[length];
        TryReadSpanCustom(definition, items, module, baseOffset, offsets);

        return items;
    }

    public dynamic[] ReadSpanCustom(TypeDefinition definition, int length, nint baseAddress, params int[] offsets)
    {
        dynamic[] items = new dynamic[length];
        TryReadSpanCustom(definition, items, baseAddress, offsets);

        return items;
    }
    #endregion

    #region TryReadSpanCustom
    public bool TryReadSpanCustom(TypeDefinition definition, dynamic[] items, int baseOffset, params int[] offsets)
    {
        return TryReadSpanCustom(definition, items, MainModule, baseOffset, offsets);
    }

    public bool TryReadSpanCustom(TypeDefinition definition, dynamic[] items, string module, int baseOffset, params int[] offsets)
    {
        return TryReadSpanCustom(definition, items, Modules[module], baseOffset, offsets);
    }

    public bool TryReadSpanCustom(TypeDefinition definition, dynamic[] items, Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            Debug.Warn($"[ReadSpanCustom] Module could not be found.");

            items = Array.Empty<dynamic>();
            return false;
        }

        return TryReadSpanCustom(definition, items, module.Base + baseOffset, offsets);
    }

    public bool TryReadSpanCustom(TypeDefinition definition, dynamic[] items, nint baseAddress, params int[] offsets)
    {
        return ReadSpanCustom_Internal(definition, items, baseAddress, offsets);
    }
    #endregion

    internal unsafe bool ReadSpanCustom_Internal(TypeDefinition definition, dynamic[] items, nint baseAddress, params int[] offsets)
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            return false;
        }

        int size = definition.Size, itemCount = items.Length;

        byte* buffer = stackalloc byte[size * itemCount];

        if (!Game.Read(deref, buffer, size * itemCount))
        {
            return false;
        }

        for (int i = 0; i < itemCount; i++)
        {
            items[i] = Marshal.PtrToStructure((nint)(buffer + (i * size)), definition.Type);
        }

        return true;
    }
}
