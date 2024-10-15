using System.Runtime.InteropServices;
using System.Security;

using AslHelp.Memory.Native.Enums;
using AslHelp.Memory.Native.Structs;
using AslHelp.Memory.Utils;

namespace AslHelp.Memory.Native;

internal static unsafe partial class WinInterop
{
    /// <summary>
    ///     Copies the data in the specified address range from the address space of
    ///     the specified process into the specified buffer of the current process.<br/>
    ///     For further information, see:
    ///     <i><see href="https://docs.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-readprocessmemory">
    ///         ReadProcessMemory function (memoryapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process with the memory that is being read.
    /// </param>
    /// <param name="baseAddress">
    ///     A pointer to the base address in the specified process from which to read.
    /// </param>
    /// <param name="buffer">
    ///     A pointer to a buffer that receives the contents from the address space of the specified process.
    /// </param>
    /// <param name="bufferSize">
    ///     The number of bytes to be read from the specified process.
    /// </param>
    /// <param name="numberOfBytesRead">
    ///     The number of bytes successfully transferred into the specified buffer.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool ReadProcessMemory(
        nint processHandle,
        nint baseAddress,
        void* buffer,
        nint bufferSize,
        out nint numberOfBytesRead)
    {
        fixed (nint* pNumberOfBytesRead = &numberOfBytesRead)
        {
            return ReadProcessMemory((void*)processHandle, (void*)baseAddress, buffer, bufferSize, pNumberOfBytesRead) != 0;
        }

        [DllImport(Lib.Kernel32, EntryPoint = nameof(ReadProcessMemory), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int ReadProcessMemory(
            void* hProcess,
            void* lpBaseAddress,
            void* lpBuffer,
            nint nSize,
            nint* lpNumberOfBytesRead);
    }

    /// <summary>
    ///     Writes data to an area of memory in a specified process.<br/>
    ///     For further information, see:
    ///     <i><see href="https://docs.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-writeprocessmemory">
    ///         WriteProcessMemory function (memoryapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process memory to be modified.
    /// </param>
    /// <param name="baseAddress">
    ///     A pointer to the base address in the specified process to which data is written.
    /// </param>
    /// <param name="buffer">
    ///     A pointer to the buffer that contains data to be written in the address space of the specified process.
    /// </param>
    /// <param name="bufferSize">
    ///     The number of bytes to be written to the specified process.
    /// </param>
    /// <param name="numberOfBytesWritten">
    ///     The number of bytes successfully written to the specified process.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool WriteProcessMemory(
        nint processHandle,
        nint baseAddress,
        void* buffer,
        nint bufferSize,
        out nint numberOfBytesWritten)
    {
        fixed (nint* pNumberOfBytesWritten = &numberOfBytesWritten)
        {
            return WriteProcessMemory((void*)processHandle, (void*)baseAddress, buffer, bufferSize, pNumberOfBytesWritten) != 0;
        }

        [DllImport(Lib.Kernel32, EntryPoint = nameof(WriteProcessMemory), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int WriteProcessMemory(
            void* hProcess,
            void* lpBaseAddress,
            void* lpBuffer,
            nint nSize,
            nint* lpNumberOfBytesWritten);
    }

    /// <summary>
    ///     Retrieves information about a range of pages within the virtual address space of a specified process.<br/>
    ///     For further information, see:
    ///     <i><see href="https://docs.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-virtualqueryex">
    ///         VirtualQueryEx function (memoryapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process whose memory information is queried.
    /// </param>
    /// <param name="baseAddress">
    ///     A pointer to the base address of the region of pages to be queried.
    /// </param>
    /// <param name="mbi">
    ///     The <see cref="MemoryBasicInformation"/> structure in which information about the specified page range is returned.
    /// </param>
    /// <returns>
    ///     The actual number of bytes returned in the information buffer if the function succeeds;
    ///     otherwise, 0.
    /// </returns>
    public static nint VirtualQuery(nint processHandle, nint baseAddress, out MemoryBasicInformation mbi)
    {
        fixed (MemoryBasicInformation* pMbi = &mbi)
        {
            return VirtualQueryEx((void*)processHandle, (void*)baseAddress, pMbi, sizeof(MemoryBasicInformation));
        }

        [DllImport(Lib.Kernel32, EntryPoint = nameof(VirtualQueryEx), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern nint VirtualQueryEx(
            void* hProcess,
            void* lpAddress,
            MemoryBasicInformation* lpBuffer,
            nint dwLength);
    }

    /// <summary>
    ///     Reserves, commits, or changes the state of a region of memory within the virtual address space of a specified process.<br/>
    ///     For further information, see:
    ///     <i><see href="https://learn.microsoft.com/windows/win32/api/memoryapi/nf-memoryapi-virtualallocex">
    ///         VirtualAllocEx function (memoryapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process within which the memory should be allocated.
    /// </param>
    /// <param name="baseAddress">
    ///     The desired starting address for the region of memory to be allocated.
    /// </param>
    /// <param name="size">
    ///     The size of the region of memory to allocate, in bytes.
    /// </param>
    /// <param name="allocationType">
    ///     The type of memory allocation.
    /// </param>
    /// <param name="memoryProtection">
    ///     The memory protection for the region of pages to be allocated.
    /// </param>
    /// <returns>
    ///     The base address of the allocated region of pages if the function succeeds;
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    public static nint VirtualAlloc(
        nint processHandle,
        nint baseAddress,
        nint size,
        MemoryRangeState allocationType,
        MemoryRangeProtect memoryProtection)
    {
        return VirtualAllocEx((void*)processHandle, (void*)baseAddress, size, (uint)allocationType, (uint)memoryProtection);

        [DllImport(Lib.Kernel32, EntryPoint = nameof(VirtualAllocEx), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern nint VirtualAllocEx(
            void* hProcess,
            void* lpAddress,
            nint dwSize,
            uint flAllocationType,
            uint flProtect);
    }

    /// <summary>
    ///     Releases, decommits, or releases and decommits a region of memory within the virtual address space of a specified process.<br/>
    ///     For further information, see:
    ///     <i><see href="https://docs.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-virtualfreeex">
    ///         VirtualFreeEx function (memoryapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="hProcess">
    ///     A handle to the process within which the memory should be freed.
    /// </param>
    /// <param name="lpAddress">
    ///     The starting address of the region of memory to be freed.
    /// </param>
    /// <param name="dwSize">
    ///     The size of the region of memory to free, in bytes.
    /// </param>
    /// <param name="dwFreeType">
    ///     The type of free operation.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool VirtualFree(
        nint processHandle,
        nint baseAddress,
        int size,
        MemoryRangeState freeType)
    {
        return VirtualFreeEx((void*)processHandle, (void*)baseAddress, size, (uint)freeType) != 0;

        [DllImport(Lib.Kernel32, EntryPoint = nameof(VirtualFreeEx), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int VirtualFreeEx(
            void* hProcess,
            void* lpAddress,
            nint dwSize,
            uint dwFreeType);
    }
}
