public partial class Basic
{
    public bool Reject(params int[] moduleMemorySizes)
    {
        return Reject(MainModule, moduleMemorySizes);
    }

    public bool Reject(string module, params int[] moduleMemorySizes)
    {
        return Reject(Modules[module], moduleMemorySizes);
    }

    public bool Reject(Module module, params int[] moduleMemorySizes)
    {
        if (module is null)
        {
            Debug.Warn("[Reject] Module could not be found.");
            return false;
        }

        if (moduleMemorySizes is null || moduleMemorySizes.Length == 0)
        {
            Game = null;
            return true;
        }

        int exeModuleSize = module.MemorySize;
        if (moduleMemorySizes.Any(mms => mms == exeModuleSize))
        {
            Game = null;
            return true;
        }

        return false;
    }
}
