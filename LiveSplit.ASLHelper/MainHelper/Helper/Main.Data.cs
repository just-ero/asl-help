using ASLHelper.MainHelper;
using LiveSplit.Model;
using LiveSplit.UI;

namespace ASLHelper;

public partial class Main
{
    internal static Main Instance { get; private set; }

    public bool Is64Bit;
    public int PtrSize;
    internal ILayout Layout;
    internal LiveSplitState State;

    private readonly Form _form;
    private readonly object _script;

    public UIHelper UI { get; }
    public TimerHelper Timer { get; }
    public ASLSettingsHelper Settings { get; }

    public IOHelper IO { get; }
}
