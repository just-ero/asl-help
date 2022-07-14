using ASLHelper.UnityHelper;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;

namespace ASLHelper;

public partial class Unity
{
    private const string MONO_V1 = "mono.dll";
    private const string MONO_V2 = "mono-2.0-bdwgc.dll";
    private const string IL2CPP = "GameAssembly.dll";
    public HashSet<string> Exceptions = new()
    {
        "RuntimeBinderException",
        "NullReferenceException",
        "InvalidOperationException"
    };

    public bool Loaded { get; private set; }
    public Func<MonoHelper, bool> TryOnLoad { get; set; }

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

                if (module is not null)
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

            await Task.Delay(1000, CancelSource.Token);
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

                if (module is not null)
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

            await Task.Delay(1000, CancelSource.Token);
        }

        return null;
    }

    private MonoHelper MakeHelper()
    {
        if (MonoModule is null)
            return null;

        Debug.Log("Creating Mono helper...");

        var fvi = Game.MainModule.FileVersionInfo;
        MonoHelper helper = MonoModule.ModuleName switch
        {
            MONO_V1 => new MonoV1Helper(this, "mono", "v1"),
            MONO_V2 => (fvi.FileMajorPart, fvi.FileMinorPart) switch
            {
                (2021, >= 2) or ( > 2021, _) => new MonoV3Helper(this, "mono", "v3"),
                _ => new MonoV2Helper(this, "mono", "v2")
            },
            IL2CPP => new Il2CppHelper(this, "il2cpp", fvi.FileMajorPart switch
            {
                < 2019 => "base",
                2019 => "2019",
                > 2019 => "2020"
            }),
            _ => null
        };

        Debug.Log("  => Created helper successfully.");
        Debug.Log();

        return helper;
    }

    private async Task<bool> DoOnLoad(uint timeout)
    {
        if (TryOnLoad is null || _helper is null)
            return true;

        int delay = (int)timeout;

        while (true)
        {
            _watchers.Clear();

            Debug.Log("Executing TryOnLoad...");

            try
            {
                if (TryOnLoad(_helper))
                {
                    Debug.Log("  => TryOnLoad successful.");
                    Debug.Log();

                    for (int i = _watchers.Count - 1; i >= 0; i--)
                    {
                        if (_watchers[i].Name is null)
                            _watchers.RemoveAt(i);
                    }

                    return true;
                }
                else
                {
                    Debug.Log($"  => TryOnLoad not successful.");
                    Debug.Log($"    => Retrying in {delay}ms...");
                    Debug.Log();

                    await Task.Delay(delay, CancelSource.Token);
                }
            }
            catch (Exception ex)
            {
                var exType = ex.GetType().ToString();
                if (Exceptions is not null && Exceptions.Any(e => exType.EndsWith(e, StringComparison.OrdinalIgnoreCase)))
                {
                    Debug.Log($"  => TryOnLoad not successful: {exType}:\n{ex.Message}");
                    Debug.Log($"    => Retrying in {delay}ms...");
                    Debug.Log();

                    _helper.ClearImages();

                    await Task.Delay(delay, CancelSource.Token);
                    continue;
                }

                throw;
            }
        }
    }
}
