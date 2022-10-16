using AslHelp.UE.Models;
using System.Xml.Linq;

namespace AslHelp.UE.Managers;

public abstract partial class UEMemManager
{
    internal abstract IEnumerable<nint> EnumerateUFields(nint uObject);

    internal abstract nint UFieldClass(nint uField);
    internal abstract FName UFieldClassFName(nint uField);

    internal abstract int UFieldFNameIndex(nint uField);
    internal abstract int UFieldOffset(nint uField);

    internal FName UFieldFName(nint uField)
    {
        int index = UFieldFNameIndex(uField);
        return new(index);
    }

    internal (UObject Obj, string Name, bool GotInner) UFieldTypeInfo(nint uField)
    {
        nint fieldClass = UFieldClass(uField);
        string name = UFieldFName(fieldClass).Name;

        UObject obj = new(fieldClass);

        switch (name)
        {
            case "Int8Property": return (obj, "signed char", false);
            case "Int16Property": return (obj, "short", false);
            case "UInt16Property": return (obj, "unsigned short", false);
            case "IntProperty": return (obj, "int", false);
            case "UInt32Property": return (obj, "unsigned int", false);
            case "Int64Property": return (obj, "long long", false);
            case "UInt64Property": return (obj, "unsigned long long", false);
            case "FloatProperty": return (obj, "float", false);
            case "DoubleProperty": return (obj, "double", false);
            case "BoolProperty": return (obj, "bool", false);
            case "StrProperty": return (obj, "FString", false);
            case "NameProperty": return (obj, "FName", false);

            case "ByteProperty":
            {
                nint eProp = ReadPtr(uField + _engine["UByteProperty"]["Enum"]);
                if (eProp == 0)
                {
                    return (obj, $"unsigned char", false);
                }

                obj = new(eProp);
                return (obj, obj.ToString(), false);
            }

            case "ObjectProperty":
            case "LazyObjectProperty":
            case "SoftObjectProperty":
            case "WeakObjectProperty":
            {
                nint objClass = ReadPtr(uField + _engine["UObjectProperty"]["PropertyClass"]);

                obj = new(objClass);
                return (obj, obj.ToString(), false);
            }
            case "ClassProperty":
            {
                nint cProp = ReadPtr(uField + _engine["UClassProperty"]["MetaClass"]);

                obj = new(cProp);
                return (obj, obj.ToString(), false);
            }
            case "StructProperty":
            {
                nint sProp = ReadPtr(uField + _engine["UStructProperty"]["Struct"]);

                obj = new(sProp);
                return (obj, obj.ToString(), false);
            }
            case "InterfaceProperty":
            {
                nint iProp = ReadPtr(uField + _engine["UInterfaceProperty"]["InterfaceClass"]);

                obj = new(iProp);
                return (obj, obj.ToString(), false);
            }

            case "ArrayProperty":
            {
                nint aInner = ReadPtr(uField + _engine["UArrayProperty"]["Inner"]);
                obj = new(aInner);

                (obj, name, _) = UFieldTypeInfo(aInner);

                return (obj, $"TArray<{name}>", true);
            }
            case "SetProperty":
            {
                nint sInner = ReadPtr(uField + _engine["TSet"]["Element"]);
                obj = new(sInner);

                (obj, name, _) = UFieldTypeInfo(sInner);

                return (obj, $"TSet<{name}>", true);
            }
            case "EnumProperty":
            {
                nint eProp = ReadPtr(uField + _engine["UEnumProperty"]["Enum"]);

                obj = new(eProp);
                return (obj, obj.ToString(), false);
            }

            case "Object":
            case "Class":
            {
                return (new(uField), UFieldFName(uField).Name, false);
            }

            default:
            {
                return (obj, obj.ToString(), false);
            }
        }
    }
}
