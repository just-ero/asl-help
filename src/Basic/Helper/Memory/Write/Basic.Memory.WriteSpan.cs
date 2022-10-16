using System.Runtime.InteropServices;
using static AslHelp.MemUtils.WinAPI;

public partial class Basic
{
    public bool WriteSpan<T>(IList<T> values, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return WriteSpan<T>(values, MainModule, baseOffset, offsets);
    }

    public bool WriteSpan<T>(IList<T> values, string module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        return WriteSpan<T>(values, Modules[module], baseOffset, offsets);
    }

    public bool WriteSpan<T>(IList<T> values, Module module, int baseOffset, params int[] offsets) where T : unmanaged
    {
        if (module is null)
        {
            Debug.Warn("[Write] Module could not be found.");

            return false;
        }

        return WriteSpan<T>(values, module.Base + baseOffset, offsets);
    }

    public unsafe bool WriteSpan<T>(IList<T> values, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (values is not T[] valuesArray)
        {
            valuesArray = values.ToArray();
        }

        return WriteSpan_Internal<T>(valuesArray, baseAddress, offsets);
    }

    public unsafe bool WriteSpan_Internal<T>(Span<T> data, nint baseAddress, params int[] offsets) where T : unmanaged
    {
        if (!TryDeref(out nint deref, baseAddress, offsets))
        {
            return false;
        }

        if (!Is64Bit && IsPointerType<T>())
        {
            Span<uint> data32 = MemoryMarshal.Cast<T, uint>(data);
            return WriteSpan_Internal<uint>(data32, deref);
        }

        fixed (T* pData = data)
        {
            return Game.Write(deref, pData, GetTypeSize<T>(Is64Bit) * data.Length);
        }
    }
}
