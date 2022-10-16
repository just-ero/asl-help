using AslHelp.Collections;

namespace AslHelp.UE.Models;

public class FNameCache : CachedEnumerable<int, FName>
{
    public new string this[int index] => base[index].Name;

    public override IEnumerator<FName> GetEnumerator()
    {
        foreach (FName fName in Unreal.Manager.EnumerateFNames())
        {
            if (Unreal.Instance.LoadCanceled)
            {
                yield break;
            }

            yield return fName;
        }
    }

    protected override int GetKey(FName fName)
    {
        return fName.Index;
    }

    public bool TryGetValue(int index, out string name)
    {
        if (_cache.TryGetValue(index, out FName fName))
        {
            name = fName.Name;
            return true;
        }
        else
        {
            name = null;
            return false;
        }
    }

    internal void Add(FName fName)
    {
        _cache[fName.Index] = fName;
    }
}
