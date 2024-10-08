using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AslHelp.LiveSplit.Settings;

public partial class SettingsBuilder
{
    [XmlRoot("Setting")]
    private sealed class XmlSetting
    {
        [XmlAttribute] public required string Id { get; set; }
        [XmlAttribute] public string? Label { get; set; }
        [XmlAttribute] public string? State { get; set; }
        [XmlAttribute] public string? ToolTip { get; set; }

        [XmlElement("Setting")] public XmlSetting[]? Children { get; set; }
    }

    public void FromXml(string path)
    {
        using FileStream fs = File.OpenRead(path);
        XmlSerializer ser = new(typeof(XmlSetting[]), root: new("Settings"));
        if (ser.Deserialize(fs) is not XmlSetting[] settings)
        {
            const string Msg = "Xml settings file was in an incorrect format.";
            throw new FormatException(Msg);
        }

        IEnumerable<Setting> converted = ConvertFromXml(settings);
        Add(converted);
    }

    private IEnumerable<Setting> ConvertFromXml(XmlSetting[] nodes, string? parent = null)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            XmlSetting node = nodes[i];
            yield return new(
                Id: node.Id,
                State: bool.TryParse(node.State, out bool state) ? state : false,
                Label: node.Label ?? node.Id,
                Parent: parent,
                ToolTip: node.ToolTip);

            if (node.Children is { Length: > 0 } children)
            {
                foreach (Setting setting in ConvertFromXml(children, node.Id))
                {
                    yield return setting;
                }
            }
        }
    }
}
