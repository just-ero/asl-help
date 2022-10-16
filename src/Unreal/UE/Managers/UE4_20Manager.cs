namespace AslHelp.UE.Managers;

internal partial class UE4_20Manager : UE4_11Manager
{
    public UE4_20Manager(int major, int minor)
        : base(major, minor) { }

    internal override nint UObjectsData
    {
        get
        {
            nint objObjects = _gUObjectArray + _engine["FUObjectArray"]["ObjObjects"];
            return ReadPtr(objObjects + _engine["FChunkedFixedUObjectArray"]["Objects"]);
        }
    }

    internal override int UObjectsSize
    {
        get
        {
            nint objObjects = _gUObjectArray + _engine["FUObjectArray"]["ObjObjects"];
            return ReadI32(objObjects + _engine["FChunkedFixedUObjectArray"]["NumElements"]);
        }
    }

    internal override IEnumerable<nint> EnumerateUObjects()
    {
        int ptrSize = _game.PtrSize, chunkId = 0, fUObjectSize = _engine["FUObjectItem"].SelfAlignedSize;
        nint uObjectsData = UObjectsData, chunk = ReadPtr(uObjectsData);

        while (chunk != 0)
        {
            for (int i = 0; i < 0x10000; ++i)
            {
                nint uObject = _game.Read<nint>(chunk + (i * fUObjectSize));

                if (uObject != 0)
                {
                    yield return uObject;
                }
            }

            chunkId++;
            chunk = _game.Read<nint>(uObjectsData + (ptrSize * chunkId));
        }
    }
}
