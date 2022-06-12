using System.Xml.Linq;

namespace ASLHelper;

internal class EngineReflection
{
    public Dictionary<string, Dictionary<string, int>> Structs { get; } = new();
    public Dictionary<string, SigScanTarget> Signatures { get; } = new();

    public Dictionary<string, int> this[string structName]
    {
        get
        {
            if (!Structs.ContainsKey(structName))
                Structs[structName] = new();

            return Structs[structName];
        }
        set => Structs[structName] = value;
    }

    public static EngineReflection Load(string engine, string type, string version)
    {
        var xml = new EngineReflection();

        var resource = $"{type}_{version}_{(Data.s_Helper.Is64Bit ? "x64" : "x86")}";
        var path = $"ASLHelper.{engine}Helper.Structs.{resource}.xml";

        Debug.Log($"  => Loading '{resource}'...");

        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
        using var reader = new StreamReader(stream);

        var doc = XDocument.Parse(reader.ReadToEnd());

        var structs = doc.Element("mono").Element("structs").Elements();
        var sigs = doc.Element("mono").Element("signatures").Elements();

        foreach (var str in structs)
        {
            var sName = str.Name.ToString();

            foreach (var field in str.Elements())
            {
                var fName = field.Name.ToString();
                var fOffset = Convert.ToInt32(field.Attribute("offset").Value, 16);

                xml[sName][fName] = fOffset;
            }
        }

        foreach (var sig in sigs)
        {
            var sigName = sig.Name.ToString();
            if (!xml.Signatures.ContainsKey(sigName))
                xml.Signatures[sigName] = new() { OnFound = Data.s_OnFound };

            var sigOffset = int.Parse(sig.Attribute("offset").Value);
            var sigPattern = sig.Attribute("pattern").Value;

            xml.Signatures[sigName].AddSignature(sigOffset, sigPattern);
        }

        Debug.Log($"    => Success.");

        return xml;
    }
}
