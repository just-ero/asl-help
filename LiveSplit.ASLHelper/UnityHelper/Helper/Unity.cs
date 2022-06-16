using ASLHelper.UnityHelper;
using LiveSplit.Model;

namespace ASLHelper;

public partial class Unity : Main
{
    internal static new Unity Instance { get; private set; }

    private MonoHelper _helper;
    internal CancellationTokenSource CancelSource;

    public Unity(LiveSplitState state, object settings, object compiledScript)
        : base(state, settings, compiledScript)
    {
        Instance = this;

        Debug.Log("  => Unity features will be available.");
    }

    public Unity(LiveSplitState state, object compiledScript)
        : this(state, null, compiledScript) { }

    public ProcessModuleWow64Safe MonoModule { get; private set; }
    public ProcessModuleWow64Safe UnityPlayer { get; private set; }

    private bool _loadSceneManager;
    public bool LoadSceneManager
    {
        get => _loadSceneManager;
        set
        {
            if (Game is not null)
            {
                var msg = $"{nameof(LoadSceneManager)} must be set before entering the 'init {{}}' action.";
                throw new InvalidOperationException(msg);
            }

            Debug.Log(value ? "  => Will try to load SceneManager." : "  => Will not load SceneManager.");
            _loadSceneManager = value;
        }
    }

    public SceneHelper Scenes { get; private set; }

    public void Load(uint timeout = 3000, uint unityPlayerRetries = 1)
    {
        _ = Task.Run(async () =>
        {
            CancelSource = new();

            try
            {
                MonoModule = await GetMonoModule();
                if (MonoModule.ModuleName == IL2CPP || LoadSceneManager)
                    UnityPlayer = await GetUnityPlayer(unityPlayerRetries);

                _helper = MakeHelper();

                if (LoadSceneManager && UnityPlayer is not null)
                {
                    if (SceneHelper.SceneManager == 0)
                    {
                        Debug.Log("    => SceneManager will not be available!");
                    }
                    else
                    {
                        Scenes = new();
                    }
                }

                _ = await DoOnLoad(timeout);

                Loaded = true;
                Debug.Log("Mono loading complete!");
            }
            catch (TaskCanceledException) { }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Debug.Throw(ex);
                throw;
            }
        });
    }

    public override void Dispose()
    {
        Dispose(true);
    }

    public override void Dispose(bool removeTexts)
    {
        CancelSource.Cancel();
        base.Dispose(removeTexts);
    }
}
