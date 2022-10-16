namespace AslHelp.Mono.Managers;

public abstract partial class UnityMemManager
{
    internal abstract nint FindAssemblies();

    internal nint AssemblyImage(nint assembly)
    {
        return ReadPtr(assembly + _engine["MonoAssembly"]["image"]);
    }

    internal abstract IEnumerable<nint> EnumerateImages();
    internal abstract string ImageName(nint image);
    internal abstract nint ImageCacheTable(nint image);
    internal abstract int ImageCacheSize(nint image);
    internal abstract int ImageCacheEntries(nint image);
}
