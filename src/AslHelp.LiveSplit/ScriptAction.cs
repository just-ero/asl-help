extern alias Ls;

using System;

using AslHelp.LiveSplit.Diagnostics;
using AslHelp.Shared.Extensions;

using Ls::LiveSplit.ASL;

namespace AslHelp.LiveSplit;

[Obsolete("Do not use ASL-specific features.", true)]
public sealed class ScriptAction
{
    private readonly ASLScript.Methods _methods;

    public ScriptAction(ASLScript.Methods methods, string body, string name, int line)
    {
        _methods = methods;

        Body = body;
        Name = name;
        Line = line;
    }

    public ScriptAction(ASLScript.Methods methods, string name)
    {
        _methods = methods;

        Body = "";
        Name = name;
    }

    public string Body { get; private set; }
    public string Name { get; }
    public int Line { get; }

    public void Append(string code)
    {
        Body += code;
        _methods.SetFieldValue<ASLMethod>(Name, new(Body, Name, Line));

        using (AslDebug.Indent($"Added the following code to the end of '{Name}':"))
        {
            AslDebug.Info($"`{code}`");
        }
    }

    public void Prepend(string code)
    {
        Body = code + Body;
        _methods.SetFieldValue<ASLMethod>(Name, new(Body, Name, Line));

        using (AslDebug.Indent($"Added the following code to the beginning of '{Name}':"))
        {
            AslDebug.Info($"`{code}`");
        }
    }
}
