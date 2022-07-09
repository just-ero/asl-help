namespace ASLHelper.Extensions;

internal static class ReflectionExtensions
{
    private const BindingFlags PRIVATE_INSTANCE = BindingFlags.Instance | BindingFlags.NonPublic;
    private const BindingFlags PUBLIC_INSTANCE = BindingFlags.Instance | BindingFlags.Public;

    public static dynamic GetFieldValue(this object type, string fieldName, BindingFlags flags = PRIVATE_INSTANCE)
    {
        return type.GetType().GetField(fieldName, flags).GetValue(type);
    }

    public static void SetFieldValue(this object type, string fieldName, object value, BindingFlags flags = PRIVATE_INSTANCE)
    {
        type.GetType().GetField(fieldName, flags).SetValue(type, value);
    }

    public static bool HasProperty(this object type, string propertyName, BindingFlags flags = PUBLIC_INSTANCE)
    {
        return type.GetType().GetProperty(propertyName, flags) is not null;
    }

    public static T As<T>(this object obj) where T : class
    {
        return obj as T;
    }
}
