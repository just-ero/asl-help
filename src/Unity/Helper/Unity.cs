using AslHelp;
using AslHelp.Mono.Managers;
using AslHelp.SceneManagement;

public partial class Unity : HelperBase<UnityMemManager>
{
    internal static new Unity Instance { get; private set; }

    public Unity()
        : this(true) { }

    public Unity(bool generateCode)
        : base(generateCode)
    {
        Instance = this;
    }

    protected override async Task<bool> LoadAsync()
    {
        if (LoadSceneManager)
        {
            UnityPlayer = await FindModuleAsync("UnityPlayer.dll");

            if (LoadCanceled)
            {
                return false;
            }

            if (UnityPlayer is not null && SceneManager.Address != 0)
            {
                Scenes = new();
                Debug.Info("    => SceneManager will be available.");
            }
            else
            {
                Debug.Info("    => SceneManager will not be available.");
            }
        }

        if (TryLoad is null)
        {
            return true;
        }

        MonoModule = await FindModuleAsync([.. MonoV1Modules, .. MonoV2Modules, .. Il2CppModules]);

        return true;
    }

    protected override void OnTryLoadFailure()
    {
        Manager.Images.Clear();
    }

    public override void Dispose(bool removeTexts)
    {
        _unityVersion = default;

        Scenes = default;
        SceneManager.Address = default;

        MonoModule = default;
        UnityPlayer = default;

        base.Dispose(removeTexts);
    }
}
