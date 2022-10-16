using LiveSplit.ComponentUtil;

namespace AslHelp.SceneManagement;

public class Scene
{
    private readonly int[] _offsets;

    internal Scene(params int[] offsets)
    {
        _offsets = offsets;
    }

    public nint Address => Unity.Instance.Read<nint>(SceneManager.Address, _offsets);
    public bool IsValid => Unity.Instance.TryRead<nint>(out _, Address);

    public int Index => Unity.Instance.Read<int>(Address + SceneManager.Offsets.BuildIndex);
    public string Path => Unity.Instance.ReadString(256, ReadStringType.UTF8, Address + SceneManager.Offsets.AssetPath, 0x0);
    public string Name => Path is string path ? System.IO.Path.GetFileNameWithoutExtension(path) : null;
}
