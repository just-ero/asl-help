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

    public ReadWriteWatcher<T> Make<T>(string staticField, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
            throw new NullReferenceException("Static field table address was null!");

        return new(Static + this[staticField], offsets);
    }

    public ReadWriteWatcher<T> Make<T>(string staticField, string field, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
            throw new NullReferenceException("Static field table address was null!");

        return new(Static + this[staticField], offsets.Prepend(this[field]).ToArray());
    }

    public StringWatcher MakeString(string staticField, params int[] offsets)
    {
        if (Static == 0)
            throw new NullReferenceException("Static field table address was null!");

        return new(Static, this[staticField], offsets);
    }

    public StringWatcher MakeString(string staticField, string field, params int[] offsets)
    {
        if (Static == 0)
            throw new NullReferenceException("Static field table address was null!");

        return new(Static, this[staticField], offsets.Prepend(this[field]).ToArray());
    }

    public ListWatcher<T> MakeList<T>(string staticField, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
            throw new NullReferenceException("Static field table address was null!");

        return new(Static, this[staticField], offsets);
    }

    public ListWatcher<T> MakeList<T>(string staticField, string field, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
            throw new NullReferenceException("Static field table address was null!");

        return new(Static, this[staticField], offsets.Prepend(this[field]).ToArray());
    }

    public ArrayWatcher<T> MakeArray<T>(string staticField, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
            throw new NullReferenceException("Static field table address was null!");

        return new(Static, this[staticField], offsets);
    }

    public ArrayWatcher<T> MakeArray<T>(string staticField, string field, params int[] offsets) where T : unmanaged
    {
        if (Static == 0)
            throw new NullReferenceException("Static field table address was null!");

        return new(Static, this[staticField], offsets.Prepend(this[field]).ToArray());
    }
}
