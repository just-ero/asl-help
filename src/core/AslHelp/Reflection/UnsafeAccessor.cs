using System;
using System.Reflection;
using System.Reflection.Emit;

namespace AslHelp.Reflection;

/// <summary>
///     Builds compiled field accessors via IL, trading the one-time emit cost for fast repeated
///     access to a field (including non-public ones).
/// </summary>
public static class UnsafeAccessor
{
    private const BindingFlags InstanceFlags = ReflectionExtensions.InstanceFlags;

    /// <summary>
    ///     Reads a field of type <typeparamref name="TField"/> from a <typeparamref name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TTarget">The type declaring the field.</typeparam>
    /// <typeparam name="TField">The field's type.</typeparam>
    /// <param name="instance">The instance to read from.</param>
    public delegate TField? GetField<TTarget, TField>(TTarget instance);

    /// <summary>
    ///     Writes a field of type <typeparamref name="TField"/> on a <typeparamref name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TTarget">The type declaring the field.</typeparam>
    /// <typeparam name="TField">The field's type.</typeparam>
    /// <param name="instance">The instance to write to.</param>
    /// <param name="value">The value to assign.</param>
    public delegate void SetField<TTarget, TField>(TTarget instance, TField? value);

    /// <summary>
    ///     Compiles a getter for the field named <paramref name="name"/> on <typeparamref name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TTarget">The type declaring the field.</typeparam>
    /// <typeparam name="TField">The field's type.</typeparam>
    /// <param name="name">The name of the field to read.</param>
    /// <param name="flags">The binding flags used to locate the field.</param>
    /// <returns>
    ///     A delegate that reads the field from a given instance.
    /// </returns>
    /// <exception cref="MissingFieldException">No field of type <typeparamref name="TField"/> named <paramref name="name"/> exists.</exception>
    public static GetField<TTarget, TField> CreateFieldGetter<TTarget, TField>(
        string name,
        BindingFlags flags = InstanceFlags)
    {
        var fi = typeof(TTarget).GetField(name, flags);
        if (fi?.FieldType != typeof(TField))
        {
            MissingFieldException.Throw(typeof(TTarget).Name, name);
        }

        DynamicMethod dm = new($"get{name}", typeof(TField), [typeof(TTarget)], true);

        var il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, fi);
        il.Emit(OpCodes.Ret);

        return (GetField<TTarget, TField>)dm.CreateDelegate(typeof(GetField<TTarget, TField>));
    }

    /// <summary>
    ///     Compiles a setter for the field named <paramref name="name"/> on <typeparamref name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TTarget">The type declaring the field.</typeparam>
    /// <typeparam name="TField">The field's type.</typeparam>
    /// <param name="name">The name of the field to write.</param>
    /// <param name="flags">The binding flags used to locate the field.</param>
    /// <returns>
    ///     A delegate that writes the field on a given instance.
    /// </returns>
    /// <exception cref="MissingFieldException">No field of type <typeparamref name="TField"/> named <paramref name="name"/> exists.</exception>
    public static SetField<TTarget, TField> CreateFieldSetter<TTarget, TField>(
        string name,
        BindingFlags flags = InstanceFlags)
    {
        var fi = typeof(TTarget).GetField(name, flags);
        if (fi?.FieldType != typeof(TField))
        {
            MissingFieldException.Throw(typeof(TTarget).Name, name);
        }

        DynamicMethod dm = new($"set{name}", null, [typeof(TTarget), typeof(TField)], true);

        var il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Stfld, fi);
        il.Emit(OpCodes.Ret);

        return (SetField<TTarget, TField>)dm.CreateDelegate(typeof(SetField<TTarget, TField>));
    }
}
