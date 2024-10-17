extern alias Ls;

using System.Windows.Forms;

using Ls::LiveSplit.Model;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    public void AlertRealTime(string? message = default)
    {
        if (_asl.State.CurrentTimingMethod == TimingMethod.RealTime)
        {
            return;
        }

        DialogResult result = MessageBox.Show(
            message ??
            $"{GameName ?? "This game"} uses real time as its timing method.\nWould you like to switch now?",
            $"LiveSplit | {GameName}",
            MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
            _asl.State.CurrentTimingMethod = TimingMethod.RealTime;
        }
    }

    public void AlertGameTime(string? message = default)
    {
        if (_asl.State.CurrentTimingMethod == TimingMethod.GameTime)
        {
            return;
        }

        DialogResult result = MessageBox.Show(
            message ??
            $"{GameName ?? "This game"} uses in-game time.\nWould you like to switch now?",
            $"LiveSplit | {GameName}",
            MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
            _asl.State.CurrentTimingMethod = TimingMethod.GameTime;
        }
    }

    public void AlertLoadless(string? message = default)
    {
        if (_asl.State.CurrentTimingMethod == TimingMethod.GameTime)
        {
            return;
        }

        DialogResult result = MessageBox.Show(
            message ??
            $"Removing loads from {GameName ?? "this game"} requires comparing against Game Time.\nWould you like to switch now?",
            $"LiveSplit | {GameName}",
            MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
            _asl.State.CurrentTimingMethod = TimingMethod.GameTime;
        }
    }
}
