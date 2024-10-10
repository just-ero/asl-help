using System.Collections.Generic;

using AslHelp.IO;
using AslHelp.Shared.Logging;

public partial class Basic
{
    protected readonly MultiLogger _logger = [];
    private readonly List<FileWatcher> _fileWatchers = [];

    public override void StartFileLogger(string fileName, int maxLines = 4096, int linesToErase = 512)
    {
        throw new System.NotImplementedException();
    }

    public override FileWatcher StartFileWatcher(string fileName)
    {
        throw new System.NotImplementedException();
    }
}
