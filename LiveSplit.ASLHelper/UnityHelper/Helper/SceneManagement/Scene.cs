namespace ASLHelper.UnityHelper;

public class Scene
{
    public Scene(params int[] offsets)
    {
        _offsets = offsets;
    }

    #region Fields
    private readonly int[] _offsets;
    #endregion

    #region Properties
    public bool IsValid
    {
        get => Data.s_Helper.Game.ReadPointer(Deref(0x0)) != IntPtr.Zero;
    }

    public IntPtr Address
    {
        get => Data.s_Helper.Game.ReadPointer(Deref());
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
        get => Data.s_Helper.Game.ReadValue<int>(Deref(SceneHelper.SceneOffsets[5]));
    }
    #endregion

    private IntPtr Deref(params int[] offsets)
    {
        var deref = Data.s_Helper.Deref(Data.s_SceneManager, _offsets);
        return Data.s_Helper.Deref(deref, offsets);
    }
}