using System.Reflection;

namespace ASLHelper.Extensions
{
    internal static class ReflectionExtensions
    {
        private const BindingFlags PRIVATE_INSTANCE = BindingFlags.Instance | BindingFlags.NonPublic;

        public static dynamic GetFieldValue(this object type, string fieldName, BindingFlags flags = PRIVATE_INSTANCE)
        {
            return type.GetType().GetField(fieldName, flags).GetValue(type);
        }

        public static void SetFieldValue(this object type, string fieldName, object value, BindingFlags flags = PRIVATE_INSTANCE)
        {
            type.GetType().GetField(fieldName, flags).SetValue(type, value);
        }
    }
}