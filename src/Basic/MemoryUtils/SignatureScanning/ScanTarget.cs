namespace AslHelp.MemUtils.SigScan;

public class ScanTarget : IReadOnlyCollection<Signature>
{
    private readonly List<Signature> _signatures = new();

    public int Count => _signatures.Count;
    public Func<string, nint, nint> OnFound { get; set; }

    public void Add(Signature signature)
    {
        _signatures.Add(signature);
    }

    public ScanTarget Add(int offset, params string[] pattern)
    {
        _signatures.Add(new(offset, pattern));
        return this;
    }

    public ScanTarget Add(string name, int offset, params string[] pattern)
    {
        _signatures.Add(new(offset, pattern) { Name = name });
        return this;
    }

    public ScanTarget Add(int offset, params byte[] pattern)
    {
        _signatures.Add(new(offset, pattern));
        return this;
    }

    public ScanTarget Add(string name, int offset, params byte[] pattern)
    {
        _signatures.Add(new(offset, pattern) { Name = name });
        return this;
    }

    public IEnumerator<Signature> GetEnumerator()
    {
        return _signatures.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _signatures.GetEnumerator();
    }
}
