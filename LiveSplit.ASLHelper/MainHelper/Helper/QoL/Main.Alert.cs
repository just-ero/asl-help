using LiveSplit.Model;

namespace ASLHelper;

public partial class Main
{
    public void AlertRealTime(string gameName = null)
    {
        if (State.CurrentTimingMethod == TimingMethod.RealTime)
            return;

        gameName ??= _gameName;

        var mbox = MessageBox.Show(_form,
            $"{gameName ?? "This game"} uses real time for its timing.\nWould you like to switch to it?",
            $"LiveSplit | {gameName ?? "Auto Splitter"}",
            MessageBoxButtons.YesNo);

        if (mbox == DialogResult.Yes)
            State.CurrentTimingMethod = TimingMethod.RealTime;
    }

    public void AlertGameTime(string gameName = null)
    {
        if (State.CurrentTimingMethod == TimingMethod.GameTime)
            return;

        gameName ??= _gameName;

        var mbox = MessageBox.Show(_form,
            $"{gameName ?? "This game"} uses in-game time.\nWould you like to switch to it?",
            $"LiveSplit | {gameName ?? "Auto Splitter"}",
            MessageBoxButtons.YesNo);

        if (mbox == DialogResult.Yes)
            State.CurrentTimingMethod = TimingMethod.GameTime;
    }

    public void AlertLoadless(string gameName = null)
    {
        if (State.CurrentTimingMethod == TimingMethod.GameTime)
            return;

        gameName ??= _gameName;

        var mbox = MessageBox.Show(_form,
            $"Removing loads from {gameName ?? "this game"} requires comparing against Game Time.\nWould you like to switch to it?",
            $"LiveSplit | {gameName ?? "Auto Splitter"}",
            MessageBoxButtons.YesNo);

        if (mbox == DialogResult.Yes)
            State.CurrentTimingMethod = TimingMethod.GameTime;
    }
}
