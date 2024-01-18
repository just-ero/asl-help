using AslHelp.Data.AutoSplitter;

namespace AslHelp;

public static class Validation
{
    public static void AssertNotNull(string name, dynamic value)
    {
        if (value == null)
        {
            throw new ArgumentNullException($"{name} may not be null.");
        }
    }

    public static void AssertAction(string name, string action)
    {

        if (Actions.Current != "startup")
        {
            string msg = $"{name} may only be set in the '{action} {{}}' action.";
            throw new InvalidOperationException(msg);
        }
    }
}