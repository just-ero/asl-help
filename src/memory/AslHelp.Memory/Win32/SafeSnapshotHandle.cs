using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Win32.SafeHandles;

namespace AslHelp.Memory.Win32;

/// <summary>
///     Represents a wrapper around a native ToolHelp32 snapshot handle that ensures the handle is closed
///     via <see cref="PInvoke.CloseHandle"/> when it is released.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
[ExcludeFromCodeCoverage]
internal sealed class SafeSnapshotHandle()
    : SafeHandleZeroOrMinusOneIsInvalid(ownsHandle: true)
{
    /// <summary>
    ///     Closes the underlying snapshot handle.
    /// </summary>
    /// <returns>
    ///     <see langword="true"/> if the handle was released successfully;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    protected override bool ReleaseHandle()
    {
        return PInvoke.CloseHandle((nuint)(nint)handle);
    }

    /// <summary>
    ///     Returns the underlying handle formatted as a hexadecimal string.
    /// </summary>
    /// <returns>
    ///     The handle value formatted as <c>0x{value:X}</c>.
    /// </returns>
    public override string ToString()
    {
        return $"0x{(long)handle:X}";
    }
}
