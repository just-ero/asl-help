using System;

namespace AslHelp.Memory.Native.Enums;

/// <summary>
///     Provides process-specific access rights.
/// </summary>
/// <remarks>
///     For further information, see:
///     <i><see href="https://learn.microsoft.com/en-us/windows/win32/procthread/process-security-and-access-rights">Process Security and Access Rights</see></i>
/// </remarks>
[Flags]
internal enum ProcessAccess : uint
{
    /// <summary>
    ///     Required to terminate a process.
    /// </summary>
    Terminate = 0x0001,

    /// <summary>
    ///     Required to create a thread in the process.
    /// </summary>
    CreateThread = 0x0002,

    /// <summary>
    ///
    /// </summary>
    SetSessionId = 0x0004,

    /// <summary>
    ///     Required to perform an operation on the address space of a process.
    /// </summary>
    VmOperation = 0x0008,

    /// <summary>
    ///     Required to read memory in a process.
    /// </summary>
    VmRead = 0x0010,

    /// <summary>
    ///     Required to write to memory in a process.
    /// </summary>
    VmWrite = 0x0020,

    /// <summary>
    ///     Required to duplicate a handle.
    /// </summary>
    DuplicateHandle = 0x0040,

    /// <summary>
    ///     Required to use this process as the parent process.
    /// </summary>
    CreateProcess = 0x0080,

    /// <summary>
    ///     Required to set memory limits.
    /// </summary>
    SetQuota = 0x0100,

    /// <summary>
    ///     Required to set certain information about a process.
    /// </summary>
    SetInformation = 0x0200,

    /// <summary>
    ///     Required to retrieve certain information about a process.
    /// </summary>
    QueryInformation = 0x0400,

    /// <summary>
    ///     Required to suspend or resume a process.
    /// </summary>
    SuspendResume = 0x0800,

    /// <summary>
    ///     Required to retrieve certain information about a process.
    /// </summary>
    QueryLimitedInformation = 0x1000,

    /// <summary>
    ///
    /// </summary>
    SetLimitedInformation = 0x2000
}
