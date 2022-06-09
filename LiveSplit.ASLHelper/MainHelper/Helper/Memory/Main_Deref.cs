using LiveSplit.ComponentUtil;
using System;

namespace ASLHelper
{
    public partial class Main
    {
        #region Deref
        public IntPtr Deref(int baseOffset, params int[] offsets)
        {
            TryDeref(out var deref, Game?.MainModuleWow64Safe(), baseOffset, offsets);
            return deref;
        }

        public IntPtr Deref(string moduleName, int baseOffset, params int[] offsets)
        {
            TryDeref(out var deref, GetModule(moduleName), baseOffset, offsets);
            return deref;
        }

        public IntPtr Deref(ProcessModuleWow64Safe module, int baseOffset, params int[] offsets)
        {
            TryDeref(out var deref, module, baseOffset, offsets);
            return deref;
        }

        public IntPtr Deref(IntPtr baseAddress, params int[] offsets)
        {
            TryDeref(out var deref, baseAddress, offsets);
            return deref;
        }
        #endregion

        #region TryDeref
        public bool TryDeref(out IntPtr deref, int baseOffset, params int[] offsets)
        {
            return TryDeref(out deref, Game?.MainModuleWow64Safe(), baseOffset, offsets);
        }

        public bool TryDeref(out IntPtr deref, string moduleName, int baseOffset, params int[] offsets)
        {
            return TryDeref(out deref, GetModule(moduleName), baseOffset, offsets);
        }

        public bool TryDeref(out IntPtr deref, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets)
        {
            if (module == null)
            {
                Debug.Warn("[Deref] Module could not be found!");

                deref = default;
                return false;
            }

            return TryDeref(out deref, module.BaseAddress + baseOffset, offsets);
        }

        public unsafe bool TryDeref(out IntPtr deref, IntPtr baseAddress, params int[] offsets)
        {
            if (baseAddress == IntPtr.Zero)
            {
                deref = default;
                return false;
            }

            deref = baseAddress;

            if (offsets.Length == 0)
                return true;

            fixed (IntPtr* pDeref = &deref)
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
}