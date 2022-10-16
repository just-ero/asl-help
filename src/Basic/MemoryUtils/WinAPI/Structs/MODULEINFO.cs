namespace AslHelp.MemUtils;

internal struct MODULEINFO
{
    public nint lpBaseOfDll;
    public uint SizeOfImage;
    public nint EntryPoint;

    public static unsafe uint SelfSize => (uint)sizeof(MODULEINFO);
}
