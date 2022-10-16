using static AslHelp.MemUtils.WinAPI;

public partial class Basic
{
    public bool Write<T>(T value, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return Write<T>(value, MainModule, baseOffset, offsets);
    }

    public bool Write<T>(T value, string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return Write<T>(value, Modules[module], baseOffset, offsets);
    }

    public bool Write<T>(T value, Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            Debug.Warn("[Write] Module could not be found.");

            return false;
        }

        return Write<T>(value, module.Base + baseOffset, offsets);
    }

    public unsafe bool Write<T>(T value, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            return false;
        }

        return Game.Write(deref, &value, GetTypeSize<T>(Is64Bit));
    }
}
