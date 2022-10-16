using System.Xml.Serialization;

namespace AslHelp.IO;

public partial class SettingsCreator
{
    [XmlType("Setting")]
    public class XmlSetting
    {
        [XmlAttribute] public string Id { get; set; }
        [XmlAttribute] public string State { get; set; }
        [XmlAttribute] public string Label { get; set; }
        [XmlAttribute] public string Parent { get; set; }
        [XmlAttribute] public string ToolTip { get; set; }

        [XmlElement("Setting")] public List<XmlSetting> Children { get; set; }
    }

    public void CreateFromXml(string path, bool defaultValue = true, string defaultParent = null)
    {
        XmlSerializer ser = new(typeof(List<XmlSetting>), new XmlRootAttribute("Settings"));
        List<XmlSetting> items = ser.Deserialize(File.OpenRead(path)) as List<XmlSetting>;

        Create(GetSettingsRecursive(items, defaultValue).ToList(), defaultParent);
    }

    private IEnumerable<Tuple<string, bool, string, string, string>> GetSettingsRecursive(List<XmlSetting> settings, bool defaultValue, string defaultParent = null)
    {
        foreach (XmlSetting setting in settings)
        {
            string id = setting.Id;
            bool state = bool.TryParse(setting.State, out bool pState) ? pState : defaultValue;
            string label = setting.Label;
            string parent = setting.Parent ?? defaultParent;
            string tt = setting.ToolTip;

            yield return _tuple(id, state, label, parent, tt);

            if (setting.Children is List<XmlSetting> children && children.Count > 0)
            {
                foreach (Tuple<string, bool, string, string, string> tuple in GetSettingsRecursive(children, defaultValue, id))
                {
                    yield return tuple;
                }
            }
        }
    }
}
