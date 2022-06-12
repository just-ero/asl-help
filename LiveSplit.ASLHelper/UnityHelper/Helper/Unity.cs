using ASLHelper.UnityHelper;
using LiveSplit.Model;

namespace ASLHelper;

public partial class Unity : Main
{
    public Unity(LiveSplitState state, object settings, object compiledScript)
        : base(state, settings, compiledScript)
    {
        Debug.Log("  => Unity features will be available.");
    }

    public Unity(LiveSplitState state, object compiledScript)
        : this(state, null, compiledScript) { }

    private MonoHelper _helper;
    internal CancellationTokenSource CancelSource = new();

    private bool _loadSceneManager;
    public bool LoadSceneManager
    {
        get => _loadSceneManager;
        set
        {
            if (Game != null)
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
                Data.s_MonoModule = await GetMonoModule();
                if (Data.s_MonoModule.ModuleName == IL2CPP || LoadSceneManager)
                    Data.s_UnityPlayer = await GetUnityPlayer(unityPlayerRetries);

                _helper = MakeHelper();

                if (LoadSceneManager && Data.s_UnityPlayer != null)
                {
                    if (Data.s_SceneManager == 0)
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
        CancelSource.Cancel();

        base.Dispose();
    }
}
