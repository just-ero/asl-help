using AslHelp.Reflection;
using LiveSplit.ASL;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace AslHelp.LiveSplit;

/// <summary>
///     Represents a single ASL script action (e.g. <c>startup</c>, <c>update</c>, <c>split</c>) and
///     supports injecting code into it via <see cref="Append"/> and <see cref="Prepend"/>.
/// </summary>
[DebuggerDisplay("{Name,nq} (line {Line})")]
public sealed class ScriptAction
{
    private readonly ASLScript.Methods _methods;

    internal ScriptAction(
        ASLScript.Methods methods,
        string name,
        string body,
        int line,
        Module? module)
    {
        _methods = methods;

        Name = name;
        Body = body;
        Line = line;
        Module = module;
    }

    internal ScriptAction(ASLScript.Methods methods, string name)
    {
        _methods = methods;

        Name = name;
        Body = "";
    }

    /// <summary>
    ///     Gets the name of the action.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the current source code of the action's body.
    /// </summary>
    public string Body { get; private set; }

    /// <summary>
    ///     Gets the 1-based line at which the action's body begins in the script file.
    /// </summary>
    public int Line { get; }

    /// <summary>
    ///     Gets the module the action was compiled into, or <see langword="null"/> when unavailable.
    /// </summary>
    public Module? Module { get; }

    /// <summary>
    ///     Appends <paramref name="code"/> to the action's <see cref="Body"/>. Call
    ///     <see cref="Recompile"/> to apply the change to the running script.
    /// </summary>
    /// <param name="code">The code to append.</param>
    public ScriptAction Append(string code)
    {
        Body += code;
        return this;
    }

    /// <summary>
    ///     Prepends <paramref name="code"/> to the action's <see cref="Body"/>. Call
    ///     <see cref="Recompile"/> to apply the change to the running script.
    /// </summary>
    /// <param name="code">The code to prepend.</param>
    public ScriptAction Prepend(string code)
    {
        Body = code + Body;
        return this;
    }

    /// <summary>
    ///     Compiles the current <see cref="Body"/> and swaps it in as the script's method for this
    ///     action, so subsequent runs execute the modified code.
    /// </summary>
    public void Recompile()
    {
        _methods.SetFieldValue<ASLMethod>(Name, new(Body, Name, Line));
    }

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
        return $"{Name} (line {Line})";
    }
}
