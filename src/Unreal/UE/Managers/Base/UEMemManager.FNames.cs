using AslHelp.UE.Models;
using System.Xml.Linq;

namespace AslHelp.UE.Managers;

public abstract partial class UEMemManager
{
    protected abstract nint FNamePool { get; }

    internal abstract IEnumerable<FName> EnumerateFNames();
    internal abstract nint FNameNameEntry(int fNameIndex);
    internal abstract int FNameIndex(nint fName);
    internal abstract string FNameName(nint fName);

    internal string FNameName(int fNameIndex)
    {
        if (fNameIndex <= 0)
        {
            return null;
        }

        if (FNames.TryGetValue(fNameIndex, out string name))
        {
            return name;
        }

        name = FNameName(FNameNameEntry(fNameIndex));

        if (name is null)
        {
            Debug.Warn($"An FName with the index [{fNameIndex:0000000}] could not be found.");
            return null;
        }

        name = name.ToValidIdentifier();

        FNames.Add(new(fNameIndex, name));

        return name;
    }
}
