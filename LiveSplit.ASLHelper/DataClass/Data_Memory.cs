using LiveSplit.ComponentUtil;
using System;
using static LiveSplit.ComponentUtil.SigScanTarget;

namespace ASLHelper
{
    internal static partial class Data
    {
        public static readonly OnFoundCallback s_OnFound = (p, _, addr)
            => p.Is64Bit() ? addr + 0x4 + p.ReadValue<int>(addr) : p.ReadPointer(addr);

        private static IntPtr g_runtimeSceneManager;
        public static IntPtr s_SceneManager
        {
            get
            {
                if (g_runtimeSceneManager != IntPtr.Zero)
                    return g_runtimeSceneManager;

                Debug.Log("Looking for g_runtimeSceneManager...");

                var helper = s_Helper as Unity;
                var scanner = new SignatureScanner(helper.Game, s_UnityPlayer.BaseAddress, s_UnityPlayer.ModuleMemorySize);
                var target = new SigScanTarget { OnFound = s_OnFound };

                if (helper.Is64Bit)
                {
                    target.AddSignature(3, "4C 8B 35 ???????? 33 F6 48 8B E9"); // 2021.2.11
                    target.AddSignature(3, "4C 8B 25 ???????? 33 F6 48 8B E9"); // 2017.2.0
                }
                else
                {
                    target.AddSignature(2, "8B 3D ???????? 33 D2 8B 75"); // 2021.2.11
                    target.AddSignature(2, "8B 0D ???????? 89 45 ?? 8A 45"); // 2019.4.24
                    target.AddSignature(2, "8B 0D ???????? 53 8D 41"); // 2017.2.0
                }

                var scan = scanner.Scan(target);
                if (scan != IntPtr.Zero)
                {
                    Debug.Log("  => Found at 0x" + scan.ToString("X") + ".");
                    Debug.Log();
                }
                else
                {
                    Debug.Log("  => Scan target could not be resolved!");
                    Debug.Log();
                }

                g_runtimeSceneManager = scan;
                return scan;
            }
            set => g_runtimeSceneManager = value;
        }
    }
}