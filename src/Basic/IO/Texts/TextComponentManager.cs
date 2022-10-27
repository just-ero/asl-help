using LiveSplit.UI.Components;

namespace AslHelp.IO.Texts;

public class TextComponentManager
{
    private readonly Dictionary<string, TextDisplay> _components = new();

    internal TextComponentManager() { }

    public TextDisplay this[string id]
    {
        get
        {
            TextDisplay td;

            if (timer.Layout.TryFindLayoutComponent("TextComponent", id, out _) is ILayoutComponent lc)
            {
                if (_components.TryGetValue(id, out td))
                {
                    return td;
                }

                td = new(id, lc);
                _components[id] = td;

                return td;
            }

            td = new(id);
            _components[id] = td;

            return td;
        }
        set
        {
            Remove(id);

            if (value is null)
            {
                return;
            }

            value.ID = id;
            value.ComponentSettings.Tag = id;
            _components[id] = value;
        }
    }

    public void Remove(string id)
    {
        if (!_components.TryGetValue(id, out TextDisplay component))
        {
            return;
        }

        timer.Layout.LayoutComponents.Remove(component.LayoutComponent);
        _components.Remove(id);
    }

    public void RemoveAll()
    {
        foreach (KeyValuePair<string, TextDisplay> entry in _components)
        {
            if (timer.Layout.FindLayoutComponent(entry.Key) is ILayoutComponent component)
            {
                timer.Layout.LayoutComponents.Remove(component);
            }
        }

        _components.Clear();
    }

    public TextDisplay Find(string text1 = "", string text2 = "")
    {
        foreach (ILayoutComponent tc in timer.Layout.LayoutComponentsOfType("TextComponent"))
        {
            dynamic tcc = tc.Component;
            bool noe1 = string.IsNullOrEmpty(text1), noe2 = string.IsNullOrEmpty(text2);

            if (noe1 && noe2)
            {
                continue;
            }

            if (!noe1 && tcc.Settings.Text1 != text1)
            {
                continue;
            }

            if (!noe2 && tcc.Settings.Text2 != text2)
            {
                continue;
            }

            return new(tc);
        }

        return null;
    }
}
