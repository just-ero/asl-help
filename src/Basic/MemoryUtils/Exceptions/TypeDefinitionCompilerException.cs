using System;
using System.CodeDom.Compiler;
using System.Text;

namespace AslHelp.MemUtils.Exceptions;

internal class TypeDefinitionCompilerException : Exception
{
    public TypeDefinitionCompilerException(string message)
        : base("asl-help compilation errors: " + message) { }

    public TypeDefinitionCompilerException(CompilerErrorCollection errors)
        : base("asl-help compilation errors:" + GetMessage(errors)) { }

    private static string GetMessage(CompilerErrorCollection errors)
    {
        if (errors == null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        StringBuilder sb = new();

        foreach (CompilerError error in errors)
        {
            sb
            .Append(Environment.NewLine)
            .Append("Line").Append(error.Line).Append(", ")
            .Append("Col").Append(error.Column).Append(": ")
            .Append(error.IsWarning ? "warning" : "error").Append(" ")
            .Append(error.ErrorNumber).Append(": ")
            .Append(error.ErrorText);
        }

        return sb.ToString();
    }
}
