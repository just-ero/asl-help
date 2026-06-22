namespace AslHelp.Memory.Win32;

/// <summary>
///     Contains the module load address, size, and entry point.
/// </summary>
/// <remarks>
///     For further information see:
///     <i><see href="https://learn.microsoft.com/windows/win32/api/psapi/ns-psapi-moduleinfo">
///         MODULEINFO structure (psapi.h)
///     </see></i>
/// </remarks>
internal unsafe struct ModuleInfo
{
    /// <summary>
    ///     The load address of the module.
    /// </summary>
    public void* BaseOfDll;

    /// <summary>
    ///     The size of the linear space that the module occupies, in bytes.
    /// </summary>
    public uint SizeOfImage;

    /// <summary>
    ///     The entry point of the module.
    /// </summary>
    public void* EntryPoint;
}
