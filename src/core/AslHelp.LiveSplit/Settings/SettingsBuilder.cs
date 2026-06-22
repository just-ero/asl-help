using LiveSplit.ASL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AslHelp.LiveSplit.Settings;

/// <summary>
///     Builds the script's settings tree, forwarding entries to LiveSplit's
///     <see cref="ASLSettingsBuilder"/>.
/// </summary>
/// <param name="builder">The LiveSplit settings builder to forward entries to.</param>
public sealed partial class SettingsBuilder(ASLSettingsBuilder builder)
{
    /// <summary>
    ///     Adds each entry as a setting whose id is the key and label is the value.
    /// </summary>
    /// <param name="settings">The id-to-label settings to add.</param>
    public void Add(Dictionary<string, string> settings)
    {
        var converted = settings.Select(kvp => new Setting(kvp.Key, false, kvp.Value, null, null));

        Add(converted);
    }

    /// <summary>
    ///     Adds settings from a 2-D array where each row supplies one to five values describing a
    ///     setting (id, state, label, parent, tooltip).
    /// </summary>
    /// <param name="settings">The settings rows to add.</param>
    /// <exception cref="ArgumentException">A row has zero or more than five columns.</exception>
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
    public void Add(dynamic?[,] settings)
#pragma warning restore CA1814
    {
        ArgumentNullException.ThrowIfNull(settings);

#pragma warning disable CA1062 // Validate arguments of public methods
        var converted = ConvertFromDynamic(settings);
#pragma warning restore CA1062
        Add(converted);
    }

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
    private static IEnumerable<Setting> ConvertFromDynamic(dynamic?[,] settings)
#pragma warning restore CA1814
    {
        var (outerCount, innerCount) = (settings.GetLength(0), settings.GetLength(1));

        for (int i = 0; i < outerCount; i++)
        {
            if (innerCount is <= 0 or > 5)
            {
                ArgumentException.Throw(
                    $"settings[{i}]",
                    $"Incorrect number of arguments provided in settings ({innerCount}).");
            }

#pragma warning disable CS8509 // The switch expression does not handle all possible inputs (it is not exhaustive).
            yield return innerCount switch
#pragma warning restore CS8509
            {
                1 => new(settings[i, 0], false, settings[i, 0], null, null),
                2 => new(settings[i, 0], false, settings[i, 1], null, null),
                3 => new(settings[i, 0], settings[i, 1], settings[i, 0], settings[i, 2], null),
                4 => new(settings[i, 0], settings[i, 1], settings[i, 2], settings[i, 3], null),
                5 => new(settings[i, 0], settings[i, 1], settings[i, 2], settings[i, 3], settings[i, 4]),
            };
        }
    }

    private void Add(IEnumerable<Setting> settings)
    {
        foreach (Setting setting in settings)
        {
            ArgumentNullException.ThrowIfNull(setting.Id, paramName: "ASLSetting.Id");
            ArgumentNullException.ThrowIfNull(setting.Label, paramName: "ASLSetting.Label");

            builder.Add(setting.Id, setting.State, setting.Label, setting.Parent);
            if (!string.IsNullOrWhiteSpace(setting.Tooltip))
            {
                builder.SetToolTip(setting.Id, setting.Tooltip);
            }
        }
    }
}
