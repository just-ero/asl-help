using AslHelp.UE.Models;
using System.Xml.Linq;

namespace AslHelp.UE.Managers;

public abstract partial class UEMemManager
{
    internal abstract nint UObjectsData { get; }
    internal abstract int UObjectsSize { get; }

    internal abstract IEnumerable<nint> EnumerateUObjects();

    internal abstract nint UObjectClass(nint uObject);
    internal abstract nint UObjectSuper(nint uObject);
    internal abstract nint UObjectOuter(nint uObject);

    internal abstract int UObjectFNameIndex(nint uObject);
    internal abstract int UObjectPropertiesSize(nint uObject);

    internal FName UObjectFName(nint uObject)
    {
        int index = UObjectFNameIndex(uObject);
        return new(index);
    }

    internal FName UObjectClassFName(nint uObject)
    {
        return UObjectFName(UObjectClass(uObject));
    }

    internal FName UObjectSuperFName(nint uObject)
    {
        return UObjectFName(UObjectSuper(uObject));
    }

    internal FName UObjectOuterFName(nint uObject)
    {
        return UObjectFName(UObjectOuter(uObject));
    }

    internal ClassCastFlags UObjectFlags(nint uObject)
    {
        return _game.Read<ClassCastFlags>(uObject + _engine["UClass"]["ClassCastFlags"]);
    }
}
