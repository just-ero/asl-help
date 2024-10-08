using System;
using System.Collections.Generic;
using System.Linq;

using AslHelp.Shared;

using LiveSplit.ASL;

namespace AslHelp.LiveSplit.Settings;

#error Add remaining overloads.

[Obsolete("Do not use ASL-specific features.", true)]
public sealed partial class SettingsBuilder
{
    private readonly ASLSettingsBuilder _builder;

    public SettingsBuilder(ASLSettingsBuilder builder)
    {
        _builder = builder;
    }

    public void Add(Dictionary<string, string> settings)
    {
        IEnumerable<Setting> converted = settings.Select(kvp => new Setting(kvp.Key, false, kvp.Value, null, null));
        Add(converted);
    }

    public void Add(dynamic?[,] settings)
    {
        IEnumerable<Setting> converted = ConvertFromDynamic(settings);
        Add(converted);
    }

    private IEnumerable<Setting> ConvertFromDynamic(dynamic?[,] settings)
    {
        (int outerCount, int innerCount) = (settings.GetLength(0), settings.GetLength(1));

        for (int i = 0; i < outerCount; i++)
        {
            if (innerCount is <= 0 or > 5)
            {
                ThrowHelper.ThrowArgumentException(
                    $"settings[{i}]",
                    $"Incorrect number of arguments provided in settings ({innerCount}).");
            }

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            yield return innerCount switch
#pragma warning restore CS8509
            {
                1 => new(settings[i, 0], false, settings[i, 0], null, null),
                2 => new(settings[i, 0], false, settings[i, 1], null, null),
                3 => new(settings[i, 0], settings[i, 1], settings[i, 0], settings[i, 2], null),
                4 => new(settings[i, 0], settings[i, 1], settings[i, 2], settings[i, 3], null),
                5 => new(settings[i, 0], settings[i, 1], settings[i, 2], settings[i, 3], settings[i, 4])
            };
        }
    }

    private void Add(IEnumerable<Setting> settings)
    {
        foreach (Setting setting in settings)
        {
            ThrowHelper.ThrowIfNull(setting.Id, paramName: "ASLSetting.Id");
            ThrowHelper.ThrowIfNull(setting.Label, paramName: "ASLSetting.Label");

            _builder.Add(setting.Id, setting.State, setting.Label, setting.Parent);
            if (!string.IsNullOrWhiteSpace(setting.ToolTip))
            {
                _builder.SetToolTip(setting.Id, setting.ToolTip);
            }
        }
    }
}
