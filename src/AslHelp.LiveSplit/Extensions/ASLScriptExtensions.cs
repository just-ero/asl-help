extern alias Ls;

using Ls::LiveSplit.ASL;

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace AslHelp.LiveSplit.Extensions;

internal static class ASLScriptExtensions
{
    private static readonly Func<ASLScript, Process?> _getScriptGame;
    private static readonly Action<ASLScript, Process?> _setScriptGame;

    static ASLScriptExtensions()
    {
        _getScriptGame = CreateGetScriptGameFunc();
        _setScriptGame = CreateSetScriptGameFunc();
    }

    public static Process? GetGame(this ASLScript script)
    {
        return _getScriptGame(script);
    }

    public static void SetGame(this ASLScript script, Process? game)
    {
        _setScriptGame(script, game);
    }

    private static Func<ASLScript, Process?> CreateGetScriptGameFunc()
    {
        DynamicMethod dm = new(nameof(_getScriptGame), typeof(Process), [typeof(ASLScript)], true);

        FieldInfo fiGame = typeof(ASLScript).GetField("_game", BindingFlags.Instance | BindingFlags.NonPublic);

        ILGenerator il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, fiGame);
        il.Emit(OpCodes.Ret);

        return (Func<ASLScript, Process?>)dm.CreateDelegate(typeof(Func<ASLScript, Process?>));
    }

    private static Action<ASLScript, Process?> CreateSetScriptGameFunc()
    {
        DynamicMethod dm = new(nameof(_setScriptGame), null, [typeof(ASLScript), typeof(Process)], true);

        FieldInfo fiGame = typeof(ASLScript).GetField("_game", BindingFlags.Instance | BindingFlags.NonPublic);

        ILGenerator il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Stfld, fiGame);
        il.Emit(OpCodes.Ret);

        return (Action<ASLScript, Process?>)dm.CreateDelegate(typeof(Action<ASLScript, Process?>));
    }
}
