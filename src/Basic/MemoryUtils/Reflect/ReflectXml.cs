using System.Collections.Specialized;
using System.Xml.Linq;

namespace AslHelp.MemUtils.Reflect;

internal class ReflectXml
{
    private int _insertAt = -1;

    public OrderedDictionary Structs { get; } = new();

    public static ReflectXml GetFromResources(string engine, int major, int minor)
    {
        return GetFromResources(engine, major.ToString(), minor.ToString());
    }

    public static ReflectXml GetFromResources(string engine, string major, string minor)
    {
        string resource = $"{major}_{minor}";
        string path = $"AslHelp.{engine}.Structs.{resource}.xml";

        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
        if (stream is null)
        {
            string msg = "Resource could not be found.";
            throw new FileNotFoundException(msg, resource);
        }

        XDocument xDoc = XDocument.Load(stream);
        if (xDoc.Root is not XElement root)
        {
            string msg = "XML file was improperly formatted.";
            throw new FormatException(msg);
        }

        ReflectXml refXml = CreateFromInherited(root.Element("Inherit"));
        refXml.AddStructs(engine is "Unreal", root.Elements("Struct"));

        return refXml;
    }

    private static ReflectXml CreateFromInherited(XElement inherit)
    {
        if (inherit is null)
        {
            return new();
        }

        if (inherit.Attribute("Engine")?.Value is not string inheritEngine)
        {
            string msg = "<Inherit/> tag's 'Engine' attribute was not provided.";
            throw new FormatException(msg);
        }

        if (inherit.Attribute("Major")?.Value is not string inheritMajor)
        {
            string msg = "<Inherit/> tag's 'Major' attribute was not provided.";
            throw new FormatException(msg);
        }

        if (inherit.Attribute("Minor")?.Value is not string inheritMinor)
        {
            string msg = "<Inherit/> tag's 'Minor' attribute was not provided.";
            throw new FormatException(msg);
        }

        return GetFromResources(inheritEngine, inheritMajor, inheritMinor);
    }

    private void AddStructs(bool insert, IEnumerable<XElement> structElements)
    {
        _insertAt = -1;

        foreach (XElement structElement in structElements)
        {
            if (structElement.Attribute("Name")?.Value is not string structName)
            {
                string msg = "<Struct/> tag's 'Name' attribute must be provided.";
                throw new FormatException(msg);
            }

            Dictionary<string, string> fields = new();

            if (Contains(structName, out int index))
            {
                _insertAt = index;
                Structs[structName] = fields;
            }
            else if (insert && _insertAt != -1)
            {
                Structs.Insert(_insertAt, structName, fields);
            }
            else
            {
                Structs[structName] = fields;
            }

            if (structElement.Attribute("Super")?.Value is string structSuper)
            {
                fields["Super"] = structSuper;
            }

            foreach (XElement fieldElement in structElement.Elements("Field"))
            {
                if (fieldElement.Attribute("Name")?.Value is not string fieldName)
                {
                    string msg = "<Field/> tag's 'Name' attribute must be provided.";
                    throw new FormatException(msg);
                }

                if (fieldElement.Attribute("Type")?.Value is not string fieldType)
                {
                    string msg = "<Field/> tag's 'Type' attribute must be provided.";
                    throw new FormatException(msg);
                }

                fields[fieldName] = fieldType;
            }
        }
    }

    private bool Contains(string structName, out int index)
    {
        index = FindIndex(structName);
        return index != -1;
    }

    private int FindIndex(string structName)
    {
        int i = 0;

        foreach (dynamic @struct in Structs)
        {
            if (@struct.Key == structName)
            {
                return i;
            }

            i++;
        }

        return -1;
    }
}
