namespace AslHelp.Extensions;

internal static class ReflectionExt
{
    public const BindingFlags PRIVATE_INSTANCE = BindingFlags.Instance | BindingFlags.NonPublic;
    public const BindingFlags PUBLIC_INSTANCE = BindingFlags.Instance | BindingFlags.Public;

    public static T GetFieldValue<T>(this object obj, string fieldName, BindingFlags flags = PRIVATE_INSTANCE)
    {
        return (T)obj.GetAllFields(flags).FirstOrDefault(f => f.Name == fieldName)?.GetValue(obj);
    }

    public static void SetFieldValue<T>(this object obj, string fieldName, T value, BindingFlags flags = PRIVATE_INSTANCE)
    {
        obj.GetAllFields(flags).FirstOrDefault(f => f.Name == fieldName)?.SetValue(obj, value);
    }

    public static T GetPropertyValue<T>(this object obj, string propertyName, BindingFlags flags = PUBLIC_INSTANCE)
    {
        return (T)obj.GetAllProperties(flags).FirstOrDefault(p => p.Name == propertyName)?.GetValue(obj);
    }

    public static void SetPropertyValue<T>(this object obj, string propertyName, T value, BindingFlags flags = PUBLIC_INSTANCE)
    {
        obj.GetAllProperties(flags).FirstOrDefault(p => p.Name == propertyName)?.SetValue(obj, value);
    }

    public static MethodInfo GetMethod(this object obj, string methodName, BindingFlags flags = PRIVATE_INSTANCE)
    {
        return obj.GetAllMethods(flags).FirstOrDefault(m => m.Name == methodName);
    }

    public static IEnumerable<FieldInfo> GetAllFields(this object obj, BindingFlags flags)
    {
        return obj.GetType().GetAllFields(flags);
    }

    public static IEnumerable<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
    {
        foreach (FieldInfo fi in type.GetFields(flags))
        {
            yield return fi;
        }

        foreach (FieldInfo fi in type.GetRuntimeFields())
        {
            yield return fi;
        }
    }

    public static IEnumerable<MethodInfo> GetAllMethods(this object obj, BindingFlags flags)
    {
        return obj.GetType().GetAllMethods(flags);
    }

    public static IEnumerable<MethodInfo> GetAllMethods(this Type type, BindingFlags flags)
    {
        foreach (MethodInfo mi in type.GetMethods(flags))
        {
            yield return mi;
        }

        foreach (MethodInfo mi in type.GetRuntimeMethods())
        {
            yield return mi;
        }
    }

    public static IEnumerable<PropertyInfo> GetAllProperties(this object obj, BindingFlags flags)
    {
        return obj.GetType().GetAllProperties(flags);
    }

    public static IEnumerable<PropertyInfo> GetAllProperties(this Type type, BindingFlags flags)
    {
        foreach (PropertyInfo pi in type.GetProperties(flags))
        {
            yield return pi;
        }

        foreach (PropertyInfo pi in type.GetRuntimeProperties())
        {
            yield return pi;
        }
    }
}
