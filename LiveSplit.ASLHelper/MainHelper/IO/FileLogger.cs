namespace ASLHelper.MainHelper;

internal class FileLogger
{
    public FileLogger(string path)
    {
        _path = path;
        _lines = File.ReadAllLines(path).Length;
        Writer = new(path, true);
    }

    private int _lines;
    private readonly string _path;
    public readonly StreamWriter Writer;

    public void Log(object output)
    {
        if (output is null)
        {
            Writer.WriteLine();
            _lines++;
        }
        else
        {
            Writer.WriteLine($"{DateTime.Now:G} | " + output);
            _lines++;
        }

        if (_lines > 4096)
        {
            Flush();
        }
    }

    private void Flush()
    {
        var tempFile = $"{_path}-temp";
        var lines = File.ReadAllLines(_path).Skip(4096 - 512).ToArray();

        File.WriteAllLines(tempFile, lines);

        try
        {
            File.Copy(tempFile, _path, true);
            _lines = lines.Length;
        }
        catch
        {
            Debug.Warn("Failed replacing log file with temporary log file.");
        }
        finally
        {
            try
            {
                File.Delete(tempFile);
            }
            catch
            {
                Debug.Warn("Failed deleting temporary log file.");
            }
        }
    }
}
