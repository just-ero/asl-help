namespace ASLHelper.UnityHelper;

public class Scene
{
    internal Scene(params int[] offsets)
    {
        _offsets = offsets;
    }

    private readonly int[] _offsets;

    public bool IsValid
    {
        get => Unity.Instance.Read<nint>(Deref(0x0)) != 0;
    }

    public nint Address
    {
        get => Unity.Instance.Read<nint>(Deref());
    }

    public string Name
    {
        get
        {
            var path = Unity.Instance.ReadString(256, ReadStringType.UTF8, Deref(SceneHelper.SceneOffsets[4], 0x0));

            if (string.IsNullOrEmpty(path))
            {
                return "";
            }

            return Path.GetFileNameWithoutExtension(path);
        }
    }

    public int Index
    {
        get => Unity.Instance.Read<int>(Deref(SceneHelper.SceneOffsets[5]));
    }

    private nint Deref(params int[] offsets)
    {
        var deref = Unity.Instance.Deref(SceneHelper.SceneManager, _offsets);
        return Unity.Instance.Deref(deref, offsets);
    }
}
