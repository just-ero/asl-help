namespace AslHelp.MemUtils.Reflect;

internal class TypeParser
{
    private readonly EngineReflection _er;
    private readonly int _ptrSize;

    private int _offset;
    private int _bits = -1;

    private int _pAlign;
    private int _pSize = -1;

    public TypeParser(EngineReflection er)
    {
        _er = er;
        _ptrSize = Basic.Instance.Is64Bit ? 0x8 : 0x4;
    }

    private (int, int, uint) GetTypeSize(string name)
    {
        name = ReflectUtils.RemoveGeneric(name);

        if (ReflectUtils.IsNativeType(name, out int size))
        {
            _bits = -1;
            return (size, size, 0);
        }
        else if (IsKnownStruct(name, out size, out int align))
        {
            _bits = -1;
            return (size, align, 0);
        }
        else
        {
            char last = name.Length > 0 ? name[^1] : default;

            if (last == ']')
            {
                _bits = -1;

                int i = name.LastIndexOf('[');
                if (!int.TryParse(name[(i + 1)..^1], out int count))
                {
                    count = 1;
                }

                (size, align, _) = GetTypeSize(name[..i]);

                return (size * count, align, 0);
            }
            else if (char.IsDigit(last))
            {
                int i = name.IndexOf(':');

                ReflectUtils.IsNativeType(name[..i], out size);
                int bitFieldSize = int.Parse(name[(i + 1)..]);

                if (_bits is -1 || size != _pAlign)
                {
                    _bits = 0;
                }

                int tBits = _bits;
                _bits += bitFieldSize;

                align = size;
                size = _bits / 8;
                uint mask = (uint)unchecked(((1 << bitFieldSize) - 1) << tBits);

                _bits %= 8;

                return (size, align, mask);
            }
            else
            {
                _bits = -1;
                return (_ptrSize, _ptrSize, 0);
            }
        }
    }

    public (string, int, int, int, uint) ParseNext(string name)
    {
        int tBits = _bits, tOffset = _offset;
        (int size, int align, uint mask) = GetTypeSize(name);

        if (mask == 0 || tBits == -1)
        {
            if (tBits != -1 && _bits == -1 && _pSize == 0)
            {
                _offset++;
            }

            tOffset = ReflectUtils.Align(_offset, align > _pAlign ? align : _pAlign);
        }

        _offset = tOffset + size;

        _pSize = size;
        _pAlign = align;

        return (name, tOffset, size, align, mask);
    }

    private bool IsKnownStruct(string name, out int size, out int align)
    {
        if (_er.TryGetValue(name, out EngineStruct es))
        {
            size = es.Size;
            align = es.Alignment;

            return true;
        }
        else
        {
            size = _ptrSize;
            align = _ptrSize;

            return false;
        }
    }

    public void SetOffsetFromSuper(string super)
    {
        _offset = _er[ReflectUtils.RemoveGeneric(super)].Size;
    }
}
