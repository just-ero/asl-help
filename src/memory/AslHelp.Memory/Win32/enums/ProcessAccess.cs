using System;

namespace AslHelp.Memory.Win32;

/// <summary>
///     Provides process-specific access rights.
/// </summary>
/// <remarks>
///     For further information see:
///     <i><see href="https://learn.microsoft.com/windows/win32/procthread/process-security-and-access-rights">
///         Process Security and Access Rights
///     </see></i>
/// </remarks>
[Flags]
#pragma warning disable CA1028 // Enum storage should be int
public enum ProcessAccess : uint
#pragma warning restore CA1028
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
    CreateProcess = 0x080,

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
    SetLimitedInformation = 0x2000,

    // Standard Access Rights
    /// <summary>
    ///     Required to delete the object.
    /// </summary>
    Delete = StandardAccess.Delete,

    /// <summary>
    ///     Required to read information in the security descriptor for the object.
    /// </summary>
    ReadControl = StandardAccess.ReadControl,

    /// <summary>
    ///     Required to modify the DACL in the security descriptor for the object.
    /// </summary>
    WriteDac = StandardAccess.WriteDac,

    /// <summary>
    ///     Required to change the owner in the security descriptor for the object.
    /// </summary>
    WriteOwner = StandardAccess.WriteOwner,

    /// <summary>
    ///     The right to use the object for synchronization.
    /// </summary>
    Synchronize = StandardAccess.Synchronize,
}
