using ASLHelper.UnityHelper;
using LiveSplit.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASLHelper
{
    public partial class Unity : Main
    {
        public Unity(LiveSplitState state, object settings, object compiledScript)
            : base(state, settings, compiledScript)
        {
            Debug.Log("  => Unity features will be available.");
        }

        public Unity(LiveSplitState state, object compiledScript)
            : this(state, null, compiledScript) { }

        #region Fields
        private readonly CancellationTokenSource _cancelSource = new CancellationTokenSource();
        #endregion

        public void Load(uint timeout = 3000, uint unityPlayerRetries = 1)
        {
            Task.Run(async () =>
            {
                try
                {
                    Data.s_MonoModule = await GetMonoModule();
                    if (Data.s_MonoModule.ModuleName == IL2CPP || LoadSceneManager)
                        Data.s_UnityPlayer = await GetUnityPlayer(unityPlayerRetries);

                    _helper = MakeHelper();

                    if (LoadSceneManager && Data.s_UnityPlayer != null)
                    {

                        if (Data.s_SceneManager == IntPtr.Zero)
                        {
                            Debug.Log("    => SceneManager will not be available!");
                        }
                        else
                        {
                            Scenes = new SceneHelper();
                        }
                    }

                    await DoOnLoad(timeout);

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
            _cancelSource.Cancel();

            base.Dispose();
        }
    }
}