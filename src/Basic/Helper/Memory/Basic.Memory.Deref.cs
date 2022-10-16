using static AslHelp.MemUtils.WinAPI;

public partial class Basic
{
    #region Deref
    public nint Deref(int baseOffset, params int[] offsets)
    {
        TryDeref(out nint deref, MainModule, baseOffset, offsets);
        return deref;
    }

    public nint Deref(string module, int baseOffset, params int[] offsets)
    {
        TryDeref(out nint deref, Modules[module], baseOffset, offsets);
        return deref;
    }

    public nint Deref(Module module, int baseOffset, params int[] offsets)
    {
        TryDeref(out nint deref, module, baseOffset, offsets);
        return deref;
    }

    public nint Deref(nint baseAddress, params int[] offsets)
    {
        TryDeref(out nint deref, baseAddress, offsets);
        return deref;
    }
    #endregion

    #region TryDeref
    public bool TryDeref(out nint deref, int baseOffset, params int[] offsets)
    {
        return TryDeref(out deref, MainModule, baseOffset, offsets);
    }

    public bool TryDeref(out nint deref, string module, int baseOffset, params int[] offsets)
    {
        return TryDeref(out deref, Modules[module], baseOffset, offsets);
    }

    public bool TryDeref(out nint deref, Module module, int baseOffset, params int[] offsets)
    {
        if (module is null)
        {
            Debug.Warn("[Deref] Module could not be found.");

            deref = default;
            return false;
        }

        return TryDeref(out deref, module.Base + baseOffset, offsets);
    }

    public unsafe bool TryDeref(out nint deref, nint baseAddress, params int[] offsets)
    {
        if (Game is null)
        {
            Debug.Warn("[Deref] Game process was null.");
            deref = default;
            return false;
        }

        if (baseAddress == 0)
        {
            deref = default;
            return false;
        }

        deref = baseAddress;

        if (offsets.Length == 0)
        {
            return true;
        }

        fixed (nint* pDeref = &deref)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                if (!Game.Read(deref, pDeref, PtrSize))
                {
                    deref = default;
                }

                if (deref == default)
                {
                    return false;
                }

                deref += offsets[i];
            }

            return true;
        }
    }
    #endregion
}
