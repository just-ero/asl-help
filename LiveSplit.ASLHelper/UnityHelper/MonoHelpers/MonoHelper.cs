namespace ASLHelper.UnityHelper;

public abstract partial class MonoHelper
{
    internal MonoHelper(Unity helper, string type, string version)
    {
        _engine = EngineReflection.Load("Unity", type, version);
        _helper = helper;
    }

    private protected EngineReflection _engine;
    private protected Unity _helper;
    private protected nint _loadedImages;
    private protected readonly Dictionary<string, MonoImage> _imageCache = new();

    private protected abstract MonoClass GetClass(MonoImage image, uint classToken, int depth = 0);
    private protected abstract nint ScanForImages();
    private protected abstract MonoImage GetImage(string name);
    private protected abstract IEnumerable<nint> Classes(MonoImage image);
    private protected abstract int ClassFieldCount(nint klass);
    private protected abstract nint GetStaticAddress(nint klass);

    public void ClearImages()
    {
        _imageCache.Clear();
    }

    private protected MonoClass MakeClass(nint klass)
    {
        return new()
        {
            NameSpace = ClassNameSpace(klass),
            Name = ClassName(klass),
            Address = klass,
            Static = GetStaticAddress(klass),
            Fields = GetAllFields(klass)
        };
    }

    #region Classes
    protected MonoClass CreateMonoClass(nint klass, int depth)
    {
        var name = ClassName(klass);
        var nameSpace = ClassNameSpace(klass);
        var fields = GetAllFields(klass);
        var parent = klass;

        for (int i = 0; i < depth; ++i)
        {
            parent = ClassParent(parent);
            fields.AddRange(GetAllFields(parent));
        }

        var staticAddress = GetStaticAddress(parent);
        Debug.Log("  => Found '" + name + "' at 0x" + klass.ToString("X") + ".");
        Debug.Log("  => Static field table at 0x" + staticAddress.ToString("X") + ".");

        foreach (var field in fields.OrderBy(f => f.Offset).Where(f => !f.IsConst))
            Debug.Log(string.Format("    => 0x{0:X3}: {1,-6} {2}", field.Offset, field.IsStatic ? "static" : "", field.Name));

        return new()
        {
            NameSpace = nameSpace,
            Name = name,
            Address = klass,
            Static = staticAddress,
            Fields = fields
        };
    }

    private protected MonoClass GetClass(MonoImage image, string className, int depth = 0)
    {
        string classNameSpace = null;
        var delimiter = className.LastIndexOf('.');
        if (delimiter > -1)
        {
            classNameSpace = className.Substring(0, delimiter);
            className = className.Substring(delimiter + 1);
        }

        Debug.Log("Searching for class '" + className + "'...");

        foreach (var klass in Classes(image))
        {
            var name = ClassName(klass);
            var nameSpace = ClassNameSpace(klass);

            if (name == className && (classNameSpace is null || nameSpace == classNameSpace))
                return CreateMonoClass(klass, depth);
        }

        Debug.Log("  => Not found!");
        return null;
    }

    public MonoClass GetClass(uint classToken, int depth = 0)
    {
        return GetClass("Assembly-CSharp", classToken, depth);
    }

    public MonoClass GetClass(string imageName, uint classToken, int depth = 0)
    {
        return GetClass(GetImage(imageName), classToken, depth);
    }

    public MonoClass GetClass(string className, int depth = 0)
    {
        return GetClass("Assembly-CSharp", className, depth);
    }

    public MonoClass GetClass(string imageName, string className, int depth = 0)
    {
        return GetClass(GetImage(imageName), className, depth);
    }

    public MonoClass GetParent(MonoClass monoClass)
    {
        return MakeClass(ReadPtr(monoClass.Address + _engine["MonoClass"]["parent"]));
    }

    private protected nint ClassParent(nint klass)
    {
        return ReadPtr(klass + _engine["MonoClass"]["parent"]);
    }

    private protected nint ClassFromIndex(nint table, int index)
    {
        return ReadPtr(table + (_helper.PtrSize * index));
    }

    private protected string ClassName(nint klass)
    {
        return ReadStr(ReadPtr(klass + _engine["MonoClass"]["name"]), 128);
    }

    private protected string ClassNameSpace(nint klass)
    {
        return ReadStr(ReadPtr(klass + _engine["MonoClass"]["name_space"]), 256);
    }

    private protected bool ClassHasFields(nint klass, out nint fields, out int fieldCount)
    {
        fields = ReadPtr(klass + _engine["MonoClass"]["fields"]);
        fieldCount = ClassFieldCount(klass);
        return fields != 0 && fieldCount > 0;
    }
    #endregion

    #region Fields
    private protected List<MonoField> GetAllFields(nint klass)
    {
        var fields = new List<MonoField>();
        foreach (var field in Fields(klass))
        {
            var attrs = FieldAttrs(field);

            fields.Add(new()
            {
                Name = FieldName(field),
                Offset = FieldOffset(field),
                IsConst = attrs.HasFlag(MonoFieldAttribute.MONO_FIELD_ATTR_LITERAL),
                IsStatic = attrs.HasFlag(MonoFieldAttribute.MONO_FIELD_ATTR_STATIC)
            });
        }

        return fields;
    }

    private protected IEnumerable<nint> Fields(nint klass)
    {
        if (!ClassHasFields(klass, out var fields, out var fieldCount))
        {
            Debug.Log("  => No fields.");
            yield break;
        }

        var fieldSize = _engine["MonoClassField"]["size"];
        for (int i = 0; i < fieldCount; i++)
            yield return fields + (fieldSize * i);
    }

    private protected string FieldName(nint field)
    {
        var name = ReadStr(ReadPtr(field + _engine["MonoClassField"]["name"]), 128);
        var split = name.Split('<', '>');
        if (split.Length == 3 && !string.IsNullOrEmpty(split[1]))
            name = split[1];

        return name;
    }

    private protected int FieldOffset(nint field)
    {
        return ReadI32(field + _engine["MonoClassField"]["offset"]);
    }

    private protected MonoFieldAttribute FieldAttrs(nint field)
    {
        var monoType = ReadPtr(field + _engine["MonoClassField"]["type"]);
        return _helper.Read<MonoFieldAttribute>(monoType + _engine["MonoType"]["attrs"]);
    }
    #endregion

    #region Helpers
    private protected nint ReadRel(nint address)
    {
        return address + 0x4 + _helper.Read<int>(address);
    }

    private protected nint ReadPtr(nint address)
    {
        return _helper.Read<nint>(address);
    }

    private protected int ReadI32(nint address)
    {
        return _helper.Read<int>(address);
    }

    private protected uint ReadU32(nint address)
    {
        return _helper.Read<uint>(address);
    }

    private protected ushort ReadU16(nint address)
    {
        return _helper.Read<ushort>(address);
    }

    private protected byte ReadI8(nint address)
    {
        return _helper.Read<byte>(address);
    }

    private protected string ReadStr(nint address, int length)
    {
        return _helper.ReadString(length, ReadStringType.UTF8, address);
    }
    #endregion
}
