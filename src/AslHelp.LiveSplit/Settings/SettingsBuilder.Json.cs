using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace AslHelp.LiveSplit.Settings;

public partial class SettingsBuilder
{
    [DataContract]
    private sealed record JsonSetting(
        [property: DataMember(Name = "label")] string? Label,
        [property: DataMember(Name = "state")] bool State,
        [property: DataMember(Name = "tooltip")] string? ToolTip,
        [property: DataMember(Name = "children")] Dictionary<string, JsonSetting>? Children);

    public void CreateFromJson(string path)
    {
        using FileStream fs = File.OpenRead(path);
        DataContractJsonSerializer serializer = new(typeof(Dictionary<string, JsonSetting>));

        if (serializer.ReadObject(fs) is Dictionary<string, JsonSetting> { Count: > 0 } settings)
        {
            Add(ConvertFromJson(settings));
        }
    }

    private IEnumerable<Setting> ConvertFromJson(Dictionary<string, JsonSetting> settings, string? parent = null)
    {
        foreach (KeyValuePair<string, JsonSetting> setting in settings)
        {
            string id = setting.Key;
            JsonSetting value = setting.Value;

            yield return new(
                Id: id,
                State: value.State,
                Label: value.Label ?? id,
                ToolTip: value.ToolTip,
                Parent: parent);

            if (value.Children is { Count: > 0 } children)
            {
                foreach (Setting child in ConvertFromJson(children, id))
                {
                    yield return child;
                }
            }
        }
    }
}
