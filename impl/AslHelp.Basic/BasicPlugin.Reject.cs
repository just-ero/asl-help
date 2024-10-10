using System.Linq;

using AslHelp.Memory;
using AslHelp.Shared;

public partial class Basic
{
    public override bool Reject(params int[] moduleMemorySizes)
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return Reject(MainModule, moduleMemorySizes);
    }

    public override bool Reject(string module, params int[] moduleMemorySizes)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return Reject(Modules[module], moduleMemorySizes);
    }

    public override bool Reject(Module module, params int[] moduleMemorySizes)
    {
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
