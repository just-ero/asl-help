using AslHelp.MemUtils.Definitions;
using System.Runtime.InteropServices;

using static AslHelp.MemUtils.WinAPI;

public partial class Basic
{
    #region ReadCustom
    public dynamic ReadCustom(TypeDefinition definition, int baseOffset, params int[] offsets)
    {
        TryReadCustom(definition, out dynamic result, baseOffset, offsets);
        return result;
    }

    public dynamic ReadCustom(TypeDefinition definition, string module, int baseOffset, params int[] offsets)
    {
        TryReadCustom(definition, out dynamic result, module, baseOffset, offsets);
        return result;
    }

    public dynamic ReadCustom(TypeDefinition definition, Module module, int baseOffset, params int[] offsets)
    {
        TryReadCustom(definition, out dynamic result, module, baseOffset, offsets);
        return result;
    }

    public dynamic ReadCustom(TypeDefinition definition, nint baseAddress, params int[] offsets)
    {
        TryReadCustom(definition, out dynamic result, baseAddress, offsets);
        return result;
    }
    #endregion

    #region TryReadCustom
    public bool TryReadCustom(TypeDefinition definition, out dynamic result, int baseOffset, params int[] offsets)
    {
        return TryReadCustom(definition, out result, MainModule, baseOffset, offsets);
    }

    public bool TryReadCustom(TypeDefinition definition, out dynamic result, string module, int baseOffset, params int[] offsets)
    {
        return TryReadCustom(definition, out result, Modules[module], baseOffset, offsets);
    }

    public bool TryReadCustom(TypeDefinition definition, out dynamic result, Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            Debug.Warn($"[ReadCustom] Module could not be found.");

            result = default;
            return false;
        }

        return TryReadCustom(definition, out result, module.Base + baseOffset, offsets);
    }

    public unsafe bool TryReadCustom(TypeDefinition definition, out dynamic result, nint baseAddress, params int[] offsets)
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            result = default;
            return false;
        }

        int size = definition.Size;

        byte* buffer = stackalloc byte[size];

        if (!Game.Read(deref, buffer, size))
        {
            result = definition.Default;
            return false;
        }

        result = Marshal.PtrToStructure((nint)buffer, definition.Type);

        return true;
    }
    #endregion
}
