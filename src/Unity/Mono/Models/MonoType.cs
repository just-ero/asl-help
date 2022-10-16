namespace AslHelp.Mono.Models;

public class MonoType
{
    internal MonoType(nint address)
    {
        Address = address;
    }

    public nint Address { get; }

    private nint? _data;
    public nint Data => _data ??= Unity.Manager.TypeData(Address);

    private MonoFieldAttribute? _attributes;
    public MonoFieldAttribute Attributes => _attributes ??= Unity.Manager.TypeAttributes(Address);

    private MonoElementType? _elementType;
    public MonoElementType ElementType => _elementType ??= Unity.Manager.TypeElementType(Address);

    private MonoClass _class;
    public MonoClass Class
    {
        get
        {
            return _class ??= ElementType switch
            {
                MonoElementType.Ptr => new MonoType(Data).Class,
                MonoElementType.ValueType
                    or MonoElementType.Class => new(Data),
                MonoElementType.Array
                    or MonoElementType.SzArray => new(Unity.Manager.MonoArrayClass(Data)),
                MonoElementType.GenericInst => new(Unity.Manager.MonoGenericInstClass(Data)),
                _ => null
            };
        }
    }
}
