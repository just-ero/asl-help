using System.Runtime.InteropServices;
using System.Security;

namespace AslHelp.Memory.Win32;

internal static unsafe partial class PInvoke
{
    /// <summary>
    ///     Copies the data in the specified address range from the address space of
    ///     the specified process into the specified buffer of the current process.<br/>
    ///     For further information, see:
    ///     <i><see href="https://docs.microsoft.com/windows/win32/api/memoryapi/nf-memoryapi-readprocessmemory">
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
        SafeProcessHandle processHandle,
        nuint baseAddress,
        void* buffer,
        nuint bufferSize,
        out nuint numberOfBytesRead)
    {
        fixed (nuint* pNumberOfBytesRead = &numberOfBytesRead)
        {
            return ReadProcessMemory(processHandle, (void*)baseAddress, buffer, bufferSize, pNumberOfBytesRead) != 0;
        }

        [DllImport(Kernel32, EntryPoint = nameof(ReadProcessMemory), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int ReadProcessMemory(
            SafeProcessHandle hProcess,
            void* lpBaseAddress,
            void* lpBuffer,
            nuint nSize,
            nuint* lpNumberOfBytesRead);
    }

    /// <summary>
    ///     Writes data to an area of memory in a specified process.<br/>
    ///     For further information, see:
    ///     <i><see href="https://docs.microsoft.com/windows/win32/api/memoryapi/nf-memoryapi-writeprocessmemory">
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
        SafeProcessHandle processHandle,
        nuint baseAddress,
        void* buffer,
        nuint bufferSize,
        out nuint numberOfBytesWritten)
    {
        fixed (nuint* pNumberOfBytesWritten = &numberOfBytesWritten)
        {
            return WriteProcessMemory(processHandle, (void*)baseAddress, buffer, bufferSize, pNumberOfBytesWritten) != 0;
        }

        [DllImport(Kernel32, EntryPoint = nameof(WriteProcessMemory), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int WriteProcessMemory(
            SafeProcessHandle hProcess,
            void* lpBaseAddress,
            void* lpBuffer,
            nuint nSize,
            nuint* lpNumberOfBytesWritten);
    }

    /// <summary>
    ///     Retrieves information about a range of pages within the virtual address space of a specified process.<br/>
    ///     For further information, see:
    ///     <i><see href="https://docs.microsoft.com/windows/win32/api/memoryapi/nf-memoryapi-virtualqueryex">
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
    ///     otherwise, <c>0</c>.
    /// </returns>
    public static nuint VirtualQuery(SafeProcessHandle processHandle, nuint baseAddress, out MemoryBasicInformation mbi)
    {
        fixed (MemoryBasicInformation* pMbi = &mbi)
        {
            return VirtualQueryEx(processHandle, (void*)baseAddress, pMbi, (nuint)sizeof(MemoryBasicInformation));
        }

        [DllImport(Kernel32, EntryPoint = nameof(VirtualQueryEx), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern nuint VirtualQueryEx(
            SafeProcessHandle hProcess,
            void* lpAddress,
            MemoryBasicInformation* lpBuffer,
            nuint dwLength);
    }

    /// <summary>
    ///     Reserves, commits, or changes the state of a region of memory within the virtual address space of a specified process.<br/>
    ///     For further information see:
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
    public static nuint VirtualAlloc(
        SafeProcessHandle processHandle,
        nuint baseAddress,
        uint size,
        MemoryPageState allocationType,
        MemoryPageProtect memoryProtection)
    {
        return VirtualAllocEx(processHandle, (void*)baseAddress, size, (uint)allocationType, (uint)memoryProtection);

        [DllImport(Kernel32, EntryPoint = nameof(VirtualAllocEx), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern nuint VirtualAllocEx(
            SafeProcessHandle hProcess,
            void* lpAddress,
            nuint dwSize,
            uint flAllocationType,
            uint flProtect);
    }

    /// <summary>
    ///     Releases, decommits, or releases and decommits a region of memory within the virtual address space of a specified process.<br/>
    ///     For further information see:
    ///     <i><see href="https://docs.microsoft.com/windows/win32/api/memoryapi/nf-memoryapi-virtualfreeex">
    ///         VirtualFreeEx function (memoryapi.h)
    ///     </see></i>
    /// </summary>
    /// <param name="processHandle">
    ///     A handle to the process within which the memory should be freed.
    /// </param>
    /// <param name="baseAddress">
    ///     The starting address of the region of memory to be freed.
    /// </param>
    /// <param name="size">
    ///     The size of the region of memory to free, in bytes.
    /// </param>
    /// <param name="freeType">
    ///     The type of free operation.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the function succeeds;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool VirtualFree(
        SafeProcessHandle processHandle,
        nuint baseAddress,
        uint size,
        MemoryPageState freeType)
    {
        return VirtualFreeEx(processHandle, (void*)baseAddress, size, (uint)freeType) != 0;

        [DllImport(Kernel32, EntryPoint = nameof(VirtualFreeEx), ExactSpelling = true, SetLastError = true)]
        [SuppressUnmanagedCodeSecurity]
        static extern int VirtualFreeEx(
            SafeProcessHandle hProcess,
            void* lpAddress,
            nuint dwSize,
            uint dwFreeType);
    }
}
