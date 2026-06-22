using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace AslHelp.Reflection;

/// <summary>
///     Provides extension members for <see cref="object"/>.
/// </summary>
public static class ReflectionExtensions
{
    internal const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    extension(object? obj)
    {
        /// <summary>
        ///     Reads the field named <paramref name="name"/> from this object.
        /// </summary>
        /// <typeparam name="T">The field's type.</typeparam>
        /// <param name="name">The name of the field to read.</param>
        /// <param name="flags">The binding flags used to locate the field.</param>
        /// <returns>
        ///     The field's value.
        /// </returns>
        /// <exception cref="MissingFieldException">No field of type <typeparamref name="T"/> named <paramref name="name"/> exists.</exception>
        [Pure]
        public T? GetFieldValue<T>(string name, BindingFlags flags = InstanceFlags)
        {
            ArgumentNullException.ThrowIfNull(name);

            var fi = obj?.GetType().GetField(name, flags);
            if (fi?.FieldType != typeof(T))
            {
                MissingFieldException.Throw(obj?.GetType().FullName ?? "<null>", name);
            }

            return (T)fi.GetValue(obj);
        }

        /// <summary>
        ///     Writes <paramref name="value"/> to the field named <paramref name="name"/> on this object.
        /// </summary>
        /// <typeparam name="T">The field's type.</typeparam>
        /// <param name="name">The name of the field to write.</param>
        /// <param name="value">The value to assign.</param>
        /// <param name="flags">The binding flags used to locate the field.</param>
        /// <exception cref="MissingFieldException">No field of type <typeparamref name="T"/> named <paramref name="name"/> exists.</exception>
        public void SetFieldValue<T>(string name, T value, BindingFlags flags = InstanceFlags)
        {
            ArgumentNullException.ThrowIfNull(name);

            var fi = obj?.GetType().GetField(name, flags);
            if (fi?.FieldType != typeof(T))
            {
                MissingFieldException.Throw(obj?.GetType().FullName ?? "<null>", name);
            }

            fi.SetValue(obj, value);
        }
    }
}
