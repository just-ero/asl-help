using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace AslHelp.LiveSplit.Settings;

public sealed partial class SettingsBuilder
{
    /// <summary>
    ///     Loads settings from the XML file at <paramref name="path"/> and adds them.
    /// </summary>
    /// <param name="path">The path of the XML settings file.</param>
    public void FromXml(string path)
    {
        if (XDocument.Load(path).Root is not { } root)
        {
            return;
        }

        Add(ConvertFromXml(root.Elements("Setting"), null));
    }

    private static IEnumerable<Setting> ConvertFromXml(IEnumerable<XElement> nodes, string? parent)
    {
        foreach (var node in nodes)
        {
            if ((string?)node.Attribute("Id") is not { Length: > 0 } id)
            {
                FormatException.Throw(
                    "Xml settings file was in an incorrect format: a <Setting> is missing its 'Id' attribute.");
                yield break;
            }

            var label = (string?)node.Attribute("Label");

            yield return new(
                Id: id,
                State: bool.TryParse((string?)node.Attribute("State"), out var state) && state,
                Label: label ?? id,
                Parent: parent,
                Tooltip: (string?)node.Attribute("ToolTip"));

            List<XElement> children = [.. node.Elements("Setting")];
            if (children.Count > 0)
            {
                foreach (var child in ConvertFromXml(children, id))
                {
                    yield return child;
                }
            }
        }
    }
}
