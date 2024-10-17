using System.Linq;

using AslHelp.Memory;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    public bool Reject(params int[] moduleMemorySizes)
    {
        return Reject(Memory.MainModule, moduleMemorySizes);
    }

    public bool Reject(string module, params int[] moduleMemorySizes)
    {
        return Reject(Memory.Modules[module], moduleMemorySizes);
    }

    public bool Reject(Module module, params int[] moduleMemorySizes)
    {
        if (moduleMemorySizes.Length == 0)
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
