using LiveSplit.UI.Components;

namespace ASLHelper;

/// <summary>
///     A static data-class, holding relevant information about the helper.
/// </summary>
internal static partial class Data
{
    public static IEnumerable<IComponent> s_Components;
    public static IList<ILayoutComponent> s_LayoutComponents;

    public static IComponent FindComponent(string typeName, string tag)
    {
        _ = FindLayoutComponent(typeName, tag, out var component);
        return component;
    }

    public static ILayoutComponent FindLayoutComponent(string tag)
    {
        return FindLayoutComponent(null, tag, out _);
    }

    public static ILayoutComponent FindLayoutComponent(string typeName, string tag, out IComponent component)
    {
        component = null;
        var layoutComponent = s_LayoutComponents.SingleOrDefault(lc => (lc.Component.GetSettingsControl(s_Layout.Mode).Tag as string) == tag);

        if (layoutComponent == null)
            return null;

        if (typeName == null)
            return layoutComponent;

        if (layoutComponent?.Component.GetType().Name != typeName)
            return null;

        component = layoutComponent.Component;
        return layoutComponent;
    }
}