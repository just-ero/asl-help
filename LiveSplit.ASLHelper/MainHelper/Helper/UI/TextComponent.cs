using LiveSplit.UI.Components;

namespace ASLHelper;

public class TextComponent
{
    public static IComponentFactory s_TextComponentFactory = ComponentManager.ComponentFactories["LiveSplit.Text.dll"];

    public TextComponent(string id)
    {
        var component = s_TextComponentFactory.Create(Data.s_State);

        LayoutComponent = new("LiveSplit.Text.dll", component);
        ComponentSettings = (component as dynamic).Settings;
        ID = id;

        ComponentSettings.Tag = id;

        Data.s_LayoutComponents.Add(LayoutComponent);
    }

    public TextComponent(string id, ILayoutComponent component)
    {
        LayoutComponent = (LayoutComponent)component;
        ComponentSettings = (component.Component as dynamic).Settings;
        ID = id;
    }

    public readonly LayoutComponent LayoutComponent;
    public readonly dynamic ComponentSettings;

    public string ID { get; private set; }

    public object Text1
    {
        get => ComponentSettings.Text1;
        set => ComponentSettings.Text1 = value.ToString();
    }

    public object Text2
    {
        get => ComponentSettings.Text2;
        set => ComponentSettings.Text2 = value.ToString();
    }
}