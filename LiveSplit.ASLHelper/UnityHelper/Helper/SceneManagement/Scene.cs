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
        get => Data.s_Helper.Read<nint>(Deref(0x0)) != 0;
    }

    public nint Address
    {
        get => Data.s_Helper.Read<nint>(Deref());
    }

    public string Name
    {
        get
        {
            var path = Data.s_Helper.Game.ReadString(Deref(SceneHelper.SceneOffsets[4], 0x0), ReadStringType.UTF8, 256);

            if (string.IsNullOrEmpty(path))
            {
                return "";
            }

            int folder = path.LastIndexOf('/'), exension = path.LastIndexOf(".unity");
            if (exension == -1)
            {
                path = path.Remove(exension);
            }

            return path.Substring(folder + 1);
        }
    }

    public int Index
    {
        get => Data.s_Helper.Read<int>(Deref(SceneHelper.SceneOffsets[5]));
    }

    private nint Deref(params int[] offsets)
    {
        var deref = Data.s_Helper.Deref(Data.s_SceneManager, _offsets);
        return Data.s_Helper.Deref(deref, offsets);
    }
}
