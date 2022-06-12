using LiveSplit.UI.Components;

namespace ASLHelper.MainHelper;

public class TextComponent
{
    public static IComponentFactory s_TextComponentFactory = ComponentManager.ComponentFactories["LiveSplit.Text.dll"];

    internal TextComponent(string id)
    {
        var component = s_TextComponentFactory.Create(Data.s_State);

        LayoutComponent = new("LiveSplit.Text.dll", component);
        ComponentSettings = (component as dynamic).Settings;
        ID = id;

        ComponentSettings.Tag = id;

        Data.s_LayoutComponents.Add(LayoutComponent);
    }

    internal TextComponent(string id, ILayoutComponent component)
    {
        LayoutComponent = (LayoutComponent)component;
        ComponentSettings = (component.Component as dynamic).Settings;
        ID = id;
    }

    internal TextComponent(ILayoutComponent component)
    {
        LayoutComponent = (LayoutComponent)component;
        ComponentSettings = (component.Component as dynamic).Settings;
    }

    internal readonly LayoutComponent LayoutComponent;
    internal readonly dynamic ComponentSettings;

    public string ID { get; internal set; }

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
