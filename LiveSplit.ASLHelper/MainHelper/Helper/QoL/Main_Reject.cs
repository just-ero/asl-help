namespace ASLHelper;

public partial class Main
{
    public bool Reject(params int[] moduleMemorySizes)
    {
        var module = Game?.MainModuleWow64Safe();
        return Reject(module, moduleMemorySizes);
    }

    public bool Reject(string moduleName, params int[] moduleMemorySizes)
    {
        var module = GetModule(moduleName);
        return Reject(module, moduleMemorySizes);
    }

    public bool Reject(ProcessModuleWow64Safe module, params int[] moduleMemorySizes)
    {
        if (module == null)
        {
            Debug.Warn("[Reject] Module could not be found!");
            return false;
        }

        if (moduleMemorySizes == null || moduleMemorySizes.Length == 0)
        {
            Game = null;
            return true;
        }

        var exeModuleSize = module.ModuleMemorySize;
        if (moduleMemorySizes.Any(mms => mms == exeModuleSize))
        {
            Game = null;
            return true;
        }

        return false;
    }
}