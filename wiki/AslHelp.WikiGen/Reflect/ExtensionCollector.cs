using AslHelp.WikiGen.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace AslHelp.WikiGen.Reflect;

/// <summary>
///     Collects the C# 14 <c>extension</c> members that docfx omits, by reflecting the compiled
///     assembly (where they are ordinary static methods) and pairing them with their XML summaries.
/// </summary>
internal static class ExtensionCollector
{
    /// <summary>
    ///     Returns extension members grouped by their declaring type's full name (= docfx UID).
    /// </summary>
    public static Dictionary<string, List<ApiMember>> Collect(IEnumerable<string> assemblyPaths, XmlSummaries summaries)
    {
        var result = new Dictionary<string, List<ApiMember>>(StringComparer.Ordinal);

        foreach (var assemblyPath in assemblyPaths)
        {
            CollectFrom(Assembly.LoadFrom(assemblyPath), summaries, result);
        }

        return result;
    }

    private static void CollectFrom(Assembly assembly, XmlSummaries summaries, Dictionary<string, List<ApiMember>> result)
    {
        foreach (var type in SafeTypes(assembly))
        {
            if (!type.IsPublic || !type.IsAbstract || !type.IsSealed) // static class
            {
                continue;
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (!method.IsDefined(typeof(ExtensionAttribute), inherit: false))
                {
                    continue;
                }

                (result.TryGetValue(type.FullName!, out var list) ? list : result[type.FullName!] = [])
                    .Add(ToMember(method, summaries.For(type.FullName!, method.Name)));
            }
        }
    }

    private static ApiMember ToMember(MethodInfo method, string? summary)
    {
        var name = method.Name + GenericSuffix(method.GetGenericArguments());
        return new ApiMember(
            Name: name,
            Group: MemberGroup.Methods,
            Signature: Signature(method),
            Summary: summary,
            ValueType: null,
            Parameters: [],
            ReturnType: null,
            ReturnSummary: null,
            Source: null);
    }

    private static string Signature(MethodInfo method)
    {
        var sb = new StringBuilder("public static ");
        sb.Append(TypeName(method.ReturnType)).Append(' ').Append(method.Name);
        sb.Append(GenericSuffix(method.GetGenericArguments())).Append('(');

        var ps = method.GetParameters();
        for (var i = 0; i < ps.Length; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }

            if (i == 0)
            {
                sb.Append("this ");
            }

            if (ps[i].ParameterType.IsByRef)
            {
                sb.Append(ps[i].IsIn ? "in " : ps[i].IsOut ? "out " : "ref ");
            }

            sb.Append(TypeName(ps[i].ParameterType)).Append(' ').Append(ps[i].Name);
        }

        return sb.Append(')').ToString();
    }

    private static string GenericSuffix(Type[] args)
    {
        return args.Length == 0 ? "" : $"<{string.Join(", ", args.Select(TypeName))}>";
    }

    private static string TypeName(Type type)
    {
        if (type.IsByRef)
        {
            return TypeName(type.GetElementType()!);
        }

        if (type.IsArray)
        {
            return TypeName(type.GetElementType()!) + "[]";
        }

        if (type.IsGenericType)
        {
            var raw = type.Name;
            var tick = raw.IndexOf('`', StringComparison.Ordinal);
            var baseName = tick >= 0 ? raw[..tick] : raw;
            return $"{baseName}<{string.Join(", ", type.GetGenericArguments().Select(TypeName))}>";
        }

        return Keyword(type.Name);
    }

    private static string Keyword(string name)
    {
        return name switch
        {
            "Boolean" => "bool",
            "Int32" => "int",
            "Int64" => "long",
            "String" => "string",
            "Object" => "object",
            "Void" => "void",
            _ => name,
        };
    }

    private static IEnumerable<Type> SafeTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
    }
}
