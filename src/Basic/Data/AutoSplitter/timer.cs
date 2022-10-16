using LiveSplit.Model;
using LiveSplit.UI;
using System.Windows.Forms;

namespace AslHelp.Data.AutoSplitter;

#pragma warning disable IDE1006
internal static class timer
#pragma warning restore IDE1006
{
    static timer()
    {
        dynamic form = Application.OpenForms["TimerForm"];
        dynamic state = form.CurrentState;

        State = state;
    }

    public static LiveSplitState State { get; }

    public static IRun Run
    {
        get => State.Run;
        set => State.Run = value;
    }

    public static ILayout Layout
    {
        get => State.Layout;
        set => State.Layout = value;
    }

    public static Form Form
    {
        get => State.Form;
        set => State.Form = value;
    }

    public static TimingMethod CurrentTimingMethod
    {
        get => State.CurrentTimingMethod;
        set => State.CurrentTimingMethod = value;
    }
}
