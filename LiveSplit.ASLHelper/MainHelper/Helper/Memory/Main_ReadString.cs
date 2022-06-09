using LiveSplit.ComponentUtil;
using System;

namespace ASLHelper
{
    public partial class Main
    {
        public string ReadString(int length, ReadStringType stringType, int baseOffset, params int[] offsets)
        {
            return ReadString(length, stringType, Game.MainModuleWow64Safe(), baseOffset, offsets);
        }

        public string ReadString(int length, ReadStringType stringType, string moduleName, int baseOffset, params int[] offsets)
        {
            return ReadString(length, stringType, GetModule(moduleName), baseOffset, offsets);
        }

        public string ReadString(int length, ReadStringType stringType, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets)
        {
            if (module == null)
                return default;

            return ReadString(length, stringType, module.BaseAddress + baseOffset, offsets);
        }

        public string ReadString(int length, ReadStringType stringType, IntPtr baseAddress, params int[] offsets)
        {
            var deref = Deref(baseAddress, offsets);
            return Game.ReadString(deref, stringType, length, "");
        }
    }
}