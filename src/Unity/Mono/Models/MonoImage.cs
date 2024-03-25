using AslHelp.Collections;

namespace AslHelp.Mono.Models;

public class MonoImage : CachedEnumerable<string, MonoClass>
{
    internal MonoImage(nint address)
    {
        Address = address;
    }

    public nint Address { get; }

    private string _name;
    public string Name => _name ??= Unity.Manager.ImageName(Address);

    private nint? _cacheTable;
    public nint CacheTable => _cacheTable ??= Unity.Manager.ImageCacheTable(Address);

    private int? _cacheSize;
    public int CacheSize => _cacheSize ??= Unity.Manager.ImageCacheSize(Address);

    public override IEnumerator<MonoClass> GetEnumerator()
    {
        foreach (nint klass in Unity.Manager.EnumerateClasses(CacheTable, CacheSize))
        {
            if (Unity.Instance.LoadCanceled)
            {
                yield break;
            }

            MonoClass monoClass = new(klass);
            if (monoClass.Name is not null)
            {
                yield return monoClass;
            }
        }
    }

    protected override string GetKey(MonoClass monoClass)
    {
        if (string.IsNullOrEmpty(monoClass.Namespace))
        {
            return monoClass.Name;
        }
        else
        {
            return monoClass.Namespace + "." + monoClass.Name;
        }
    }

    protected override void OnSearch(string name)
    {
        Debug.Info($"Searching for class '{name}'...");
    }

    protected override void OnFound(MonoClass monoClass)
    {
        Debug.Info($"  => Found at 0x{monoClass.Address.ToString("X")}.");
        Debug.Info($"  => Static field table at 0x{monoClass.Static.ToString("X")}.");
    }

    protected override void OnNotFound(string key)
    {
        Debug.Info("  => Not found!");
    }

    protected override bool CompareKeys(string searchedName, string itemName)
    {
        int searchedIdIndex = searchedName.LastIndexOf('.');
        int itemIdIndex = itemName.LastIndexOf('.');

        return searchedIdIndex switch
        {
            -1 => itemIdIndex switch
            {
                -1 => itemName == searchedName,
                _ => itemName[(itemIdIndex + 1)..] == searchedName
            },
            _ => itemIdIndex switch
            {
                -1 => false,
                _ => itemName == searchedName
            }
        };
    }
}
