using LiveSplit.UI.Components;
using System.Collections.Generic;

namespace ASLHelper
{
    public class TextComponentHelper
    {
        private readonly Dictionary<string, TextComponent> _components = new Dictionary<string, TextComponent>();

        public TextComponent this[string id]
        {
            get
            {
                TextComponent tc;

                if (Data.FindLayoutComponent("TextComponent", id, out _) is ILayoutComponent lc)
                {
                    if (_components.TryGetValue(id, out tc))
                        return tc;

                    tc = new TextComponent(id, lc);
                    _components[id] = tc;
                    return tc;
                }

                tc = new TextComponent(id);
                _components[id] = tc;

                return tc;
            }
            set
            {
                if (value == null)
                    Remove(id);
            }
        }

        public void Remove(string id)
        {
            if (!_components.TryGetValue(id, out var component))
                return;

            Data.s_LayoutComponents.Remove(component.LayoutComponent);
            _components.Remove(id);
        }

        public void RemoveAll()
        {
            foreach (var entry in _components)
            {
                if (Data.FindLayoutComponent(entry.Key) is ILayoutComponent component)
                    Data.s_LayoutComponents.Remove(component);
            }

            _components.Clear();
        }
    }
}