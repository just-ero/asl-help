using LiveSplit.ComponentUtil;
using System;

namespace ASLHelper
{
    public partial class Main
    {
        public T Read<T>(int baseOffset, params int[] offsets) where T : unmanaged
        {
            return Read<T>(Game.MainModuleWow64Safe(), baseOffset, offsets);
        }

        public T Read<T>(string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
        {
            return Read<T>(GetModule(moduleName), baseOffset, offsets);
        }

        public T Read<T>(ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
        {
            if (module == null)
                return default;

            return Read<T>(module.BaseAddress + baseOffset, offsets);
        }

        public T Read<T>(IntPtr baseAddress, params int[] offsets) where T : unmanaged
        {
            var deref = Deref(baseAddress, offsets);

            if (typeof(T) == typeof(IntPtr) || typeof(T) == typeof(UIntPtr))
            {
                return (T)(object)(Game.ReadPointer(deref));
            }

            return Game.ReadValue<T>(deref);
        }
    }
}