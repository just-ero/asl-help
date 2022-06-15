using LiveSplit.UI.Components;

namespace ASLHelper.MainHelper;

public class TextComponentHelper
{
    private readonly Dictionary<string, TextComponent> _components = new();

    public TextComponent this[string id]
    {
        get
        {
            TextComponent tc;

            if (Data.FindLayoutComponent("TextComponent", id, out _) is ILayoutComponent lc)
            {
                if (_components.TryGetValue(id, out tc))
                    return tc;

                tc = new(id, lc);
                _components[id] = tc;
                return tc;
            }

            tc = new(id);
            _components[id] = tc;

            return tc;
        }
        set
        {
            Remove(id);

            if (value is null)
                return;

            value.ID = id;
            value.ComponentSettings.Tag = id;
            _components[id] = value;
        }
    }

    public void Remove(string id)
    {
        if (!_components.TryGetValue(id, out var component))
            return;

        _ = Data.s_LayoutComponents.Remove(component.LayoutComponent);
        _ = _components.Remove(id);
    }

    public void RemoveAll()
    {
        foreach (var entry in _components)
        {
            if (Data.FindLayoutComponent(entry.Key) is ILayoutComponent component)
                _ = Data.s_LayoutComponents.Remove(component);
        }

        _components.Clear();
    }
}
