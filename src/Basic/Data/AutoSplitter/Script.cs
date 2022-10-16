namespace AslHelp.Data.AutoSplitter;

internal static class Script
{
    public static object ASLScript { get; }

    static Script()
    {
        IEnumerable<dynamic> components = timer.Layout.Components.Prepend(timer.Run.AutoSplitter?.Component).Cast<dynamic>();
        dynamic aslComponent = components.FirstOrDefault(component =>
        {
            if (component is null || component.GetType().Name != "ASLComponent")
            {
                return false;
            }

            if (component.Script is not object script)
            {
                return false;
            }

            IEnumerable<object> methods = script.GetFieldValue<IEnumerable<object>>("_methods");
            if (methods?.FirstOrDefault() is not object method)
            {
                return false;
            }

            object cc = method.GetFieldValue<object>("_compiled_code");
            return cc.GetType().Assembly == GetCompiledScriptAssembly();
        });

        ASLScript = aslComponent.Script;
        vars = aslComponent.Script.Vars;
        SettingsBuilder = ASLScript.GetFieldValue<dynamic>("_settings").Builder;

        LoadMethods(aslComponent);
    }

    public static Process ScriptGame
    {
        get => ASLScript.GetFieldValue<Process>("_game");
        set => ASLScript.SetFieldValue<Process>("_game", value);
    }

#pragma warning disable IDE1006
    public static dynamic vars { get; }
    public static IDictionary<string, dynamic> current => (ASLScript as dynamic).State.Data;
#pragma warning restore IDE1006

    public static dynamic SettingsBuilder { get; }

    private static Assembly GetCompiledScriptAssembly()
    {
        StackFrame[] frames = new StackTrace().GetFrames();

        foreach (StackFrame frame in frames)
        {
            Type decl = frame.GetMethod().DeclaringType;

            if (decl?.Name == "CompiledScript")
            {
                return decl.Assembly;
            }
        }

        return null;
    }

    private static void LoadMethods(object aslComponent)
    {
        dynamic settings = aslComponent.GetFieldValue<dynamic>("_settings");
        dynamic path = settings.ScriptPath;
        dynamic code = File.ReadAllText(path);

        object aslGrammar = ASLScript.GetType().Assembly.CreateInstance("LiveSplit.ASL.ASLGrammar");
        Type grammarType = aslGrammar.GetType().BaseType;
        Type parserType = grammarType.Assembly.GetType("Irony.Parsing.Parser");

        dynamic parser = Activator.CreateInstance(parserType, aslGrammar);
        dynamic tree = parser.Parse(code);

        dynamic methodsNode = (tree.Root.ChildNodes as IEnumerable<dynamic>).First(n => n.Term.Name == "methodList");

        foreach (dynamic method in methodsNode.ChildNodes[0].ChildNodes)
        {
            string name = method.ChildNodes[0].Token.Value as string;
            string body = method.ChildNodes[2].Token.Value as string;
            dynamic line = method.ChildNodes[2].Token.Location.Line + 1;

            PropertyInfo action = typeof(Actions).GetProperty(name);
            action.SetValue(action, new Actions.Action(body, name, line));
        }
    }
}
