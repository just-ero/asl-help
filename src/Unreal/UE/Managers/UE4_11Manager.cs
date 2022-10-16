namespace AslHelp.UE.Managers;

internal partial class UE4_11Manager : UE4_8Manager
{
    public UE4_11Manager(int major, int minor)
        : base(major, minor) { }

    internal override nint UObjectsData
    {
        get
        {
            nint objObjects = _gUObjectArray + _engine["FUObjectArray"]["ObjObjects"];
            return ReadPtr(objObjects + _engine["FFixedUObjectArray"]["Objects"]);
        }
    }

    internal override int UObjectsSize
    {
        get
        {
            nint objObjects = _gUObjectArray + _engine["FUObjectArray"]["ObjObjects"];
            return ReadI32(objObjects + _engine["FFixedUObjectArray"]["NumElements"]);
        }
    }

    internal override IEnumerable<nint> EnumerateUObjects()
    {
        int uObjectsSize = UObjectsSize, fUObjectItemSize = _engine["FUObjectItem"].SelfAlignedSize;
        nint uObjectsData = UObjectsData;

        for (int i = 0; i < uObjectsSize; i++)
        {
            nint uObject = ReadPtr(uObjectsData + (fUObjectItemSize * i));
            if (uObject != 0)
            {
                yield return uObject;
            }
        }
    }
}
