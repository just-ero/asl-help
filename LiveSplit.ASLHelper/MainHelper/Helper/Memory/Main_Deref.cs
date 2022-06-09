using LiveSplit.ComponentUtil;
using System;

namespace ASLHelper
{
    public partial class Main
    {
		public IntPtr Deref(int baseOffset, params int[] offsets)
        {
			return Deref(Game.MainModuleWow64Safe(), baseOffset, offsets);
        }

		public IntPtr Deref(string moduleName, int baseOffset, params int[] offsets)
        {
			return Deref(GetModule(moduleName), baseOffset, offsets);
        }

		public IntPtr Deref(ProcessModuleWow64Safe module, int baseOffset, params int[] offsets)
        {
			if (module == null)
				return IntPtr.Zero;

			return Deref(module.BaseAddress + baseOffset, offsets);
        }

        public IntPtr Deref(IntPtr baseAddress, params int[] offsets)
		{
			if (offsets.Length == 0)
				return baseAddress;

			for (int i = 0; i < offsets.Length; ++i)
			{
				baseAddress = Game.ReadPointer(baseAddress);

				if (baseAddress == IntPtr.Zero)
					return IntPtr.Zero;

				baseAddress += offsets[i];
			}

			return baseAddress;
		}
    }
}