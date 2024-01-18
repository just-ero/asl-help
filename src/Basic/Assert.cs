using AslHelp.Data.AutoSplitter;
using System.Runtime.CompilerServices;

namespace AslHelp;

public static class Assert
{
    public static void NotNull(object argument, string paramName)
    {
        if (argument == null)
        {
            throw new ArgumentNullException($"{paramName} may not be null.");
        }
    }

    public static void InAction(string action, [CallerMemberName] string name = "")
    {

        if (Actions.Current != action)
        {
            string msg = $"{name} may only be set in the '{action} {{}}' action.";
            throw new InvalidOperationException(msg);
        }
    }
}