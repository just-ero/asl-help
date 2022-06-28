namespace ASLHelper;

public partial class Main
{
    #region Deref
    public nint Deref(int baseOffset, params int[] offsets)
    {
        _ = TryDeref(out var deref, Game?.MainModuleWow64Safe(), baseOffset, offsets);
        return deref;
    }

    public nint Deref(string moduleName, int baseOffset, params int[] offsets)
    {
        _ = TryDeref(out var deref, GetModule(moduleName), baseOffset, offsets);
        return deref;
    }

    public nint Deref(ProcessModuleWow64Safe module, int baseOffset, params int[] offsets)
    {
        _ = TryDeref(out var deref, module, baseOffset, offsets);
        return deref;
    }

    public nint Deref(nint baseAddress, params int[] offsets)
    {
        _ = TryDeref(out var deref, baseAddress, offsets);
        return deref;
    }
    #endregion

    #region TryDeref
    public bool TryDeref(out nint deref, int baseOffset, params int[] offsets)
    {
        return TryDeref(out deref, Game?.MainModuleWow64Safe(), baseOffset, offsets);
    }

    public bool TryDeref(out nint deref, string moduleName, int baseOffset, params int[] offsets)
    {
        return TryDeref(out deref, GetModule(moduleName), baseOffset, offsets);
    }

    public bool TryDeref(out nint deref, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            Debug.Warn("[Deref] Module could not be found!");

            deref = default;
            return false;
        }

        return TryDeref(out deref, module.BaseAddress + baseOffset, offsets);
    }

    public unsafe bool TryDeref(out nint deref, nint baseAddress, params int[] offsets)
    {
        if (baseAddress == 0)
        {
            deref = default;
            return false;
        }

        deref = baseAddress;

        if (offsets.Length == 0)
            return true;

        fixed (nint* pDeref = &deref)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                if (!Read(pDeref, Is64Bit ? 0x8 : 0x4, deref))
                {
                    deref = default;
                    return false;
                }

                if (deref == default)
                    return false;

                deref += offsets[i];
            }

            return true;
        }
    }
    #endregion
}
