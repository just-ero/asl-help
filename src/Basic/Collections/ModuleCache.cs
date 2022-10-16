using AslHelp.MemUtils;

namespace AslHelp.Collections;

public class ModuleCache : CachedEnumerable<string, Module>
{
    public override IEnumerator<Module> GetEnumerator()
    {
        foreach (Module module in Basic.Instance.Game.Modules())
        {
            yield return module;
        }
    }

    protected override string GetKey(Module module)
    {
        return module.Name;
    }

    protected override bool CompareKeys(string searchedId, string itemId)
    {
        return searchedId.Equals(itemId, StringComparison.OrdinalIgnoreCase);
    }

    protected override string KeyNotFoundMessage(string name)
    {
        return $"The given module '{name}' was not present in the process.";
    }
}
