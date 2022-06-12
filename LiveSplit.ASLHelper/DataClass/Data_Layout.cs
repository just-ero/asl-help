using LiveSplit.UI.Components;

namespace ASLHelper;

internal static partial class Data
{
    public static IEnumerable<IComponent> s_Components
    {
        get => Main.Instance.Layout.Components;
    }

    public static IList<ILayoutComponent> s_LayoutComponents
    {
        get => Main.Instance.Layout.LayoutComponents;
    }

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
        var lc = s_LayoutComponents.SingleOrDefault(lc => (lc.Component.GetSettingsControl(Main.Instance.Layout.Mode).Tag as string) == tag);

        if (lc == null)
            return null;

        if (typeName == null)
            return lc;

        if (lc?.Component.GetType().Name != typeName)
            return null;

        component = lc.Component;
        return lc;
    }

    public static IEnumerable<ILayoutComponent> LayoutComponentsOfType(string type)
    {
        foreach (var lc in s_LayoutComponents)
        {
            if (lc.Component.GetType().Name == type)
                yield return lc;
        }
    }
}
