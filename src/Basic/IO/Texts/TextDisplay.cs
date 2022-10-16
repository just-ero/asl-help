using LiveSplit.UI.Components;

namespace AslHelp.IO.Texts;

public class TextDisplay
{
    private static readonly IComponentFactory _textComponentFactory = ComponentManager.ComponentFactories["LiveSplit.Text.dll"];

    internal TextDisplay(string id)
    {
        dynamic component = _textComponentFactory.Create(timer.State);
        LayoutComponent = new("LiveSplit.Text.dll", component);
        ComponentSettings = component.Settings;
        ID = id;

        ComponentSettings.Tag = id;

        timer.Layout.LayoutComponents.Add(LayoutComponent);
    }

    internal TextDisplay(ILayoutComponent component)
    {
        LayoutComponent = (LayoutComponent)component;
        ComponentSettings = (component.Component as dynamic).Settings;
    }

    internal TextDisplay(string id, ILayoutComponent component)
    {
        LayoutComponent = component as LayoutComponent;
        ComponentSettings = (component.Component as dynamic).Settings;
        ID = id;
    }

    public string ID { get; internal set; }
    internal LayoutComponent LayoutComponent { get; }
    internal dynamic ComponentSettings { get; }

    public dynamic Left
    {
        get => ComponentSettings.Text1;
        set => ComponentSettings.Text1 = value.ToString();
    }

    public dynamic Right
    {
        get => ComponentSettings.Text2;
        set => ComponentSettings.Text2 = value.ToString();
    }
}
