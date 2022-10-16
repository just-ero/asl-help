using AslHelp.Mono.Models;

namespace AslHelp.Mono.Managers;

internal class MonoV3Manager : MonoV2Manager
{
    public MonoV3Manager(string version)
        : base(version) { }

    internal override MonoTypeKind ClassTypeKind(nint klass)
    {
        return _game.Read<MonoTypeKind>(klass + _engine["MonoClass"]["class_kind"]);
    }
}
