namespace AslHelp.IO.Logging;

public abstract class Logger
{
    private readonly Dictionary<string, Stopwatch> _stopwatches = new();
    private readonly Dictionary<string, List<double>> _averages = new();

    public abstract void Log();
    public abstract void Log(object output);
    public abstract void Start();
    public abstract void Stop();

    public void Benchmark(string id, Action action)
    {
        if (id is null)
        {
            Log("[Benchmark] ID may not be null.");
            return;
        }

        StartBenchmark(id);
        action?.Invoke();
        StopBenchmark(id);
    }

    public void AvgBenchmark(string id, Action action)
    {
        if (id is null)
        {
            Log("[AvgBenchmark] ID may not be null.");
            return;
        }

        StartAvgBenchmark(id);
        action?.Invoke();
        StopAvgBenchmark(id);
    }

    public void StartBenchmark(string id)
    {
        if (id is null)
        {
            Log("[StartBenchmark] ID may not be null.");
            return;
        }

        _stopwatches[id] = Stopwatch.StartNew();
    }

    public void StopBenchmark(string id)
    {
        if (id is null)
        {
            Log("[StopBenchmark] ID may not be null.");
            return;
        }

        _stopwatches[id].Stop();
        Log($"[BENCH] [{id}] :: {_stopwatches[id].Elapsed}");
    }

    public void StartAvgBenchmark(string id)
    {
        if (id is null)
        {
            Log("[StartAvgBenchmark] ID may not be null.");
            return;
        }

        _stopwatches[id] = Stopwatch.StartNew();

        if (!_averages.ContainsKey(id))
        {
            _averages[id] = new();
        }
    }

    public void StopAvgBenchmark(string id)
    {
        if (id is null)
        {
            Log("[StopAvgBenchmark] ID may not be null.");
            return;
        }

        _stopwatches[id].Stop();

        _averages[id].Add(_stopwatches[id].Elapsed.TotalSeconds);
        Log($"[BENCH] [{id}] :: {_stopwatches[id].Elapsed} | Average: {_averages[id].Sum() / _averages[id].Count:0.0000000}");
    }
}
