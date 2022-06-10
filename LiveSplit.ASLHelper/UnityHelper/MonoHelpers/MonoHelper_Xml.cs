namespace ASLHelper.UnityHelper;

public abstract partial class MonoHelper
{
    protected class Xml
    {
        public Dictionary<string, Dictionary<string, int>> Structs { get; } = new();
        public Dictionary<string, SigScanTarget> Signatures { get; } = new();

        public Dictionary<string, int> this[string structName]
        {
            get => Structs[structName];
        }

        public Dictionary<string, Dictionary<string, string>> _structs
        {
            set
            {
                foreach (var s in value)
                {
                    var (structName, fields) = (s.Key, s.Value);

                    Structs[structName] = new();

                    foreach (var f in fields)
                    {
                        var (fieldName, fieldOffset) = (f.Key, f.Value);

                        Structs[structName][fieldName] = Convert.ToInt32(fieldOffset, 16);
                    }
                }
            }
        }

        public Dictionary<string, string> _signatures
        {
            set
            {
                foreach (var sig in value)
                {
                    var (name, target) = (sig.Key, sig.Value);
                    var split = target.Split(',');

                    Signatures[name] = new(int.Parse(split[0]), split[1]);
                }
            }
        }

        public static Xml Load(string type, string version)
        {
            var resource = $"{type}_{version}_{(Data.s_Helper.Is64Bit ? "x64" : "x86")}";

            Debug.Log($"  => Loading '{resource}'...");

            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"LiveSplit.ASLHelper.UnityHelper.Structs.{resource}.json");
            using var reader = new StreamReader(stream);

            var xml = reader.ReadToEnd();

            Debug.Log($"    => Success.");

            return null;
        }
    }
}
