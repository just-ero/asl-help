using AslHelp.IO;
using AslHelp.LiveSplit.Control;
using AslHelp.LiveSplit.Settings;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    public SettingsBuilder? Settings { get; private set; }
    public TimerController? Timer { get; private set; }
    public TextComponentController? Texts { get; private set; }

    public abstract void StartFileLogger(string fileName, int maxLines = 4096, int linesToErase = 512);
    public abstract FileWatcher StartFileWatcher(string fileName);
}
