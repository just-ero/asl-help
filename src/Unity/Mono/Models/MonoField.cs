namespace AslHelp.Mono.Models;

public class MonoField
{
    private readonly bool _parentIsStruct;

    internal MonoField(nint address, bool parentIsStruct)
    {
        _parentIsStruct = parentIsStruct;
        Address = address;
    }

    public nint Address { get; }

    private MonoType _type;
    public MonoType Type => _type ??= new(Unity.Manager.FieldType(Address));

    private string _name;
    public string Name => _name ??= Unity.Manager.FieldName(Address).ToValidIdentifierUnity();

    private int? _offset;
    public int Offset
    {
        get
        {
            if (_offset is not null)
            {
                return _offset.Value;
            }

            int offset = Unity.Manager.FieldOffset(Address);
            if (!IsStatic && _parentIsStruct)
            {
                offset -= Unity.Instance.PtrSize * 2;
            }

            _offset = offset;
            return offset;
        }
    }

    private bool? _isConst;
    public bool IsConst => _isConst ??= (Type.Attributes & MonoFieldAttribute.LITERAL) != 0;

    private bool? _isStatic;
    public bool IsStatic => _isStatic ??= (Type.Attributes & MonoFieldAttribute.STATIC) != 0;

    public override string ToString()
    {
        return $"0x{Offset:X3}: {(IsStatic ? "static" : "      ")} {Name}";
    }
}
