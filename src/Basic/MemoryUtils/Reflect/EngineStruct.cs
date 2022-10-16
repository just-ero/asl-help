namespace AslHelp.MemUtils.Reflect;

internal record EngineStruct
{
    private readonly Dictionary<string, EngineField> _fields = new();

    public EngineStruct(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public string Super { get; set; }

    public IEnumerable<EngineField> Fields => _fields.Values;

    public EngineField this[string fieldName]
    {
        get
        {
            if (_fields.TryGetValue(fieldName, out EngineField field))
            {
                return field;
            }
            else
            {
                string msg = $"[EngineStruct] Field '{fieldName}' was not present in struct '{Name}'.";
                throw new KeyNotFoundException(msg);
            }
        }
        set => _fields[fieldName] = value;
    }

    public bool TryGetValue(string fieldName, out EngineField field)
    {
        return _fields.TryGetValue(fieldName, out field);
    }

    public int Size => Fields.LastOrDefault() is EngineField field ? field.Offset + field.Size : 0;
    public int Alignment => Fields.FirstOrDefault() is EngineField field ? field.Alignment : ReflectUtils.MIN_ALIGN;
    public int SelfAlignedSize => ReflectUtils.Align(Size, Alignment);
}
