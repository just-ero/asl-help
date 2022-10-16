using AslHelp.UE.Models;
using LiveSplit.ComponentUtil;

namespace AslHelp.UE.Managers;

internal partial class UE4_23Manager : UE4_20Manager
{
    public UE4_23Manager(int major, int minor)
        : base(major, minor) { }

    protected override nint FNamePool => _fNamePool + _engine["FNamePool"]["Entries"] + _engine["FNameEntryAllocator"]["Blocks"];

    internal override IEnumerable<FName> EnumerateFNames()
    {
        int ptrSize = _game.PtrSize, chunkId = 0;
        nint fNamePool = FNamePool, chunk = ReadPtr(fNamePool);

        while (chunk != 0)
        {
            nint cursor = chunk;
            int offset = getOffset(cursor);

            while (offset < ushort.MaxValue)
            {
                int length = FNameLength(cursor, out bool isWide);

                if (length == 0)
                {
                    break;
                }

                yield return new((chunkId << 16) | offset);

                cursor += 2 + length;

                if ((cursor & 1) != 0)
                {
                    cursor += 1;
                }

                offset = getOffset(cursor);
            }

            chunkId++;
            chunk = fNamePool + (ptrSize * chunkId);
        }

        int getOffset(nint cursor)
        {
            return (int)(cursor - (long)chunk) >> 1;
        }
    }

    internal override nint FNameNameEntry(int fNameIndex)
    {
        nint entry = ReadPtr(FNamePool + (_game.PtrSize * (fNameIndex >> 16)));
        return (entry + (fNameIndex & ushort.MaxValue)) << 1;
    }

    internal override int FNameIndex(nint fName)
    {
        throw new NotImplementedException();
    }

    internal override string FNameName(nint fName)
    {
        int length = FNameLength(fName, out bool isWide);
        ReadStringType type = isWide ? ReadStringType.UTF16 : ReadStringType.UTF8;

        return _game.ReadString(length, type, fName + _engine["FNameEntry"]["Name"]) ?? "";
    }

    internal virtual int FNameLength(nint fName, out bool isWide)
    {
        ushort fNameEntryHeader = _game.Read<ushort>(fName);

        isWide = (fNameEntryHeader & _engine["FNameEntryHeader"]["bIsWide"]) != 0;
        int length = fNameEntryHeader & _engine["FNameEntryHeader"]["Len"];

        return isWide ? length * 2 : length;
    }
}
