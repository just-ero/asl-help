using AslHelp.UE.Models;

namespace AslHelp.UE.Managers;

internal partial class UE4_25Manager : UE4_23Manager
{
    public UE4_25Manager(int major, int minor)
        : base(major, minor) { }

    internal override IEnumerable<nint> EnumerateUFields(nint uObject)
    {
        int nextOffset = _engine["UProperty"]["PropertyLinkNext"];
        int superPropSize = UObjectPropertiesSize(UObjectSuper(uObject));
        int thisPropSize = superPropSize + UObjectPropertiesSize(uObject);

        nint child = ReadPtr(uObject + _engine["UStruct"]["ChildProperties"]);

        while (child != 0)
        {
            int offset = UFieldOffset(child);

            if (offset < superPropSize || offset > thisPropSize)
            {
                break;
            }

            //string className = UFieldClassFName(child).Name;

            //if (string.IsNullOrEmpty(className))
            //{
            //    break;
            //}

            //string name = UFieldFName(child).Name;

            //if (string.IsNullOrEmpty(name))
            //{
            //    break;
            //}

            yield return child;

            child = ReadPtr(child + nextOffset);
        }
    }

    internal override nint UFieldClass(nint uField)
    {
        return ReadPtr(uField + _engine["FField"]["Class"]);
    }

    internal override FName UFieldClassFName(nint uField)
    {
        int fNameIndex = ReadI32(UFieldClass(uField) + _engine["FFieldClass"]["Name"]);

        return new(fNameIndex);
    }

    internal override int UFieldFNameIndex(nint uField)
    {
        return ReadI32(uField + _engine["FField"]["Name"]);
    }
}
