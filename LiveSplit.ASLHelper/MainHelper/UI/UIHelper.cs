namespace ASLHelper.MainHelper;

public class UIHelper
{
    internal UIHelper()
    {
        Text = new();
    }

    public TextComponentHelper Text { get; }

    public TextComponent FindText(string text1 = "", string text2 = "")
    {
        foreach (var tc in Data.LayoutComponentsOfType("TextComponent"))
        {
            dynamic tcd = tc.Component;
            bool noe1 = string.IsNullOrEmpty(text1), noe2 = string.IsNullOrEmpty(text2);

            if (noe1 && noe2)
                continue;

            if (!noe1 && tcd.Settings.Text1 != text1)
                continue;

            if (!noe2 && tcd.Settings.Text2 != text2)
                continue;

            return new(tc);
        }

        return null;
    }
}
