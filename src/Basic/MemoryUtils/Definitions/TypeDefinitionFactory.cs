using AslHelp.MemUtils.Exceptions;

using Microsoft.CSharp;

using System.CodeDom.Compiler;

namespace AslHelp.MemUtils.Definitions;

internal static class TypeDefinitionFactory
{
    private static readonly CSharpCodeProvider _codeProvider = new();

    public static TypeDefinition CreateFromSource(string source, params string[] references)
    {
        if (_codeProvider is null)
        {
            string msg = "[Define] Code provider was null.";
            throw new NullReferenceException(msg);
        }

        CompilerParameters parameters = new()
        {
            GenerateInMemory = true,
            CompilerOptions = "/optimize"
        };

        parameters.ReferencedAssemblies.AddRange(references);

        CompilerResults asm = _codeProvider.CompileAssemblyFromSource(parameters, source);

        if (asm.Errors.HasErrors)
        {
            throw new TypeDefinitionCompilerException(asm.Errors);
        }

        Type[] types = asm.CompiledAssembly.GetTypes();

        if (types.Length == 0)
        {
            string msg = "The provided source code did not contain a type.";
            throw new TypeDefinitionCompilerException(msg);
        }

        return new(types[0]);
    }
}
