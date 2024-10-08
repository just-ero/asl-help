using System;

namespace AslHelp.Memory.Native.Enums;

[Flags]
internal enum ListModulesFilter : uint
{
    Default,

    List32Bit = 1 << 0,
    List64Bit = 1 << 1,

    ListAll = List32Bit | List64Bit
}
