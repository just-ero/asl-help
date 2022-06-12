using LiveSplit.Model;
using LiveSplit.UI;

namespace ASLHelper;

/// <summary>
///     A static data-class, holding relevant information about the helper.
/// </summary>
internal static partial class Data
{
    public static Main s_Helper;
    public static ILayout s_Layout;
    public static LiveSplitState s_State;

    public static ProcessModuleWow64Safe s_MonoModule;
    public static ProcessModuleWow64Safe s_UnityPlayer;

    public static void Dispose()
    {
        // Main
        s_Helper = null;
        s_Layout = null;
        s_State = null;

        // Unity
        s_MonoModule = null;
        s_UnityPlayer = null;
        s_SceneManager = 0;
    }
}
