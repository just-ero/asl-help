using LiveSplit.Model;
using System.Windows.Forms;

public partial class Basic
{
    public void AlertRealTime()
    {
        if (timer.CurrentTimingMethod == TimingMethod.RealTime)
        {
            return;
        }

        DialogResult mbox = MessageBox.Show(timer.Form,
            $"{_gameName ?? "This game"} uses real time for its timing.\nWould you like to switch to it?",
            $"LiveSplit | {GameName}",
            MessageBoxButtons.YesNo);

        if (mbox == DialogResult.Yes)
        {
            timer.CurrentTimingMethod = TimingMethod.RealTime;
        }
    }

    public void AlertGameTime()
    {
        if (timer.CurrentTimingMethod == TimingMethod.GameTime)
        {
            return;
        }

        DialogResult mbox = MessageBox.Show(timer.Form,
            $"{_gameName ?? "This game"} uses in-game time.\nWould you like to switch to it?",
            $"LiveSplit | {GameName}",
            MessageBoxButtons.YesNo);

        if (mbox == DialogResult.Yes)
        {
            timer.CurrentTimingMethod = TimingMethod.GameTime;
        }
    }

    public void AlertLoadless()
    {
        if (timer.CurrentTimingMethod == TimingMethod.GameTime)
        {
            return;
        }

        DialogResult mbox = MessageBox.Show(timer.Form,
            $"Removing loads from {_gameName ?? "this game"} requires comparing against Game Time.\nWould you like to switch to it?",
            $"LiveSplit | {GameName}",
            MessageBoxButtons.YesNo);

        if (mbox == DialogResult.Yes)
        {
            timer.CurrentTimingMethod = TimingMethod.GameTime;
        }
    }
}
