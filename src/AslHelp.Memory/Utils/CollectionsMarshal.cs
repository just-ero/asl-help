using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace AslHelp.Memory.Utils;

public static class CollectionsMarshal<T>
{
    private static readonly Func<List<T>, (T[] Data, int Size)> _getBackingArray;

    static CollectionsMarshal()
    {
        _getBackingArray = CreateGetBackingArrayFunc();
    }

    public static Span<T> AsSpan(List<T> list)
    {
        (T[] data, int size) = _getBackingArray(list);
        return new(data, 0, size);
    }

    private static Func<List<T>, (T[] Data, int Size)> CreateGetBackingArrayFunc()
    {
        DynamicMethod dm = new(nameof(_getBackingArray), typeof((T[], int)), [typeof(List<T>)], true);

        FieldInfo fiItems = typeof(List<T>).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo fiSize = typeof(List<T>).GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic);

        ILGenerator il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, fiItems);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, fiSize);
        il.Emit(OpCodes.Ret);

        return (Func<List<T>, (T[], int)>)dm.CreateDelegate(typeof(Func<List<T>, (T[], int)>));
    }
}
