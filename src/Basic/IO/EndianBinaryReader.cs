using System.Text;

namespace AslHelp.IO;

public class EndianBinaryReader : BinaryReader
{
    public EndianBinaryReader(Stream stream, bool isBigEndian = false)
        : base(stream)
    {
        IsBigEndian = isBigEndian;
    }

    public EndianBinaryReader(string path, bool isBigEndian = false)
        : this(File.OpenRead(path), isBigEndian) { }

    public bool IsBigEndian { get; private set; }

    public override byte ReadByte()
    {
        return IsBigEndian ? (byte)ReadBytesBigEndian(1) : base.ReadByte();
    }

    public override sbyte ReadSByte()
    {
        return IsBigEndian ? (sbyte)ReadBytesBigEndian(1) : base.ReadSByte();
    }

    public override ushort ReadUInt16()
    {
        return IsBigEndian ? (ushort)ReadBytesBigEndian(2) : base.ReadUInt16();
    }

    public override short ReadInt16()
    {
        return IsBigEndian ? (short)ReadBytesBigEndian(2) : base.ReadInt16();
    }

    public override uint ReadUInt32()
    {
        return IsBigEndian ? (uint)ReadBytesBigEndian(4) : base.ReadUInt32();
    }

    public override int ReadInt32()
    {
        return IsBigEndian ? (int)ReadBytesBigEndian(4) : base.ReadInt32();
    }

    public override ulong ReadUInt64()
    {
        return IsBigEndian ? ReadBytesBigEndian(8) : base.ReadUInt64();
    }

    public override long ReadInt64()
    {
        return IsBigEndian ? (long)ReadBytesBigEndian(8) : base.ReadInt64();
    }

    public override string ReadString()
    {
        if (!IsBigEndian)
        {
            return base.ReadString();
        }

        List<byte> bytes = new();

        while (BaseStream.Position != BaseStream.Length && bytes.Count < 2048)
        {
            byte b = base.ReadByte();
            if (b == 0)
            {
                break;
            }

            bytes.Add(b);
        }

        return Encoding.UTF8.GetString(bytes.ToArray());
    }

    private ulong ReadBytesBigEndian(int count)
    {
        byte[] bytes = base.ReadBytes(count);
        ulong result = bytes[^1];

        for (int i = count - 2; i >= 0; i--)
        {
            result |= (uint)bytes[i] << ((count - 1 - i) * 8);
        }

        return result;
    }
}
