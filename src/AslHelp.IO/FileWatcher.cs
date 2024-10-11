using System;
using System.IO;

namespace AslHelp.IO;

public sealed class FileWatcher : IDisposable
{
    private readonly string _path;

    private bool _open;
    private StreamReader? _reader;

    public FileWatcher(string path)
    {
        _path = path;
    }

    // TODO: Use semi-auto property in RC2.
#pragma warning disable IDE0032 // Use auto property
    private string? _line;
#pragma warning restore IDE0032
    public string? Line
    {
        get
        {
            if (!_open && !TryOpen())
            {
                return null;
            }

            if (_reader?.ReadLine() is string line)
            {
                _line = line;
            }

            return _line;
        }
    }

    private bool TryOpen()
    {
        if (File.Exists(_path))
        {
            FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _reader = new(stream);

            while (_reader.ReadLine() is string line)
            {
                _line = line;
            }

            _open = true;
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        _reader?.Dispose();
        _reader = null;

        _open = false;
    }
}
