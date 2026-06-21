using AslHelp.Logging;
using AslHelp.Reflection;
using Irony.Parsing;
using LiveSplit.ASL;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AslHelp.LiveSplit.Asl.Attach;

/// <summary>
///     Provides access to the actions of the attached ASL script.
/// </summary>
public sealed class ScriptActions
{
    private static readonly string[] _names =
    [
        "startup", "shutdown",
        "init", "exit",
        "update",
        "start", "split", "reset",
        "gameTime", "isLoading",
        "onStart", "onSplit", "onReset",
    ];

    private readonly Dictionary<string, ScriptAction> _byName;

    private ScriptActions(Dictionary<string, ScriptAction> byName)
    {
        _byName = byName;
    }

    /// <summary>Gets the <c>startup</c> action.</summary>
    public ScriptAction Startup => _byName["startup"];

    /// <summary>Gets the <c>shutdown</c> action.</summary>
    public ScriptAction Shutdown => _byName["shutdown"];

    /// <summary>Gets the <c>init</c> action.</summary>
    public ScriptAction Init => _byName["init"];

    /// <summary>Gets the <c>exit</c> action.</summary>
    public ScriptAction Exit => _byName["exit"];

    /// <summary>Gets the <c>update</c> action.</summary>
    public ScriptAction Update => _byName["update"];

    /// <summary>Gets the <c>start</c> action.</summary>
    public ScriptAction Start => _byName["start"];

    /// <summary>Gets the <c>split</c> action.</summary>
    public ScriptAction Split => _byName["split"];

    /// <summary>Gets the <c>reset</c> action.</summary>
    public ScriptAction Reset => _byName["reset"];

    /// <summary>Gets the <c>gameTime</c> action.</summary>
    public ScriptAction GameTime => _byName["gameTime"];

    /// <summary>Gets the <c>isLoading</c> action.</summary>
    public ScriptAction IsLoading => _byName["isLoading"];

    /// <summary>Gets the <c>onStart</c> action.</summary>
    public ScriptAction OnStart => _byName["onStart"];

    /// <summary>Gets the <c>onSplit</c> action.</summary>
    public ScriptAction OnSplit => _byName["onSplit"];

    /// <summary>Gets the <c>onReset</c> action.</summary>
    public ScriptAction OnReset => _byName["onReset"];

    /// <summary>
    ///     Gets all actions in declaration order.
    /// </summary>
    public IEnumerable<ScriptAction> All => _names.Select(name => _byName[name]);

    /// <summary>
    ///     Builds the module-to-action-name map used to identify the calling script during attach.
    /// </summary>
    /// <param name="methods">The script's compiled methods.</param>
    /// <returns>
    ///     A map from each defined action's compiled module to its name.
    /// </returns>
    internal static IReadOnlyDictionary<Module, string> GetActionModules(ASLScript.Methods methods)
    {
        Dictionary<Module, string> map = [];
        foreach (ASLMethod method in methods)
        {
            // Each defined action compiles into its own assembly; empty (no-op) actions share a
            // single placeholder module and carry no name, so they are skipped.
            if (method is { IsEmpty: false, Module: { } module, Name: { } name })
            {
                map[module] = name;
            }
        }

        return map;
    }

    /// <summary>
    ///     Builds the action set, recovering each action's body from the script file when it parses.
    /// </summary>
    /// <param name="logger">The logger to write diagnostics to.</param>
    /// <param name="scriptPath">The path of the running script file.</param>
    /// <param name="methods">The script's compiled methods.</param>
    /// <returns>
    ///     The actions; bodies are unavailable (and <c>Append</c>/<c>Prepend</c> throw) when the
    ///     script could not be parsed.
    /// </returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types",
        Justification = "Any parse failure (IO, grammar, cast) falls back to bodiless actions.")]
    internal static ScriptActions Parse(Logger logger, string scriptPath, ASLScript.Methods methods)
    {
        try
        {
            string code = File.ReadAllText(scriptPath);

            ASLGrammar grammar = new();
            Parser parser = new(grammar);

            ParseTree tree = parser.Parse(code);
            ParseTreeNode node = tree.Root.ChildNodes.First(n => n.Term.Name == "methodList");

            Dictionary<string, ScriptAction> byName = CreateEmpty(methods);
            foreach (ParseTreeNode method in node.ChildNodes[0].ChildNodes)
            {
                string name = (string)method.ChildNodes[0].Token.Value;
                if (!byName.ContainsKey(name))
                {
                    logger.LogTrace($"Skipping unknown script method '{name}'.");
                    continue;
                }

                string body = (string)method.ChildNodes[2].Token.Value;
                int line = method.ChildNodes[2].Token.Location.Line + 1;

                ASLMethod aslMethod = methods.GetFieldValue<ASLMethod>(name)!;
                byName[name] = new(methods, name, body, line, aslMethod.Module);
                logger.LogTrace($"Parsed action '{name}' (line {line}).");
            }

            return new(byName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                $"Could not parse the script for action bodies ({ex.GetType().Name}); " +
                $"Append/Prepend will be unavailable.");

            return new(CreateEmpty(methods));
        }
    }

    private static Dictionary<string, ScriptAction> CreateEmpty(ASLScript.Methods methods)
    {
        Dictionary<string, ScriptAction> byName = [];
        foreach (string name in _names)
        {
            byName[name] = new(methods, name);
        }

        return byName;
    }
}
