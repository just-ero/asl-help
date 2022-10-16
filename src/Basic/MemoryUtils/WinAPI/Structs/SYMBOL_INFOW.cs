namespace AslHelp.MemUtils;

#pragma warning disable CS0649
internal struct SYMBOL_INFOW
{
    public uint SizeOfStruct;
    public uint TypeIndex;
    public ulong Reserved_0;
    public ulong Reserved_1;
    public uint Index;
    public uint Size;
    public ulong ModBase;
    public uint Flags;
    public ulong Value;
    public ulong Address;
    public uint Register;
    public uint Scope;
    public uint Tag;
    public uint NameLen;
    public uint MaxNameLen;
    public unsafe fixed ushort Name[1024];
}
#pragma warning restore CS0649
