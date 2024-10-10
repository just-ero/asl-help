extern alias Ls;

using Ls::LiveSplit.Model;
using Ls::LiveSplit.UI.Components;

using System.Drawing;

namespace AslHelp.LiveSplit.Control;

public sealed class TextComponentWrapper
{
    private const string TextComponentName = "LiveSplit.Text.dll";

    private readonly TextComponentSettings _settings;

    public TextComponentWrapper(LiveSplitState state)
        : this(ComponentManager.LoadLayoutComponent(TextComponentName, state)) { }

    public TextComponentWrapper(ILayoutComponent component)
    {
        LayoutComponent = component;
        _settings = ((TextComponent)component.Component).Settings;
    }

    public ILayoutComponent LayoutComponent { get; }

    public dynamic Text1
    {
        get => _settings.Text1;
        set => _settings.Text1 = value.ToString();
    }

    public dynamic Text2
    {
        get => _settings.Text2;
        set => _settings.Text2 = value.ToString();
    }

    public Font Font1
    {
        get => _settings.Font1;
        set => _settings.Font1 = value;
    }

    public Font Font2
    {
        get => _settings.Font2;
        set => _settings.Font2 = value;
    }
}
