using System;

using AslHelp.LiveSplit;

[Obsolete("Do not use ASL-specific features.", true)]
public partial class Basic : AslPluginBase
{
    public Basic()
        : this(true) { }

    public Basic(bool generateCode)
        : base(generateCode) { }

    public nint ReadRelative(nint relativeAddress, int instructionSize = 0x4)
    {
        return Is64Bit
            ? relativeAddress + instructionSize + Read<int>(relativeAddress)
            : Read<nint>(relativeAddress);
    }
}
