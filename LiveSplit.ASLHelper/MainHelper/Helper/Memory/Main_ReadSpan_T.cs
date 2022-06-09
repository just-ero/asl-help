using LiveSplit.ComponentUtil;
using System;

namespace ASLHelper
{
    public partial class Main
    {
        #region ReadSpan<T>
        public T[] ReadSpan<T>(int length, int baseOffset, params int[] offsets) where T : unmanaged
        {
            T[] buffer = new T[length];
            TryReadSpan<T>(buffer, Game?.MainModuleWow64Safe(), baseOffset, offsets);
            return buffer;
        }

        public T[] ReadSpan<T>(int length, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
        {
            T[] buffer = new T[length];
            TryReadSpan<T>(buffer, GetModule(moduleName), baseOffset, offsets);
            return buffer;
        }

        public T[] ReadSpan<T>(int length, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
        {
            T[] buffer = new T[length];
            TryReadSpan<T>(buffer, module, baseOffset, offsets);
            return buffer;
        }

        public T[] ReadSpan<T>(int length, IntPtr baseAddress, params int[] offsets) where T : unmanaged
        {
            T[] buffer = new T[length];
            TryReadSpan<T>(buffer, baseAddress, offsets);
            return buffer;
        }
        #endregion

        #region TryReadSpan<T>
        public bool TryReadSpan<T>(T[] buffer, int baseOffset, params int[] offsets) where T : unmanaged
        {
            return TryReadSpan<T>(buffer, Game?.MainModuleWow64Safe(), baseOffset, offsets);
        }

        public bool TryReadSpan<T>(T[] buffer, string moduleName, int baseOffset, params int[] offsets) where T : unmanaged
        {
            return TryReadSpan<T>(buffer, GetModule(moduleName), baseOffset, offsets);
        }

        public bool TryReadSpan<T>(T[] buffer, ProcessModuleWow64Safe module, int baseOffset, params int[] offsets) where T : unmanaged
        {
            if (module == null)
            {
                Debug.Warn("[Read] Module could not be found!");

                buffer = Array.Empty<T>();
                return false;
            }


            return TryReadSpan<T>(buffer, module.BaseAddress + baseOffset, offsets);
        }

        public unsafe bool TryReadSpan<T>(T[] buffer, IntPtr baseAddress, params int[] offsets) where T : unmanaged
        {
            var deref = Deref(baseAddress, offsets);
            if (deref == IntPtr.Zero)
                return false;

            if (!Is64Bit && IsPointerType<T>())
            {
                var buf32 = new uint[buffer.Length];
                if (!TryReadSpan<uint>(buf32, deref))
                    return false;

                for (int i = 0; i < buf32.Length; i++)
                {
                    fixed (uint* pBuf = &buf32[i])
                    {
                        buffer[i] = *(T*)pBuf;
                    }
                }

                return true;
            }

            fixed (T* pBuffer = buffer)
            {
                return Read(pBuffer, GetTypeSize<T>(), deref);
            }
        }
        #endregion
    }
}