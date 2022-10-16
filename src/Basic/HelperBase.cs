using AslHelp.Data.AutoSplitter;
using AslHelp.MemUtils.Exceptions;
using AslHelp.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using System.ComponentModel;

namespace AslHelp;

public abstract class HelperBase<TManager> : Basic
{
    internal static TManager Manager { get; set; }

    private CancellationTokenSource _cts;
    internal bool LoadCanceled => _cts is not null && _cts.IsCancellationRequested;

    public HelperBase(bool generateCode)
        : base(generateCode)
    {
        Debug.Info($"  => {this} features will be available.");
    }

    protected override void GenerateCode()
    {
        base.GenerateCode();

        Actions.init.Append("vars.Helper.Load();");
        Actions.update.Prepend($"if (!vars.Helper.Loaded) return false; vars.Helper.MapPointers();");
        Actions.exit.Prepend("vars.Helper.Dispose();");
    }

    public uint TryLoadTimeout { get; set; } = 3000;
    public uint ModuleLoadTimeout { get; set; } = 1000;
    public uint ModuleLoadAttempts { get; set; } = 3;

    private bool _loaded;
    public bool Loaded
    {
        get
        {
            if (!_loaded)
            {
                return false;
            }

            Update();
            return true;
        }
        private set => _loaded = value;
    }

    private Func<TManager, bool> _tryLoad;
    public Func<TManager, bool> TryLoad
    {
        get => _tryLoad;
        set
        {
            if (Actions.Current != "init")
            {
                string msg = $"{nameof(TryLoad)} may only be set in the 'init {{}}' action.";
                throw new InvalidOperationException(msg);
            }

            _tryLoad = value;
        }
    }

    protected abstract Task<bool> LoadAsync();
    protected abstract TManager MakeManager();

    public void Load()
    {
        if (Actions.Current != "init")
        {
            string msg = $"{nameof(Load)}() may only be executed in the 'init {{}}' action.";
            throw new InvalidOperationException(msg);
        }

        if (_loaded)
        {
            return;
        }

        _cts = new();

        Task.Run(async () =>
        {
            try
            {
                if (!await LoadAsync())
                {
                    return;
                }

                if (TryLoad is null)
                {
                    Debug.Info();
                    Debug.Info(this + " loading complete.");

                    Loaded = true;

                    return;
                }

                Manager = MakeManager();

                Debug.Info();
                if (!await DoOnLoad())
                {
                    return;
                }

                Debug.Info();
                Debug.Info(this + " loading complete.");

                Loaded = true;
            }
            catch (Exception ex) when (ex is OperationCanceledException or TaskCanceledException) { }
            catch (Exception ex)
            {
                Debug.Throw(ex);
                throw;
            }
        });
    }

    protected async Task<bool> DoOnLoad()
    {
        if (Manager is null)
        {
            Debug.Warn("  => Cannot execute TryLoad because the manager is null.");
            return false;
        }

        return
            await TaskBuilder<bool>.Create(_cts)
            .WithStartupMessage("Executing TryLoad...")
            .Exec(ctx =>
            {
                if (!TryLoad(Manager))
                {
                    return false;
                }

                ctx.Result = true;
                return true;
            })
            .Catch<RuntimeBinderException>()
                .RetryOnFailure()
            .Catch<InvalidAddressException>()
                .RetryOnFailure()
            .Catch<NotFoundException>()
                .RetryOnFailure()
            .WithTimeout(TryLoadTimeout)
            .WithFailureMessage("TryLoad not successful.")
            .WithCompletionMessage("TryLoad successful.")
            .RunAsync();
    }

    protected async Task<Module> FindModuleAsync(params string[] moduleNames)
    {
        string names = moduleNames.Length == 1 ? "name" : "names";

        return
            await TaskBuilder<Module>.Create(_cts)
            .WithStartupMessage($"Searching for module with {names}: " + string.Join(", ", moduleNames) + "...")
            .Exec(ctx =>
            {
                if (Modules.FirstOrDefault(m => moduleNames.Contains(m.Name, StringComparer.OrdinalIgnoreCase)) is not Module module)
                {
                    return false;
                }

                ctx.Result = module;
                return true;
            })
            .Catch<Win32Exception>()
                .WithFailureMessage("  => Module list locked.")
                .RetryOnFailure()
            .Catch<ArgumentException>()
                .WithFailureMessage("  => Module list locked.")
                .RetryOnFailure()
            .WithRetries(ModuleLoadAttempts - 1)
            .WithTimeout(ModuleLoadTimeout)
            .WithFailureMessage("  => No module found yet.")
            .WithCompletionMessage(m => $"  => Found {m.Name}.")
            .RunAsync();
    }

    public override void Dispose(bool removeTexts)
    {
        _cts?.Cancel();

        Loaded = default;
        Manager = default;

        base.Dispose(removeTexts);
    }
}
