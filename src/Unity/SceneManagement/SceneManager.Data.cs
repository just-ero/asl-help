using AslHelp.Data;
using AslHelp.MemUtils.SigScan;

namespace AslHelp.SceneManagement;

public partial class SceneManager
{
    private static readonly ScanTarget _gRuntimeSceneManagerTrg =
        Unity.Instance.Is64Bit
        ? new()
        {
            { 7, "48 83 EC 20 4C 8B ?5 ???????? 33 F6" }
        }
        : new()
        {
            { 5, "55 8B EC 51 A1 ???????? 53 33 DB" },
            { -4, "53 8D 41 ?? 33 DB" },
            { 7, "55 8B EC 83 EC 18 A1 ???????? 33 C9 53" }
        };

    private static nint _address;
    public static nint Address
    {
        get
        {
            if (_address != default)
            {
                return _address;
            }

            Unity game = Unity.Instance;
            _gRuntimeSceneManagerTrg.OnFound = Memory.OnFound;

            Debug.Info("Looking for g_runtimeSceneManager...");

            _address = game.Scan(_gRuntimeSceneManagerTrg, game.UnityPlayer);

            if (_address != 0)
            {
                Debug.Info("  => Found at 0x" + _address.ToString("X") + ".");
            }
            else
            {
                Debug.Info("  => Scan targets could not be resolved.");
            }

            return _address;
        }
        internal set => _address = value;
    }

    internal static class Offsets
    {
        public static int SceneCount => Unity.Instance.Is64Bit ? 0x18 : 0xC;
        public static int LoadedScenes => Unity.Instance.Is64Bit ? 0x28 : 0x18;
        public static int ActiveScene => Unity.Instance.Is64Bit ? 0x48 : 0x28;
        public static int AssetPath => Unity.Instance.Is64Bit ? 0x10 : 0xC;
        public static int BuildIndex => Unity.Instance.Is64Bit ? 0x98 : 0x70;
    }
}
