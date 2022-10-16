namespace AslHelp.SceneManagement;

public partial class SceneManager
{
    internal SceneManager()
    {
        Active = new(Offsets.ActiveScene);
    }

    public Scene Active { get; }

    public int Count => Unity.Instance.Read<int>(Address, Offsets.SceneCount);

    public SceneEnumerator Loaded { get; } = new(Offsets.LoadedScenes);

}
