using AslHelp.Collections;

namespace AslHelp.UE.Models;

public class UObject : CachedEnumerable<string, UField>
{
    internal UObject(nint address)
    {
        Address = address;
    }

    public nint Address { get; }

    private FName _fName;
    public FName FName => _fName ??= Unreal.Manager.UObjectFName(Address);

    private UObject _class;
    public UObject Class => _class ??= new(Unreal.Manager.UObjectClass(Address));

    private UObject _super;
    public UObject Super => _super ??= new(Unreal.Manager.UObjectSuper(Address));

    private UObject _outer;
    public UObject Outer => _outer ??= new(Unreal.Manager.UObjectOuter(Address));

    public override IEnumerator<UField> GetEnumerator()
    {
        foreach (nint uField in Unreal.Manager.EnumerateUFields(Address))
        {
            if (Unreal.Instance.LoadCanceled)
            {
                yield break;
            }

            yield return new(uField, Address);
        }
    }

    protected override string GetKey(UField uField)
    {
        return uField.FName.Name;
    }

    internal void DebugAllFields()
    {
        if (!this.Any())
        {
            Debug.Info("    => No fields.");
        }
        else
        {
            foreach (UField field in this.OrderBy(f => f.Offset))
            {
                Debug.Info($"    => {field}");
            }
        }
    }

    public override string ToString()
    {
        return FName.Name;
    }
}
