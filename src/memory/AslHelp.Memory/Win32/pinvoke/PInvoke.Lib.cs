using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]

namespace AslHelp.Memory.Win32;

[ExcludeFromCodeCoverage]
internal static unsafe partial class PInvoke
{
    public const string Kernel32 = "kernel32.dll";
    public const string PsApi = "psapi.dll";
    public const string DbgHelp = "dbghelp.dll";
}
