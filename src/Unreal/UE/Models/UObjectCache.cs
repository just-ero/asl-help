using AslHelp.Collections;

namespace AslHelp.UE.Models;

public class UObjectCache : CachedEnumerable<string, UObject>
{
    public override IEnumerator<UObject> GetEnumerator()
    {
        foreach (nint uObject in Unreal.Manager.EnumerateUObjects())
        {
            if (Unreal.Instance.LoadCanceled)
            {
                yield break;
            }

            yield return new(uObject);
        }
    }

    protected override string GetKey(UObject uObject)
    {
        string obj = uObject.ToString();

        if (uObject.Outer?.ToString() is string outer && outer != "")
        {
            return outer + "." + obj;
        }
        else
        {
            return obj;
        }
    }

    protected override void OnSearch(string name)
    {
        int i = name.IndexOf('.');

        if (i == -1)
        {
            string msg =
                "Search for UObjects including their outer: 'OuterName.Name'." + Environment.NewLine +
                "This is to combat duplicate names in different packages.";

            throw new InvalidOperationException(msg);
        }

        Debug.Info($"Searching for UObject '{name[(i + 1)..]}'...");
    }

    protected override void OnFound(UObject uObject)
    {
        Debug.Info($"  => Found at 0x{uObject.Address.ToString("X")}.");

        uObject.DebugAllFields();
    }

    protected override void OnNotFound(string name)
    {
        Debug.Info("  => Not found!");
    }
}
