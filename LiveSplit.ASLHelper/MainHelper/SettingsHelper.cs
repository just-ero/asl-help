using System;
using System.Collections.Generic;
using System.Linq;

namespace ASLHelper.MainHelper
{
    public class SettingsHelper
    {
        const string NULL_STRING = null;
        static readonly Func<string, bool, string, string, string, Tuple<string, bool, string, string, string>> SETTINGS_TUPLE = Tuple.Create;

        public SettingsHelper(object builder)
        {
            _builder = builder;
        }

        private readonly dynamic _builder;

        public void Create(Dictionary<string, string> settings, bool defaultValue = true, string defaultParent = null)
        {
            var converted = settings?.Select(kvp => SETTINGS_TUPLE(kvp.Key, defaultValue, kvp.Value, defaultParent, NULL_STRING)).ToArray();
            Create(converted, defaultParent);
        }

        public void Create(Dictionary<string, bool> settings, string defaultParent = null)
        {
            var converted = settings?.Select(kvp => SETTINGS_TUPLE(kvp.Key, kvp.Value, kvp.Key, defaultParent, NULL_STRING)).ToArray();
            Create(converted, defaultParent);
        }

        public void Create(params string[] settings)
        {
            Create(settings, true, null);
        }

        public void Create(IList<string> settings, bool defaultValue = true, string defaultParent = null)
        {
            var converted = settings?.Select(s => SETTINGS_TUPLE(s, defaultValue, s, defaultParent, NULL_STRING)).ToArray();
            Create(converted, defaultParent);
        }

        public void Create(IList<Tuple<string, string>> settings, bool defaultValue = true, string defaultParent = null)
        {
            var converted = settings?.Select(t => SETTINGS_TUPLE(t.Item1, defaultValue, t.Item2, defaultParent, NULL_STRING)).ToArray();
            Create(converted, defaultParent);
        }

        public void Create(IList<Tuple<string, bool, string>> settings, string defaultParent = null)
        {
            var converted = settings?.Select(t => SETTINGS_TUPLE(t.Item1, t.Item2, t.Item1, t.Item3, NULL_STRING)).ToArray();
            Create(converted, defaultParent);
        }

        public void Create(IList<Tuple<string, bool, string, string>> settings, string defaultParent = null)
        {
            var converted = settings?.Select(t => SETTINGS_TUPLE(t.Item1, t.Item2, t.Item3, t.Item4, NULL_STRING)).ToArray();
            Create(converted, defaultParent);
        }

        public void Create(IList<Tuple<string, bool, string, string, string>> settings, string defaultParent = null)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings), "The settings collection was null.");

            for (int i = 0; i < settings.Count; i++)
            {
                var setting = settings[i];
                if (setting == null)
                    throw new ArgumentNullException("settings collection", $"The element at index {i} in the collection was null.");

                if (setting.Item1 == null || setting.Item3 == null)
                    throw new ArgumentNullException("settings collection", $"An item in the element at index {i} was null.");

                _builder.Add(setting.Item1, setting.Item2, setting.Item3, setting.Item4 ?? defaultParent);
                if (!string.IsNullOrEmpty(setting.Item5))
                    _builder.SetToolTip(setting.Item1, setting.Item5);
            }
        }

        public void Create(dynamic[,] settings, bool defaultValue = true, string defaultParent = null)
        {
            var (outerCount, innerCount) = (settings.GetLength(0), settings.GetLength(1));
            var converted = new Tuple<string, bool, string, string, string>[outerCount];

            switch (innerCount)
            {
                case 1:
                {
                    for (int i = 0; i < outerCount; i++)
                        converted[i] = SETTINGS_TUPLE(settings[i, 0], defaultValue, settings[i, 0], defaultParent, NULL_STRING);

                    break;
                }
                case 2:
                {
                    for (int i = 0; i < outerCount; i++)
                        converted[i] = SETTINGS_TUPLE(settings[i, 0], defaultValue, settings[i, 1], defaultParent, NULL_STRING);

                    break;
                }
                case 3:
                {
                    for (int i = 0; i < outerCount; i++)
                        converted[i] = SETTINGS_TUPLE(settings[i, 0], settings[i, 1], settings[i, 0], settings[i, 2], NULL_STRING);

                    break;
                }
                case 4:
                {
                    for (int i = 0; i < outerCount; i++)
                        converted[i] = SETTINGS_TUPLE(settings[i, 0], settings[i, 1], settings[i, 2], settings[i, 3], NULL_STRING);

                    break;
                }
                case 5:
                {
                    for (int i = 0; i < outerCount; i++)
                        converted[i] = SETTINGS_TUPLE(settings[i, 0], settings[i, 1], settings[i, 2], settings[i, 3], settings[i, 4]);

                    break;
                }
                default:
                {
                    var msg = $"Jagged settings array must have anywhere from 1 to 5 inner elements (was {innerCount}).";
                    throw new ArgumentException(msg);
                }
            }

            Create(converted, defaultParent);
        }

        public void Create(dynamic[][] settings, bool defaultValue = true, string defaultParent = null)
        {
            var count = settings.Length;
            var converted = new Tuple<string, bool, string, string, string>[count];

            for (int i = 0; i < count; i++)
            {
                var setting = settings[i];
                if (setting == null)
                    throw new ArgumentNullException("settings collection", $"The element at index {i} in the collection was null.");

                switch (setting.Length)
                {
                    case 1:
                    {
                        converted[i] = SETTINGS_TUPLE(setting[0], defaultValue, setting[0], defaultParent, NULL_STRING);
                        break;
                    }
                    case 2:
                    {
                        converted[i] = SETTINGS_TUPLE(setting[0], defaultValue, setting[1], defaultParent, NULL_STRING);
                        break;
                    }
                    case 3:
                    {
                        converted[i] = SETTINGS_TUPLE(setting[0], setting[1], setting[0], setting[2], NULL_STRING);
                        break;
                    }
                    case 4:
                    {
                        converted[i] = SETTINGS_TUPLE(setting[0], setting[1], setting[2], setting[3], NULL_STRING);
                        break;
                    }
                    case 5:
                    {
                        converted[i] = SETTINGS_TUPLE(setting[0], setting[1], setting[2], setting[3], setting[4]);
                        break;
                    }
                    default:
                    {
                        var msg = $"Two-dimensional settings array must have anywhere from 1 to 5 inner elements (was {setting.Length}).";
                        throw new ArgumentException(msg);
                    }
                }
            }
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
            if (positions.Any(p => p < 1 || p > 5))
                throw new ArgumentException("The positions must be in range 1 - 5.");

            var (outerCount, innerCount) = (settings.GetLength(0), settings.GetLength(1));
            if (positions.Length != innerCount)
            {
                var msg = $"Amount of positions for settings ({positions.Length}) did not equal collection's inner elements' count ({innerCount}).";
                throw new ArgumentException(msg);
            }

            var converted = new Tuple<string, bool, string, string, string>[outerCount];

            for (int i = 0; i < outerCount; i++)
            {
                var sorted = new dynamic[5];
                for (int j = 0; j < innerCount; j++)
                    sorted[positions[j] - 1] = settings[i, j];

                var id = sorted[0] ?? sorted[2];
                var state = sorted[1] ?? defaultValue;
                var desc = sorted[2] ?? sorted[0];
                var parent = sorted[3];
                var tooltip = sorted[4];

                converted[i] = SETTINGS_TUPLE(id, state, desc, parent, tooltip);
            }

            Create(converted, defaultParent);
        }
    }
}