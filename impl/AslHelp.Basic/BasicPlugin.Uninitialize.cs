public partial class Basic
{
    protected override void DisposePlugin(bool closing)
    {
        _logger.Stop();
        _logger.Clear();

        _fileWatchers.ForEach(w => w.Dispose());
        _fileWatchers.Clear();

        if (!closing)
        {
            Texts?.Clear();
        }
    }
}
