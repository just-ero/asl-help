using AslHelp.Mono.Models;

namespace AslHelp.Mono.Managers;

public abstract partial class UnityMemManager
{
    internal nint TypeData(nint type)
    {
        return ReadPtr(type + _engine["MonoType"]["data"]);
    }

    internal MonoFieldAttribute TypeAttributes(nint type)
    {
        return _game.Read<MonoFieldAttribute>(type + _engine["MonoType"]["attrs"]);
    }

    internal MonoElementType TypeElementType(nint type)
    {
        return _game.Read<MonoElementType>(type + _engine["MonoType"]["type"]);
    }
}
