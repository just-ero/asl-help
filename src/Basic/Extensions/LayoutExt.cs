using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace AslHelp.Extensions;

internal static class LayoutExt
{
    public static ILayoutComponent FindLayoutComponent(this ILayout layout, string tag)
    {
        return layout.LayoutComponents
               .SingleOrDefault(lc => (lc.Component.GetSettingsControl(layout.Mode).Tag as string) == tag);
    }

    public static ILayoutComponent TryFindLayoutComponent(this ILayout layout, string typeName, string tag, out IComponent component)
    {
        ILayoutComponent lc = layout.LayoutComponents
                              .SingleOrDefault(lc => (lc.Component.GetSettingsControl(layout.Mode).Tag as string) == tag);

        if (lc?.Component is IComponent comp && comp.GetType().Name == typeName)
        {
            component = comp;
            return lc;
        }
        else
        {
            component = null;
            return null;
        }
    }

    public static IEnumerable<ILayoutComponent> LayoutComponentsOfType(this ILayout layout, string typeName)
    {
        foreach (ILayoutComponent lc in layout.LayoutComponents)
        {
            if (lc.Component.GetType().Name == typeName)
            {
                yield return lc;
            }
        }
    }
}
