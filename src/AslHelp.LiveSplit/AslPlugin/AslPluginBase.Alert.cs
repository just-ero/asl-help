using System.Windows.Forms;

using LiveSplit.Model;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    public AslPluginBase AlertRealTime(string? message = default)
    {
        EnsureInitialized();

        if (_asl.State.CurrentTimingMethod == TimingMethod.RealTime)
        {
            return this;
        }

        DialogResult result = MessageBox.Show(
            message ??
            $"{_gameName ?? "This game"} uses real time as its timing method.\nWould you like to switch to it?",
            $"LiveSplit | {GameName}",
            MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
            _asl.State.CurrentTimingMethod = TimingMethod.RealTime;
        }

        return this;
    }

    public AslPluginBase AlertGameTime(string? message = default)
    {
        EnsureInitialized();

        if (_asl.State.CurrentTimingMethod == TimingMethod.GameTime)
        {
            return this;
        }

        DialogResult result = MessageBox.Show(
            message ??
            $"{_gameName ?? "This game"} uses in-game time.\nWould you like to switch to it?",
            $"LiveSplit | {GameName}",
            MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
            _asl.State.CurrentTimingMethod = TimingMethod.GameTime;
        }

        return this;
    }

    public AslPluginBase AlertLoadless(string? message = default)
    {
        EnsureInitialized();

        if (_asl.State.CurrentTimingMethod == TimingMethod.GameTime)
        {
            return this;
        }

        DialogResult result = MessageBox.Show(
            message ??
            $"Removing loads from {_gameName ?? "this game"} requires comparing against Game Time.\nWould you like to switch to it?",
            $"LiveSplit | {GameName}",
            MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
            _asl.State.CurrentTimingMethod = TimingMethod.GameTime;
        }

        return this;
    }
}
