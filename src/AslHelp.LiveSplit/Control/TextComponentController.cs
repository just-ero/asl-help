using System.Collections;
using System.Collections.Generic;

using LiveSplit.Model;
using LiveSplit.UI.Components;

namespace AslHelp.LiveSplit.Control;

public sealed class TextComponentController : IReadOnlyDictionary<string, TextComponentWrapper>
{
    private readonly Dictionary<string, TextComponentWrapper> _textCache = [];
    private readonly LiveSplitState _state;

    public TextComponentController(LiveSplitState state)
    {
        _state = state;
    }

    public TextComponentWrapper this[string key]
    {
        get
        {
            if (!_textCache.TryGetValue(key, out TextComponentWrapper wrapper))
            {
                _textCache[key] = wrapper = new(_state);
            }

            ILayoutComponent lc = wrapper.LayoutComponent;
            IList<ILayoutComponent> components = _state.Layout.LayoutComponents;

            for (int i = components.Count - 1; i >= 0; i--)
            {
                if (components[i] == lc)
                {
                    return wrapper;
                }
            }

            _state.Layout.LayoutComponents.Add(lc);

            return wrapper;
        }
    }

    public int Count => _textCache.Count;

    public IEnumerable<string> Keys => _textCache.Keys;
    public IEnumerable<TextComponentWrapper> Values => _textCache.Values;

    public bool ContainsKey(string key)
    {
        return _textCache.ContainsKey(key);
    }

    public bool TryGetValue(string key, out TextComponentWrapper value)
    {
        return _textCache.TryGetValue(key, out value);
    }

    public void Remove(string key)
    {
        if (!_textCache.TryGetValue(key, out TextComponentWrapper wrapper))
        {
            return;
        }

        _ = _state.Layout.LayoutComponents.Remove(wrapper.LayoutComponent);
        _ = _textCache.Remove(key);
    }

    public void Clear()
    {
        foreach (TextComponentWrapper wrapper in _textCache.Values)
        {
            _ = _state.Layout.LayoutComponents.Remove(wrapper.LayoutComponent);
        }

        _textCache.Clear();
    }

    public IEnumerator<KeyValuePair<string, TextComponentWrapper>> GetEnumerator()
    {
        return _textCache.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
