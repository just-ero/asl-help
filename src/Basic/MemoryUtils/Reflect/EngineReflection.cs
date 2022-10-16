using System.Text;

namespace AslHelp.MemUtils.Reflect;

internal record EngineReflection
{
    private readonly Dictionary<string, EngineStruct> _structs = new();

    public static EngineReflection Load(string engine, string major, string minor)
    {
        Debug.Info($"Loading {engine} {major}.{minor} structs...");

        ReflectXml xml = ReflectXml.GetFromResources(engine, major, minor);
        EngineReflection er = new();

        foreach (dynamic @struct in xml.Structs)
        {
            TypeParser tp = new(er);

            (dynamic sName, dynamic fields) = (ReflectUtils.RemoveGeneric(@struct.Key), @struct.Value);
            EngineStruct es = new(sName);

            if (fields.TryGetValue("Super", out string super))
            {
                es.Super = super;
                tp.SetOffsetFromSuper(super);
            }

            foreach (dynamic field in fields)
            {
                (dynamic fName, dynamic type) = (field.Key, field.Value);
                if (fName == "Super")
                {
                    continue;
                }

                (string typeNoGeneric, int offset, int size, int align, uint mask) = tp.ParseNext((string)type);

                es[fName] = new EngineField(fName, type, offset, size, align, mask);
            }

            er[sName] = es;
        }

        Debug.Info("  => Success.");

        return er;
    }

    public IEnumerable<EngineStruct> Structs => _structs.Values;

    public EngineStruct this[string structName]
    {
        get
        {
            if (_structs.TryGetValue(structName, out EngineStruct @struct))
            {
                return @struct;
            }
            else
            {
                string msg = $"[EngineReflection] Struct '{structName}' was not present.";
                throw new KeyNotFoundException(msg);
            }
        }
        set => _structs[structName] = value;
    }

    public bool TryGetValue(string structName, out EngineStruct @struct)
    {
        return _structs.TryGetValue(structName, out @struct);
    }

    public override string ToString()
    {
        StringBuilder strBuilder = new();

        foreach (EngineStruct es in Structs)
        {
            if (es.Super is not null)
            {
                strBuilder.AppendLine($"{es.Name,-24} : {es.Super,-24} // 0x{es.Size:X3} (0x{es.SelfAlignedSize:X3})");
            }
            else
            {
                strBuilder.AppendLine($"{es.Name,-24} // 0x{es.Size:X3} (0x{es.SelfAlignedSize:X3})");
            }

            foreach (EngineField ef in es.Fields)
            {
                if (ef.BitMask > 0)
                {
                    strBuilder.AppendLine($"    {ef.Type,-32} {ef.Name,-32} // 0x{ef.Offset:X3} (0x{ef.Size:X3}, 0b{Convert.ToString(ef.BitMask, 2).PadLeft(8, '0')})");
                }
                else
                {
                    strBuilder.AppendLine($"    {ef.Type,-32} {ef.Name,-32} // 0x{ef.Offset:X3} (0x{ef.Size:X3})");
                }
            }
        }

        return strBuilder.ToString();
    }
}
