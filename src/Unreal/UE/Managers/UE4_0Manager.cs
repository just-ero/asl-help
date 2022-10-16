using AslHelp.MemUtils.Exceptions;
using AslHelp.UE.Models;
using LiveSplit.ComponentUtil;

namespace AslHelp.UE.Managers;

internal partial class UE4_0Manager : UEMemManager
{
    public UE4_0Manager(int major, int minor)
        : base(major, minor) { }

    protected override nint FNamePool
    {
        get
        {
            if (_fNamePool != -1)
            {
                return ReadPtr(_fNamePool + _engine["TStaticIndirectArrayThreadSafeRead"]["Chunks"]);
            }

            _fNamePool = _game.ScanRel(_fNamesTrg);

            if (_fNamePool == 0)
            {
                throw new NotFoundException("Could not find FNamePool!");
            }

            return FNamePool;
        }
    }

    internal override nint UObjectsData
    {
        get
        {
            nint objObjects = _gUObjectArray + _engine["FUObjectArray"]["ObjObjects"];
            return ReadPtr(objObjects + _engine["TArray"]["AllocatorInstance"]);
        }
    }

    internal override int UObjectsSize
    {
        get
        {
            nint objObjects = _gUObjectArray + _engine["FUObjectArray"]["ObjObjects"];
            return ReadI32(objObjects + _engine["TArray"]["ArrayNum"]);
        }
    }

    internal override IEnumerable<FName> EnumerateFNames()
    {
        int ptrSize = _game.PtrSize, chunkId = 0;
        nint fNamePool = FNamePool, chunk = ReadPtr(fNamePool);

        while (chunk != 0)
        {
            nint[] fNames = _game.ReadSpan<nint>(0x4000, chunk);

            for (int i = 0; i < fNames.Length; i++)
            {
                if (fNames[i] != 0)
                {
                    yield return new(fNames[i]);
                }
            }

            chunkId++;
            chunk = ReadPtr(fNamePool + (ptrSize * chunkId));
        }
    }

    internal override nint FNameNameEntry(int fNameIndex)
    {
        nint entry = ReadPtr(FNamePool + (fNameIndex / 0x4000 * _game.PtrSize));

        return ReadPtr(entry + (fNameIndex % 0x4000 * _game.PtrSize));
    }

    internal override int FNameIndex(nint fName)
    {
        return ReadI32(fName + _engine["FNameEntry"]["Index"]) >> 1;
    }

    internal override string FNameName(nint fName)
    {
        bool isWide = (ReadI32(fName + _engine["FNameEntry"]["Index"]) & 1) != 0;
        ReadStringType stringType = isWide ? ReadStringType.UTF16 : ReadStringType.UTF8;

        return _game.ReadString(128, stringType, fName + _engine["FNameEntry"]["Name"]);
    }

    internal override IEnumerable<nint> EnumerateUObjects()
    {
        int size = UObjectsSize, ptrSize = _game.PtrSize;
        nint uObjData = UObjectsData;

        nint[] uObjects = _game.ReadSpan<nint>(size, uObjData);

        for (int i = 0; i < uObjects.Length; i++)
        {
            if (uObjects[i] != 0)
            {
                yield return uObjects[i];
            }
        }
    }

    internal override nint UObjectClass(nint uObject)
    {
        return ReadPtr(uObject + _engine["UObject"]["Class"]);
    }

    internal override nint UObjectSuper(nint uObject)
    {
        return ReadPtr(uObject + _engine["UStruct"]["SuperStruct"]);
    }

    internal override nint UObjectOuter(nint uObject)
    {
        return ReadPtr(uObject + _engine["UObject"]["Outer"]);
    }

    internal override int UObjectFNameIndex(nint uObject)
    {
        return ReadI32(uObject + _engine["UObject"]["Name"]);
    }

    internal override int UObjectPropertiesSize(nint uObject)
    {
        return ReadI32(uObject + _engine["UStruct"]["PropertiesSize"]);
    }

    internal override IEnumerable<nint> EnumerateUFields(nint uObject)
    {
        int next = _engine["UField"]["Next"];

        while (uObject != 0)
        {
            nint child = ReadPtr(uObject + _engine["UStruct"]["Children"]);

            for (; child != IntPtr.Zero; child = ReadPtr(child + next))
            {
                int classFNameIndex = UFieldFNameIndex(UFieldClass(child));
                if (classFNameIndex == 0 || FNameName(classFNameIndex) == "Function")
                {
                    continue;
                }

                if (UFieldFNameIndex(child) != 0)
                {
                    yield return child;
                }
            }

            uObject = UObjectSuper(uObject);
        }
    }

    internal override nint UFieldClass(nint uField)
    {
        return UObjectClass(uField);
    }

    internal override FName UFieldClassFName(nint uField)
    {
        return UObjectClassFName(uField);
    }

    internal override int UFieldFNameIndex(nint uField)
    {
        return UObjectFNameIndex(uField);
    }

    internal override int UFieldOffset(nint uField)
    {
        return ReadI32(uField + _engine["UProperty"]["Offset_Internal"]);
    }
}
