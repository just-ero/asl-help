using AslHelp.Collections;

namespace AslHelp.MemUtils.Models;

public record Module
{
    internal Module(string baseName, string fileName, MODULEINFO moduleInfo)
    {
        Name = baseName;
        FilePath = fileName;
        Base = moduleInfo.lpBaseOfDll;
        MemorySize = (int)moduleInfo.SizeOfImage;
        Symbols = new(this);
    }

    public string Name { get; }
    public string FilePath { get; }
    public nint Base { get; }
    public int MemorySize { get; }
    public SymbolCache Symbols { get; }

    public FileVersionInfo VersionInfo => FileVersionInfo.GetVersionInfo(FilePath);

    public override string ToString()
    {
        return Name;
    }
}
