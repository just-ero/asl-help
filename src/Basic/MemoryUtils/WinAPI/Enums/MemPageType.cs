namespace AslHelp.MemUtils;

public enum MemPageType : uint
{
    MEM_PRIVATE = 0x0020000,
    MEM_MAPPED = 0x0040000,
    MEM_IMAGE = 0x1000000
}
