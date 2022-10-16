using AslHelp.MemUtils.Reflect;
using LiveSplit.ComponentUtil;

namespace AslHelp.Mono.Managers;

public abstract partial class UnityMemManager
{
    private protected readonly Unity _game = Unity.Instance;
    private protected EngineReflection _engine;

    internal UnityMemManager(string type, string version)
    {
        Unity.Manager = this;
        _engine = EngineReflection.Load("Unity", type, version);
    }

    private protected nint ReadPtr(nint address)
    {
        return _game.Read<nint>(address);
    }

    private protected int ReadI32(nint address)
    {
        return _game.Read<int>(address);
    }

    private protected uint ReadU32(nint address)
    {
        return _game.Read<uint>(address);
    }

    private protected ushort ReadU16(nint address)
    {
        return _game.Read<ushort>(address);
    }

    private protected byte ReadU8(nint address)
    {
        return _game.Read<byte>(address);
    }

    private protected string ReadStr(nint address, int length)
    {
        return _game.ReadString(length, ReadStringType.UTF8, ReadPtr(address));
    }
}
