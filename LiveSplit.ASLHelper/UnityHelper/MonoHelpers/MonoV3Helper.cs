namespace ASLHelper.UnityHelper;

public class MonoV3Helper : MonoV2Helper
{
    public MonoV3Helper(Unity helper, string type, string version)
        : base(helper, type, version) { }

    private protected override nint ScanForImages()
    {

        Debug.Log("here");
        return _helper.ScanRel(Unity.Instance.MonoModule, _engine.Signatures["global_loaded_images"]) + 0x10;
    }
}
