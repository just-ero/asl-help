namespace ASLHelper.UnityHelper;

public class MonoClass
{
    public string NameSpace { get; internal set; }
    public string Name { get; internal set; }
    public nint Address { get; internal set; }
    public nint Static { get; internal set; }
    public List<MonoField> Fields { get; internal set; }

    public int this[string fieldName]
    {
        get => Fields.First(f => f.Name == fieldName).Offset;
    }
}
