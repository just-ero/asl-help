using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using LiveSplit.UI;
using System;

namespace ASLHelper
{
    internal static partial class Data
    {
        public static Main s_Helper;
        public static ILayout s_Layout;
        public static LiveSplitState s_State;

        #region UnityHelper
        public static ProcessModuleWow64Safe s_MonoModule;
        public static ProcessModuleWow64Safe s_UnityPlayer;
        #endregion

        public static void Dispose()
        {
            s_Helper = null;
            s_Layout = null;
            s_State = null;

            s_MonoModule = null;
            s_UnityPlayer = null;
            s_SceneManager = IntPtr.Zero;
        }
    }
}