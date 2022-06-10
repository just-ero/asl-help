using System.Runtime.InteropServices;

namespace ASLHelper;

public partial class Main
{
    [DllImport("kernel32")]
    private static extern unsafe int ReadProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        void* lpBuffer,
        UIntPtr nSize,
        UIntPtr* lpNumberOfBytesRead);

    [DllImport("kernel32")]
    private static extern unsafe int WriteProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        void* lpBuffer,
        UIntPtr nSize,
        UIntPtr* lpNumberOfBytesWritten);

    private static bool IsPointerType<T>() where T : unmanaged
    {
        return typeof(T) == typeof(IntPtr) || typeof(T) == typeof(UIntPtr);
    }

    private unsafe int GetTypeSize<T>() where T : unmanaged
    {
        return IsPointerType<T>() ? (Is64Bit ? 0x8 : 0x4) : sizeof(T);
    }

    private unsafe bool Read(void* buffer, int bufferLength, IntPtr address)
    {
        if (Game == null)
            return false;

        var len = (UIntPtr)bufferLength;

        UIntPtr bytesRead;
        return ReadProcessMemory(Game.Handle, address, buffer, len, &bytesRead) != 0
               && bytesRead == len;
    }
}