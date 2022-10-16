using AslHelp.MemUtils.Reflect;
using LiveSplit.ComponentUtil;

namespace AslHelp.UE.Managers;

public abstract partial class UEMemManager
{
    private protected readonly Unreal _game = Unreal.Instance;
    private protected readonly EngineReflection _engine;

    internal UEMemManager(int major, int minor)
    {
        if ((major, minor) is ( <= 4, < 8) && _game.Is64Bit)
        {
            string msg =
                "64-bit detected. " + Environment.NewLine +
                "Unreal Engine versions before 4.8 do not officially support 64-bit in shipping configurations.";

            throw new NotSupportedException(msg);
        }

        _engine = EngineReflection.Load("Unreal", major.ToString(), minor.ToString());
        Scan(major, minor);
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
