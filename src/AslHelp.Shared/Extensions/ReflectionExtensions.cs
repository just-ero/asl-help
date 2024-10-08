using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace AslHelp.Shared.Extensions;

/// <summary>
///     The <see cref="ReflectionExtensions"/> class
///     provides extension methods for reflection.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    ///     Gets the value of a field with the specified <paramref name="fieldName"/> from the specified object.
    /// </summary>
    /// <typeparam name="T">The target type of the field.</typeparam>
    /// <param name="obj">The instance to get the field value from.</param>
    /// <param name="fieldName">The name of the field to get the value of.</param>
    /// <returns>
    ///     The value of the field with the specified <paramref name="fieldName"/> if the field exists;
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    public static T? GetFieldValue<T>(this object obj, string fieldName)
    {
        return (T?)obj.GetType().GetRuntimeFields().FirstOrDefault(fi => fi.Name == fieldName)?.GetValue(obj);
    }

    /// <summary>
    ///     Sets the value of a field with the specified name on the specified object.
    /// </summary>
    /// <typeparam name="T">The target type of the field.</typeparam>
    /// <param name="obj">The object to set the field value on.</param>
    /// <param name="fieldName">The name of the field to set the value of.</param>
    /// <param name="value">The value to set the field to.</param>
    public static void SetFieldValue<T>(this object obj, string fieldName, T value)
    {
        obj.GetType().GetRuntimeFields().FirstOrDefault(fi => fi.Name == fieldName)?.SetValue(obj, value);
    }

    /// <summary>
    ///     Gets the value of a property with the specified <paramref name="propertyName"/> from the specified object.
    /// </summary>
    /// <typeparam name="T">The target type of the property.</typeparam>
    /// <param name="obj">The object to get the property value from.</param>
    /// <param name="propertyName">The name of the property to get the value of.</param>
    /// <returns>
    ///     The value of the property with the specified <paramref name="propertyName"/> if the property exists;
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    public static T? GetPropertyValue<T>(this object obj, string propertyName)
    {
        return (T?)obj.GetType().GetRuntimeProperties().FirstOrDefault(pi => pi.Name == propertyName)?.GetValue(obj);
    }

    /// <summary>
    ///     Sets the value of a property with the specified <paramref name="propertyName"/> on the specified object.
    /// </summary>
    /// <typeparam name="T">The target type of the property.</typeparam>
    /// <param name="obj">The object to set the property value on.</param>
    /// <param name="propertyName">The name of the property to set the value of.</param>
    /// <param name="value">The value to set the property to.</param>
    public static void SetPropertyValue<T>(this object obj, string propertyName, T value)
    {
        obj.GetType().GetRuntimeProperties().FirstOrDefault(pi => pi.Name == propertyName)?.SetValue(obj, value);
    }

    /// <summary>
    ///     Gets the method with the specified <paramref name="methodName"/> from the specified object.
    /// </summary>
    /// <param name="obj">The object to get the method from.</param>
    /// <param name="methodName">The name of the method to get.</param>
    /// <returns>
    ///     The method with the specified <paramref name="methodName"/> if the method exists;
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    public static MethodInfo? GetMethod(this object obj, string methodName)
    {
        return obj.GetType().GetRuntimeMethods().FirstOrDefault(m => m.Name == methodName);
    }

    /// <summary>
    ///     Determines whether the specified object is of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to compare the object to.</typeparam>
    /// <param name="obj">The object to compare.</param>
    /// <returns>
    ///     <see langword="true"/> if the object is of the specified type;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsType<T>(this object obj)
    {
        return obj.GetType() == typeof(T);
    }

    /// <summary>
    ///     Gets the assembly that contains the code that is currently executing.
    /// </summary>
    public static Assembly ExecutingAssembly { get; } = Assembly.GetExecutingAssembly();

    /// <summary>
    ///     Gets the assembly that contains the code that called this method.
    /// </summary>
    public static IEnumerable<Assembly> AssemblyTrace
    {
        get
        {
            return
                new StackTrace().GetFrames()
                .Select(frame => frame.GetMethod()?.DeclaringType?.Assembly)
                .OfType<Assembly>()
                .Distinct();
        }
    }
}
