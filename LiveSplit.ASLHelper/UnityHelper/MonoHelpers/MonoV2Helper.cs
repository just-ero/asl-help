namespace ASLHelper.UnityHelper;

public class MonoV2Helper : MonoV1Helper
{
    public MonoV2Helper(Unity helper, string type, string version)
        : base(helper, type, version) { }

    #region Classes
    private protected override int ClassFieldCount(nint klass)
    {
        var monoType = ReadI8(klass + _engine["MonoClass"]["class_kind"]) & 7;

        return monoType switch
        {
            1 or 2 => ReadI32(klass + _engine["MonoClass"]["field_count"]),
            3 => ClassFieldCount(ClassGenericClass(klass)),
            _ => 0,
        };
    }

    private protected override nint GetStaticAddress(nint klass)
    {
        var vtable_size = ReadI32(klass + _engine["MonoClass"]["vtable_size"]);
        return ReadPtr(ClassVTable(klass) + _engine["MonoVTable"]["vtable"] + (_helper.PtrSize * vtable_size));
    }

    private protected nint ClassGenericClass(nint klass)
    {
        var generic_class = ReadPtr(klass + _engine["MonoClassGenericInst"]["generic_class"]);
        return ReadPtr(generic_class + _engine["MonoGenericClass"]["container_class"]);
    }
    #endregion
}
