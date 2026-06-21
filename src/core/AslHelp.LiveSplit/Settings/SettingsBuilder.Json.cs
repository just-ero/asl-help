using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AslHelp.LiveSplit.Settings;

public sealed partial class SettingsBuilder
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes",
        Justification = "Instantiated by the JSON deserializer via reflection.")]
    private sealed record JsonSetting(
        [property: JsonPropertyName("label")] string? Label,
        [property: JsonPropertyName("state")] bool State,
        [property: JsonPropertyName("tooltip")] string? ToolTip,
        [property: JsonPropertyName("children")] Dictionary<string, JsonSetting>? Children);

    /// <summary>
    ///     Loads settings from the JSON file at <paramref name="path"/> and adds them.
    /// </summary>
    /// <param name="path">The path of the JSON settings file.</param>
    public void FromJson(string path)
    {
        using FileStream fs = File.OpenRead(path);
        var settings = JsonSerializer.Deserialize<Dictionary<string, JsonSetting>>(fs, _jsonOptions);

        if (settings is { Count: > 0 })
        {
            var converted = ConvertFromJson(settings);
            Add(converted);
        }
    }

    private static IEnumerable<Setting> ConvertFromJson(Dictionary<string, JsonSetting> settings, string? parent = null)
    {
        foreach (var setting in settings)
        {
            string id = setting.Key;
            JsonSetting value = setting.Value;

            yield return new(
                Id: id,
                State: value.State,
                Label: value.Label ?? id,
                Tooltip: value.ToolTip,
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
