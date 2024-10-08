using System;
using System.IO;
using System.Linq;

using AslHelp.Shared.Extensions;

using Irony.Parsing;

using LiveSplit.ASL;
using LiveSplit.UI.Components;

namespace AslHelp.LiveSplit;

public partial class Autosplitter
{
    private static ScriptActions ParseActions(ASLComponent component, ASLScript.Methods methods)
    {
        ComponentSettings settings = component.GetFieldValue<ComponentSettings>("_settings")!;

        // Shouldn't fail. LiveSplit was already able to read the executing script.
        string code = File.ReadAllText(settings.ScriptPath);

        // Shouldn't need to be careful about any of the below.
        // If the script exists at all, this has already passed.

        ScriptActions actions = new(methods);

        ASLGrammar grammar = new();
        Parser parser = new(grammar);

        ParseTree tree = parser.Parse(code);
        ParseTreeNode node = tree.Root.ChildNodes.First(n => n.Term.Name == "methodList");

        foreach (ParseTreeNode method in node.ChildNodes[0].ChildNodes)
        {
            string body = (string)method.ChildNodes[2].Token.Value;
            string name = (string)method.ChildNodes[0].Token.Value;
            int line = method.ChildNodes[2].Token.Location.Line + 1;

            ScriptAction action = new(methods, body, name, line);

            switch (name)
            {
                case "startup":
                {
                    actions.Startup = action;
                    continue;
                }
                case "shutdown":
                {
                    actions.Shutdown = action;
                    continue;
                }

                case "init":
                {
                    actions.Init = action;
                    continue;
                }
                case "update":
                {
                    actions.Update = action;
                    continue;
                }

                case "start":
                {
                    actions.Start = action;
                    continue;
                }
                case "split":
                {
                    actions.Split = action;
                    continue;
                }
                case "reset":
                {
                    actions.Reset = action;
                    continue;
                }

                case "gameTime":
                {
                    actions.GameTime = action;
                    continue;
                }
                case "isLoading":
                {
                    actions.IsLoading = action;
                    continue;
                }

                case "onStart":
                {
                    actions.OnStart = action;
                    continue;
                }
                case "onSplit":
                {
                    actions.OnSplit = action;
                    continue;
                }
                case "onReset":
                {
                    actions.OnReset = action;
                    continue;
                }
            }
        }

        return actions;
    }

    [Obsolete("Do not use ASL-specific features.", true)]
    public class ScriptActions
    {
        public ScriptActions(ASLScript.Methods methods)
        {
            Startup = new(methods, "startup");
            Shutdown = new(methods, "shutdown");

            Init = new(methods, "init");
            Exit = new(methods, "exit");

            Update = new(methods, "update");

            Start = new(methods, "start");
            Split = new(methods, "split");
            Reset = new(methods, "reset");

            GameTime = new(methods, "gameTime");
            IsLoading = new(methods, "isLoading");

            OnStart = new(methods, "onStart");
            OnSplit = new(methods, "onSplit");
            OnReset = new(methods, "onReset");
        }

        public ScriptAction Startup { get; set; }
        public ScriptAction Shutdown { get; set; }

        public ScriptAction Init { get; set; }
        public ScriptAction Exit { get; set; }

        public ScriptAction Update { get; set; }

        public ScriptAction Start { get; set; }
        public ScriptAction Split { get; set; }
        public ScriptAction Reset { get; set; }

        public ScriptAction GameTime { get; set; }
        public ScriptAction IsLoading { get; set; }

        public ScriptAction OnStart { get; set; }
        public ScriptAction OnSplit { get; set; }
        public ScriptAction OnReset { get; set; }
    }
}
