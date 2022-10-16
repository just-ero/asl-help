using AslHelp.MemUtils.Definitions;
using System.Runtime.InteropServices;

using static AslHelp.MemUtils.WinAPI;

public partial class Basic
{
    #region ReadSpanCustom
    public dynamic[] ReadSpanCustom(TypeDefinition definition, int length, int baseOffset, params int[] offsets)
    {
        dynamic[] buffer = new dynamic[length];
        TryReadSpanCustom(definition, buffer, MainModule, baseOffset, offsets);

        return buffer;
    }

    public dynamic[] ReadSpanCustom(TypeDefinition definition, int length, string module, int baseOffset, params int[] offsets)
    {
        dynamic[] buffer = new dynamic[length];
        TryReadSpanCustom(definition, buffer, Modules[module], baseOffset, offsets);

        return buffer;
    }

    public dynamic[] ReadSpanCustom(TypeDefinition definition, int length, Module module, int baseOffset, params int[] offsets)
    {
        dynamic[] buffer = new dynamic[length];
        TryReadSpanCustom(definition, buffer, module, baseOffset, offsets);

        return buffer;
    }

    public dynamic[] ReadSpanCustom(TypeDefinition definition, int length, nint baseAddress, params int[] offsets)
    {
        dynamic[] buffer = new dynamic[length];
        TryReadSpanCustom(definition, buffer, baseAddress, offsets);

        return buffer;
    }
    #endregion

    #region TryReadSpanCustom
    public bool TryReadSpanCustom(TypeDefinition definition, dynamic[] buffer, int baseOffset, params int[] offsets)
    {
        return TryReadSpanCustom(definition, buffer, MainModule, baseOffset, offsets);
    }

    public bool TryReadSpanCustom(TypeDefinition definition, dynamic[] buffer, string module, int baseOffset, params int[] offsets)
    {
        return TryReadSpanCustom(definition, buffer, Modules[module], baseOffset, offsets);
    }

    public bool TryReadSpanCustom(TypeDefinition definition, dynamic[] buffer, Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            Debug.Warn($"[ReadSpanCustom] Module could not be found.");

            buffer = Array.Empty<dynamic>();
            return false;
        }

        return TryReadSpanCustom(definition, buffer, module.Base + baseOffset, offsets);
    }

    public bool TryReadSpanCustom(TypeDefinition definition, dynamic[] buffer, nint baseAddress, params int[] offsets)
    {
        return ReadSpanCustom_Internal(definition, buffer, baseAddress, offsets);
    }
    #endregion

    internal unsafe bool ReadSpanCustom_Internal(TypeDefinition definition, dynamic[] buffer, nint baseAddress, params int[] offsets)
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            return false;
        }

        int defSize = definition.Size, bufSize = buffer.Length;

        byte* bytes = stackalloc byte[defSize * bufSize];

        if (!Game.Read(deref, bytes, defSize * bufSize))
        {
            return false;
        }

        for (int i = 0; i < bufSize; i++)
        {
            buffer[i] = Marshal.PtrToStructure((nint)buffer[i * defSize], definition.Type);
        }

        return true;
    }
}
