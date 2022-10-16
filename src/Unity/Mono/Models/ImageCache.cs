using AslHelp.Collections;
using AslHelp.MemUtils.Exceptions;
using AslHelp.Mono.Managers;
using System.Xml.Linq;

namespace AslHelp.Mono.Models;

public class ImageCache : CachedEnumerable<string, MonoImage>
{
    public new MonoImage this[string imageName]
    {
        get
        {
            if (base.TryGetValue(imageName, out MonoImage monoImage))
            {
                return monoImage;
            }
            else
            {
                string msg =
                    $"Image '{imageName}' Could not be found. " +
                    $"Ensure correct spelling. Names are case sensitive.";

                throw new NotFoundException(msg);
            }
        }
    }

    public override IEnumerator<MonoImage> GetEnumerator()
    {
        foreach (nint image in Unity.Manager.EnumerateImages())
        {
            if (Unity.Instance.LoadCanceled)
            {
                yield break;
            }

            MonoImage monoImage = new(image);
            if (monoImage.Name is not null)
            {
                yield return monoImage;
            }
        }
    }

    protected override string GetKey(MonoImage monoImage)
    {
        return monoImage.Name;
    }

    protected override void OnSearch(string name)
    {
        Debug.Info($"Searching for image '{name}'...");
    }

    protected override void OnFound(MonoImage monoImage)
    {
        if (Unity.Manager is Il2CppManager)
        {
            Debug.Info($"  => Found at 0x{monoImage.Address.ToString("X")}.");
            Debug.Info($"    => typeCount is {monoImage.CacheSize}.");
            Debug.Info($"    => typeStart at 0x{monoImage.CacheTable.ToString("X")}.");
        }
        else
        {
            Debug.Info($"  => Found at 0x{monoImage.Address.ToString("X")}.");
            Debug.Info($"    => class_cache.size is {monoImage.CacheSize}.");
            Debug.Info($"    => class_cache.table at 0x{monoImage.CacheTable.ToString("X")}.");
        }
    }
}
