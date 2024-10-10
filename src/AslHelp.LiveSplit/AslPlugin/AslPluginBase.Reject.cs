using AslHelp.Memory;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    public abstract bool Reject(params int[] moduleMemorySizes);
    public abstract bool Reject(string module, params int[] moduleMemorySizes);
    public abstract bool Reject(Module module, params int[] moduleMemorySizes);
}
