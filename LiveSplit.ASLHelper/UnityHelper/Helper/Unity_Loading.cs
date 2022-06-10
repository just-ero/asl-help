using ASLHelper.UnityHelper;
using System.ComponentModel;

namespace ASLHelper;

public partial class Unity
{
    #region Consts
    private const string MONO_V1 = "mono.dll";
    private const string MONO_V2 = "mono-2.0-bdwgc.dll";
    private const string IL2CPP = "GameAssembly.dll";
    #endregion

    #region Properties
    public HashSet<string> Exceptions = new()
    {
        "RuntimeBinderException",
        "NullReferenceException",
        "InvalidOperationException"
    };

    public bool Loaded { get; private set; }
    public Func<MonoHelper, bool> TryOnLoad { get; set; }
    #endregion

    private async Task<ProcessModuleWow64Safe> GetMonoModule()
    {
        Debug.Log("Looking for Mono module...");
        _ = new List<string> { MONO_V1, MONO_V2, IL2CPP };

        while (true)
        {
            try
            {
                if (!TryGetModule(out var module, MONO_V1, MONO_V2, IL2CPP))
                {
                    break;
                }

                if (module != null)
                {
                    Debug.Log("  => Found " + module.ModuleName + ".");
                    Debug.Log();

                    return module;
                }
                else
                {
                    Debug.Log("  => Mono module not loaded yet. Retrying in 1 second...");
                }
            }
            catch (Win32Exception)
            {
                Debug.Log("  => Module list locked. Retrying in 1 second...");
            }
            catch (ArgumentException)
            {
                Debug.Log("  => Module list locked. Retrying in 1 second...");
            }

            await Task.Delay(1000, _cancelSource.Token);
        }

        return null;
    }

    private async Task<ProcessModuleWow64Safe> GetUnityPlayer(uint retries)
    {
        Debug.Log("Looking for UnityPlayer module...");

        for (int i = 0; i < retries; i++)
        {
            try
            {
                if (!TryGetModule(out var module, "UnityPlayer.dll"))
                {
                    break;
                }

                if (module != null)
                {
                    Debug.Log("  => Found.");
                    Debug.Log();

                    return module;
                }
                else
                {
                    var end = i + 1 == retries;
                    var msg = $"  => Not found on try {i + 1}/{retries}." + (end
                              ? "SceneManager features will not be available."
                              : "Retrying in 1 second...");

                    Debug.Log(msg);

                    if (end)
                        break;
                }
            }
            catch (Win32Exception)
            {
                Debug.Log("  => Module list locked. Retrying in 1 second...");
            }
            catch (ArgumentException)
            {
                Debug.Log("  => Module list locked. Retrying in 1 second...");
            }

            await Task.Delay(1000, _cancelSource.Token);
        }

        return null;
    }

    private MonoHelper MakeHelper()
    {
        if (Data.s_MonoModule == null)
            return null;

        Debug.Log("Creating Mono helper...");

        MonoHelper helper = null;
        switch (Data.s_MonoModule.ModuleName)
        {
            case MONO_V1:
            {
                break;
            }

            case MONO_V2:
            {
                break;
            }

            case IL2CPP:
            {
                break;
            }
        }

        Debug.Log("Created helper successfully.");
        Debug.Log();

        return helper;
    }

    private async Task<bool> DoOnLoad(uint timeout)
    {
        if (TryOnLoad == null || _helper == null)
            return true;

        int delay = (int)timeout;

        while (true)
        {
            _memWatchers.Clear();

            Debug.Log("Executing TryOnLoad...");

            try
            {
                if (TryOnLoad(_helper))
                {
                    Debug.Log("  => TryOnLoad successful.");
                    Debug.Log();

                    for (int i = _memWatchers.Count - 1; i >= 0; i--)
                    {
                        if (_memWatchers[i].Name == null)
                            _memWatchers.RemoveAt(i);
                    }

                    return true;
                }
                else
                {
                    Debug.Log($"  => TryOnLoad not successful.");
                    Debug.Log($"    => Retrying in {delay}ms...");
                    Debug.Log();

                    await Task.Delay(delay, _cancelSource.Token);
                }
            }
            catch (Exception ex)
            {
                var exType = ex.GetType().ToString();
                if (Exceptions != null && Exceptions.Any(e => exType.EndsWith(e, StringComparison.OrdinalIgnoreCase)))
                {
                    Debug.Log($"  => TryOnLoad not successful: {exType}:\n{ex.Message}");
                    Debug.Log($"    => Retrying in {delay}ms...");
                    Debug.Log();

                    _helper.ClearImages();

                    await Task.Delay(delay, _cancelSource.Token);
                    continue;
                }

                throw;
            }
        }
    }
}