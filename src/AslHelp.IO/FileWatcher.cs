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
                field = line;
            }

            return field;
        }

        private set;
    }

    private bool TryOpen()
    {
        if (File.Exists(_path))
        {
            FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _reader = new(stream);

            while (_reader.ReadLine() is string line)
            {
                Line = line;
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
