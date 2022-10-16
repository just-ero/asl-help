using AslHelp.Data;
using AslHelp.Data.AutoSplitter;
using AslHelp.MemUtils.Definitions;
using System.Windows.Forms;

public partial class Basic
{
    private void Startup(bool generateCode)
    {
        try
        {
            if (Actions.Current != "startup")
            {
                string msg = "The helper may only be instantiated in the 'startup {}' action.";
                throw new InvalidOperationException(msg);
            }

            LiveSplit.Options.Log.Info(Messages.WELCOME);
            Debug.Info();
            Debug.Info("Loading asl-help...");

            if (generateCode)
            {
                Debug.Info("  => Generating code...");
                GenerateCode();
            }

            TypeDefinitionFactory.Init();
        }
#if RELEASE
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(timer.Form,
                "asl-help aborted due to a startup error!" + Environment.NewLine +
                Environment.NewLine +
                ex,
                "LiveSplit | asl-help",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            throw;
        }
#else
        catch
        {
            throw;
        }
#endif
    }

    protected virtual void GenerateCode()
    {
        vars.Helper = this;
        Debug.Info("    => Set helper to vars.Helper.");

        vars.Log = (Action<object>)Log;
        Debug.Info("    => Created the Action<object> vars.Log.");

        Actions.shutdown.Prepend("vars.Helper.Dispose();");

        vars.StartBench = (Action<string>)StartBench;
        vars.StopBench = (Action<string>)StopBench;
    }
}
