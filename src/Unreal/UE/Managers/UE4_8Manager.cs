namespace AslHelp.UE.Managers;

internal partial class UE4_8Manager : UE4_0Manager
{
    public UE4_8Manager(int major, int minor)
        : base(major, minor) { }

    internal override nint UObjectsData
    {
        get
        {
            nint objObjects = _gUObjectArray + _engine["FUObjectArray"]["ObjObjects"];
            return objObjects + _engine["TUObjectArray"]["Chunks"];
        }
    }

    internal override int UObjectsSize => throw new NotImplementedException();

    internal override IEnumerable<nint> EnumerateUObjects()
    {
        int ptrSize = _game.PtrSize, chunkId = 0;
        nint objsData = UObjectsData, chunk = 0;

        while (chunk != 0)
        {
            nint[] uObjects = new nint[0x4000];
            if (!_game.TryReadSpan(uObjects, chunk))
            {
                goto Next;
            }

            for (int i = 0; i < uObjects.Length; i++)
            {
                if (uObjects[i] != 0)
                {
                    yield return uObjects[i];
                }
            }

        Next:
            chunkId++;
            chunk = ReadPtr(objsData + (chunkId * ptrSize));
        }
    }
}
