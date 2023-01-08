using AslHelp.Mono.Models;

namespace AslHelp.Mono.Managers;

public abstract partial class UnityMemManager
{
    internal IEnumerable<MonoField> EnumerateFields(nint klass)
    {
        nint parent = ClassParent(klass);

        if (parent != 0 && (ClassName(parent) != "Object" || ClassNamespace(parent) != "UnityEngine"))
        {
            foreach (MonoField field in EnumerateFields(parent))
            {
                yield return field;
            }
        }

        if (!ClassHasFields(klass, out nint fields, out int count))
        {
            yield break;
        }

        bool isValueType = ClassIsValueType(klass);
        int fieldSize = _engine["MonoClassField"].SelfAlignedSize;

        for (int i = 0; i < count; i++)
        {
            yield return new(fields + (fieldSize * i), isValueType);
        }
    }

    internal nint FieldType(nint field)
    {
        return ReadPtr(field + _engine["MonoClassField"]["type"]);
    }

    internal string FieldName(nint field)
    {
        return ReadStr(field + _engine["MonoClassField"]["name"], 128);
    }

    internal int FieldOffset(nint field)
    {
        return ReadI32(field + _engine["MonoClassField"]["offset"]);
    }
}
