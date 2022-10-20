namespace AslHelp.UE.Models;

public class UField
{
    private readonly nint _parent;

    internal UField(nint address, nint parent)
    {
        _parent = parent;
        Address = address;
    }

    public nint Address { get; }

    private FName _fName;
    public FName FName => _fName ??= Unreal.Manager.UFieldFName(Address);

    private UObject _class;
    public UObject Class
    {
        get
        {
            if (_class is not null)
            {
                return _class;
            }

            if (!Unreal.Instance.TryRead<nint>(out nint address, _parent + Offset)
                || address < Unreal.Instance.MainModule.Base)
            {
                address = Address;
            }

            (_class, string name, bool gotInner) = Unreal.Manager.UFieldTypeInfo(address);
            Debug.Info(address.ToString("X"));
            Debug.Info(name);

            // FIXME: Can we cache here?

            return _class;
        }
    }

    private int? _offset;
    public int Offset => _offset ??= Unreal.Manager.UFieldOffset(Address);

    public override string ToString()
    {
        return $"0x{Offset:X4}: {FName}";
    }
}
