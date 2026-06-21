using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace AslHelp.LiveSplit.Settings;

public sealed partial class SettingsBuilder
{
    [XmlRoot("Setting")]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
        Justification = "Instantiated by the XML deserializer via reflection.")]
    private sealed class XmlSetting
    {
        [XmlAttribute] public required string Id { get; set; }
        [XmlAttribute] public string? Label { get; set; }
        [XmlAttribute] public string? State { get; set; }
        [XmlAttribute] public string? ToolTip { get; set; }

        [XmlElement("Setting")] public XmlSetting[]? Children { get; set; }
    }

    /// <summary>
    ///     Loads settings from the XML file at <paramref name="path"/> and adds them.
    /// </summary>
    /// <param name="path">The path of the XML settings file.</param>
    public void FromXml(string path)
    {
        using var fs = File.OpenRead(path);
        using var reader = XmlReader.Create(fs);
        var ser = new XmlSerializer(typeof(XmlSetting[]), root: new("Settings"));
        if (ser.Deserialize(reader) is not XmlSetting[] settings)
        {
            FormatException.Throw(
                "Xml settings file was in an incorrect format.");
            return;
        }

        var converted = ConvertFromXml(settings);
        Add(converted);
    }

    private static IEnumerable<Setting> ConvertFromXml(XmlSetting[] nodes, string? parent = null)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            XmlSetting node = nodes[i];
            yield return new(
                Id: node.Id,
                State: bool.TryParse(node.State, out bool state) ? state : false,
                Label: node.Label ?? node.Id,
                Parent: parent,
                Tooltip: node.ToolTip);

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
