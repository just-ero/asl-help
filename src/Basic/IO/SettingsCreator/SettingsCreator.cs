namespace AslHelp.IO;

public partial class SettingsCreator
{
    private static readonly Func<string, bool, string, string, string, Tuple<string, bool, string, string, string>> _tuple = Tuple.Create;

    public void Create(Dictionary<string, string> settings, bool defaultValue = true, string defaultParent = null)
    {
        Tuple<string, bool, string, string, string>[] converted = settings?.Select(kvp => _tuple(kvp.Key, defaultValue, kvp.Value, defaultParent, null)).ToArray();
        Create(converted, defaultParent);
    }

    public void Create(Dictionary<string, bool> settings, string defaultParent = null)
    {
        Tuple<string, bool, string, string, string>[] converted = settings?.Select(kvp => _tuple(kvp.Key, kvp.Value, kvp.Key, defaultParent, null)).ToArray();
        Create(converted, defaultParent);
    }

    public void Create(params string[] settings)
    {
        Create(settings, true, null);
    }

    public void Create(IList<string> settings, bool defaultValue = true, string defaultParent = null)
    {
        Tuple<string, bool, string, string, string>[] converted = settings?.Select(s => _tuple(s, defaultValue, s, defaultParent, null)).ToArray();
        Create(converted, defaultParent);
    }

    public void Create(IList<Tuple<string, string>> settings, bool defaultValue = true, string defaultParent = null)
    {
        Tuple<string, bool, string, string, string>[] converted = settings?.Select(t => _tuple(t.Item1, defaultValue, t.Item2, defaultParent, null)).ToArray();
        Create(converted, defaultParent);
    }

    public void Create(IList<Tuple<string, bool, string>> settings, string defaultParent = null)
    {
        Tuple<string, bool, string, string, string>[] converted = settings?.Select(t => _tuple(t.Item1, t.Item2, t.Item1, t.Item3, null)).ToArray();
        Create(converted, defaultParent);
    }

    public void Create(IList<Tuple<string, bool, string, string>> settings, string defaultParent = null)
    {
        Tuple<string, bool, string, string, string>[] converted = settings?.Select(t => _tuple(t.Item1, t.Item2, t.Item3, t.Item4, null)).ToArray();
        Create(converted, defaultParent);
    }

    public void Create(IList<Tuple<string, bool, string, string, string>> settings, string defaultParent = null)
    {
        if (SettingsBuilder is null)
        {
            return;
        }

        if (settings is null)
        {
            throw new ArgumentNullException(nameof(settings), "The settings collection was null.");
        }

        for (int i = 0; i < settings.Count; i++)
        {
            Tuple<string, bool, string, string, string> setting = settings[i];
            if (setting is null)
            {
                throw new ArgumentNullException("settings collection", $"The element at index {i} in the collection was null.");
            }

            if (setting.Item1 is null || setting.Item3 is null)
            {
                throw new ArgumentNullException("settings collection", $"An item in the element at index {i} was null.");
            }

            SettingsBuilder.Add(setting.Item1, setting.Item2, setting.Item3, setting.Item4 ?? defaultParent);
            if (!string.IsNullOrEmpty(setting.Item5))
            {
                SettingsBuilder.SetToolTip(setting.Item1, setting.Item5);
            }
        }
    }

    public void Create(dynamic[,] settings, bool defaultValue = true, string defaultParent = null)
    {
        (int outerCount, int innerCount) = (settings.GetLength(0), settings.GetLength(1));
        Tuple<string, bool, string, string, string>[] converted = new Tuple<string, bool, string, string, string>[outerCount];

        switch (innerCount)
        {
            case 1:
            {
                for (int i = 0; i < outerCount; i++)
                {
                    converted[i] = _tuple(settings[i, 0], defaultValue, settings[i, 0], defaultParent, null);
                }

                break;
            }
            case 2:
            {
                for (int i = 0; i < outerCount; i++)
                {
                    converted[i] = _tuple(settings[i, 0], defaultValue, settings[i, 1], defaultParent, null);
                }

                break;
            }
            case 3:
            {
                for (int i = 0; i < outerCount; i++)
                {
                    converted[i] = _tuple(settings[i, 0], settings[i, 1], settings[i, 0], settings[i, 2], null);
                }

                break;
            }
            case 4:
            {
                for (int i = 0; i < outerCount; i++)
                {
                    converted[i] = _tuple(settings[i, 0], settings[i, 1], settings[i, 2], settings[i, 3], null);
                }

                break;
            }
            case 5:
            {
                for (int i = 0; i < outerCount; i++)
                {
                    converted[i] = _tuple(settings[i, 0], settings[i, 1], settings[i, 2], settings[i, 3], settings[i, 4]);
                }

                break;
            }
            default:
            {
                string msg = $"Jagged settings array must have anywhere from 1 to 5 inner elements (was {innerCount}).";
                throw new ArgumentException(msg);
            }
        }

        Create(converted, defaultParent);
    }

    public void Create(dynamic[][] settings, bool defaultValue = true, string defaultParent = null)
    {
        int count = settings.Length;
        Tuple<string, bool, string, string, string>[] converted = new Tuple<string, bool, string, string, string>[count];

        for (int i = 0; i < count; i++)
        {
            dynamic[] setting = settings[i];
            if (setting is null)
            {
                throw new ArgumentNullException("settings collection", $"The element at index {i} in the collection was null.");
            }

            switch (setting.Length)
            {
                case 1:
                {
                    converted[i] = _tuple(setting[0], defaultValue, setting[0], defaultParent, null);
                    break;
                }
                case 2:
                {
                    converted[i] = _tuple(setting[0], defaultValue, setting[1], defaultParent, null);
                    break;
                }
                case 3:
                {
                    converted[i] = _tuple(setting[0], setting[1], setting[0], setting[2], null);
                    break;
                }
                case 4:
                {
                    converted[i] = _tuple(setting[0], setting[1], setting[2], setting[3], null);
                    break;
                }
                case 5:
                {
                    converted[i] = _tuple(setting[0], setting[1], setting[2], setting[3], setting[4]);
                    break;
                }
                default:
                {
                    string msg = $"Two-dimensional settings array must have anywhere from 1 to 5 inner elements (was {setting.Length}).";
                    throw new ArgumentException(msg);
                }
            }
        }

        Create(converted, defaultParent);
    }

    public void CreateCustom(dynamic[,] settings, params int[] positions)
    {
        CreateCustom(settings, true, null, positions);
    }

    public void CreateCustom(dynamic[,] settings, string defaultParent, params int[] positions)
    {
        CreateCustom(settings, true, defaultParent, positions);
    }

    public void CreateCustom(dynamic[,] settings, bool defaultValue, string defaultParent, params int[] positions)
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

        Tuple<string, bool, string, string, string>[] converted = new Tuple<string, bool, string, string, string>[outerCount];

        for (int i = 0; i < outerCount; i++)
        {
            dynamic[] sorted = new dynamic[5];
            for (int j = 0; j < innerCount; j++)
            {
                sorted[positions[j] - 1] = settings[i, j];
            }

            dynamic id = sorted[0] ?? sorted[2];
            dynamic state = sorted[1] ?? defaultValue;
            dynamic desc = sorted[2] ?? sorted[0];
            dynamic parent = sorted[3];
            dynamic tooltip = sorted[4];

            converted[i] = _tuple(id, state, desc, parent, tooltip);
        }

        Create(converted, defaultParent);
    }
}
