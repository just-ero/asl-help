namespace AslHelp.Memory.Native.Structs;

internal unsafe struct ModuleInfo
{
    public void* BaseOfDll;
    public uint SizeOfImage;
    public void* EntryPoint;
}
