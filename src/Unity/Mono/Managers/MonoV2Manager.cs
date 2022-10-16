using AslHelp.Mono.Models;

namespace AslHelp.Mono.Managers;

internal class MonoV2Manager : MonoV1Manager
{
    public MonoV2Manager(string version)
        : base(version) { }

    internal override nint ClassStaticFields(nint klass)
    {
        int vtable_size = ReadI32(klass + _engine["MonoClass"]["vtable_size"]);
        return ReadPtr(ClassVTable(klass) + _engine["MonoVTable"]["vtable"] + (_game.PtrSize * vtable_size));
    }

    internal override int ClassFieldCount(nint klass)
    {
        return ClassTypeKind(klass) switch
        {
            MonoTypeKind.DEF or MonoTypeKind.GTD => ReadI32(klass + _engine["MonoClassDef"]["field_count"]),
            MonoTypeKind.GINST => ClassFieldCount(ClassGenericClass(klass)),
            _ => 0
        };
    }

    internal virtual MonoTypeKind ClassTypeKind(nint klass)
    {
        uint class_kind = ReadU32(klass + _engine["MonoClass"]["class_kind"]);
        class_kind &= _engine["MonoClass"]["class_kind"];

        return (MonoTypeKind)class_kind;
    }

    internal nint ClassGenericClass(nint klass)
    {
        nint generic_class = ReadPtr(klass + _engine["MonoClassGenericInst"]["generic_class"]);
        return ReadPtr(generic_class + _engine["MonoGenericClass"]["container_class"]);
    }

    internal override nint ClassNextClassCache(nint klass)
    {
        return ReadPtr(klass + _engine["MonoClassDef"]["next_class_cache"]);
    }
}
