using AslHelp.Mono.Models;

namespace AslHelp.Mono.Managers;

public abstract partial class UnityMemManager
{
    internal abstract IEnumerable<nint> EnumerateClasses(nint cacheTable, int cacheSize);
    internal abstract nint ClassStaticFields(nint klass);
    internal abstract int ClassFieldCount(nint klass);

    internal nint ClassFromIndex(nint table, int index)
    {
        return ReadPtr(table + (_game.PtrSize * index));
    }

    internal MonoClass GetClass(nint klass, int parent = 0)
    {
        nint parentKlass = klass;

        for (int i = 0; i < parent; i++)
        {
            parentKlass = ClassParent(parentKlass);
        }

        return new(klass, ClassStaticFields(parentKlass));
    }

    internal nint ClassParent(nint klass)
    {
        return ReadPtr(klass + _engine["MonoClass"]["parent"]);
    }

    internal string ClassName(nint klass)
    {
        return ReadStr(klass + _engine["MonoClass"]["name"], 128);
    }

    internal string ClassNamespace(nint klass)
    {
        return ReadStr(klass + _engine["MonoClass"]["name_space"], 256);
    }

    internal nint ClassFields(nint klass)
    {
        return ReadPtr(klass + _engine["MonoClass"]["fields"]);
    }

    internal bool ClassHasFields(nint klass, out nint fields, out int fieldCount)
    {
        fields = ClassFields(klass);
        fieldCount = ClassFieldCount(klass);

        return fields != 0 && fieldCount > 0;
    }

    internal nint ClassType(nint klass)
    {
        return klass + _engine["MonoClass"]["this_arg"];
    }

    internal bool ClassIsValueType(nint klass)
    {
        uint valuetype = _game.Read<uint>(klass + _engine["MonoClass"]["valuetype"]);
        valuetype &= _engine["MonoClass"]["valuetype"];

        return valuetype != 0;
    }

    internal nint MonoArrayClass(nint arrayType)
    {
        return ReadPtr(arrayType + _engine["MonoArrayType"]["eklass"]);
    }

    internal int MonoArrayRank(nint arrayType)
    {
        return ReadU8(arrayType + _engine["MonoArrayType"]["rank"]);
    }

    internal nint MonoGenericInstClass(nint genericClass)
    {
        return ReadPtr(genericClass + _engine["MonoGenericClass"]["context"]);
    }
}
