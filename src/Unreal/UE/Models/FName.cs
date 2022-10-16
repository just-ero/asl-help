namespace AslHelp.UE.Models;

public record FName
{
    private readonly nint _address;

    internal FName(nint address)
    {
        _address = address;
    }

    internal FName(int index)
    {
        _index = index;
    }

    internal FName(int index, string name)
    {
        _index = index;
        _name = name;
    }

    private int? _index;
    public int Index
    {
        get
        {
            if (_index is not null)
            {
                return _index.Value;
            }

            _index = Unreal.Manager.FNameIndex(_address);

            return _index.Value;
        }
    }

    private string _name;
    public string Name
    {
        get
        {
            if (_name is not null)
            {
                return _name;
            }

            _name = Unreal.Manager.FNameName(Index);
            return _name;
        }
    }

    public override string ToString()
    {
        return Name;
    }
}
