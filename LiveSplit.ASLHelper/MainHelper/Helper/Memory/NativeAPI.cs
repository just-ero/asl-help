using System.Runtime.InteropServices;

namespace ASLHelper;

public partial class Main
{
    [DllImport("kernel32")]
    private static extern unsafe int ReadProcessMemory(
        nint hProcess,
        nint lpBaseAddress,
        void* lpBuffer,
        nuint nSize,
        nuint* lpNumberOfBytesRead);

    [DllImport("kernel32")]
    private static extern unsafe int WriteProcessMemory(
        nint hProcess,
        nint lpBaseAddress,
        void* lpBuffer,
        nuint nSize,
        nuint* lpNumberOfBytesWritten);

    private static bool IsPointerType<T>() where T : unmanaged
    {
        return typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(IntPtr) || typeof(T) == typeof(UIntPtr);
    }

    private unsafe int GetTypeSize<T>() where T : unmanaged
    {
        return IsPointerType<T>() ? (Is64Bit ? 0x8 : 0x4) : sizeof(T);
    }

    private unsafe bool Read(void* buffer, int bufferLength, nint address)
    {
        if (Game is null || Game.HasExited)
            return false;

        var len = (nuint)bufferLength;

        nuint bytesRead;
        return ReadProcessMemory(Game.Handle, address, buffer, len, &bytesRead) != 0
               && bytesRead == len;
    }
}
