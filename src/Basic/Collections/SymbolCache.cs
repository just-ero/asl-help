using AslHelp.MemUtils;

namespace AslHelp.Collections;

public class SymbolCache : CachedEnumerable<string, DebugSymbol>
{
    private readonly Module _module;

    internal SymbolCache(Module module)
    {
        _module = module;
    }

    public override IEnumerator<DebugSymbol> GetEnumerator()
    {
        foreach (DebugSymbol symbol in _module.Symbols(Basic.Instance.Game))
        {
            yield return symbol;
        }
    }

    protected override string GetKey(DebugSymbol symbol)
    {
        return symbol.Name;
    }

    protected override bool CompareKeys(string searchedId, string itemId)
    {
        return searchedId.Equals(itemId, StringComparison.OrdinalIgnoreCase);
    }

    protected override string KeyNotFoundMessage(string key)
    {
        return $"The given symbol '{key}' was not present in the module '{_module.Name}'.";
    }
}
