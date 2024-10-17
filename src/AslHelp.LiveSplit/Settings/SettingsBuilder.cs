extern alias Ls;

using System;
using System.Collections.Generic;
using System.Linq;

using AslHelp.Shared;

using Ls::LiveSplit.ASL;

namespace AslHelp.LiveSplit.Settings;

[Obsolete("Do not use ASL-specific features.", true)]
public sealed partial class SettingsBuilder
{
    private readonly ASLSettingsBuilder _builder;

    public SettingsBuilder(Autosplitter asl)
    {
        _builder = asl.SettingsBuilder;
    }

    public void Create(Dictionary<string, string> settings)
    {
        IEnumerable<Setting> converted = settings.Select(kvp => new Setting(kvp.Key, false, kvp.Value, null, null));
        Add(converted);
    }

    public void Create(dynamic?[,] settings)
    {
        IEnumerable<Setting> converted = ConvertFromDynamic(settings);
        Add(converted);
    }

    public void CreateCustom(dynamic?[,] settings, params int[] positions)
    {
        IEnumerable<Setting> converted = ConvertFromDynamic(settings, positions);
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

    private IEnumerable<Setting> ConvertFromDynamic(dynamic?[,] settings, int[] positions)
    {
        if (positions.Any(p => p is < 1 or > 5))
        {
            throw new ArgumentException("The positions must be in range 1 - 5.");
        }

        (int outerCount, int innerCount) = (settings.GetLength(0), settings.GetLength(1));

        if (positions.Length != innerCount)
        {
            string msg = $"Amount of positions for settings ({positions.Length}) did not equal collection's inner elements' count ({innerCount}).";
            throw new ArgumentException(msg);
        }

        for (int i = 0; i < outerCount; i++)
        {
            var sorted = new dynamic?[5];
            for (int j = 0; j < innerCount; j++)
            {
                sorted[positions[j] - 1] = settings[i, j];
            }

            string? id = sorted[0] ?? sorted[2];
            bool state = sorted[1] ?? true;
            string? desc = sorted[2] ?? sorted[0];
            string? parent = sorted[3];
            string? tooltip = sorted[4];

            yield return new(id, state, desc, parent, tooltip);
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
