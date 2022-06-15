namespace ASLHelper.UnityHelper;

public class SceneHelper
{
    internal SceneHelper()
    {
        SceneOffsets = Offsets;
        Active = new(SceneOffsets[2]);
    }

    public Scene Active { get; }
    public List<Scene> Loading
    {
        get => UpdateList(SceneOffsets[1]);
    }

    public List<Scene> Loaded
    {
        get => UpdateList(SceneOffsets[3]);
    }

    public int Count
    {
        get => Unity.Instance.TryRead<int>(out var count, SceneManager, SceneOffsets[0]) ? count : -1;
    }

    internal static int[] SceneOffsets;
    public int[] Offsets
    {
        get
        {
            return SceneOffsets ?? (Unity.Instance.Is64Bit
                   ? new[] { 0x18, 0x28, 0x48, 0x50, 0x10, 0x98 }
                   : new[] { 0x0C, 0x18, 0x28, 0x2C, 0x0C, 0x70 });
        }
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value), "The Scenes.Offsets array must not be null!");

            if (value.Length != 6)
            {
                throw new ArgumentException(
                    "The offsets provided did not match the expected format!" + Environment.NewLine +
                    "This array takes 6 offsets to the following variables in SceneManager (in this order):" + Environment.NewLine +
                    "{ SceneManager.SceneCount, SceneManager.Loading, SceneManager.ActiveScene," +
                    " SceneManager.Loaded, Scene.FilePath, Scene.BuildIndex }", "Scenes.Offsets");
            }

            SceneOffsets = value;
        }
    }

    private List<Scene> UpdateList(int offset)
    {
        var scenes = new List<Scene>();
        for (int i = 0; i < 64; i++)
        {
            var scene = new Scene(offset, Unity.Instance.PtrSize * i);
            if (!scene.IsValid)
                break;

            scenes.Add(scene);
        }

        return scenes;
    }

    private static nint g_runtimeSceneManager;
    public static nint SceneManager
    {
        get
        {
            if (g_runtimeSceneManager != 0)
                return g_runtimeSceneManager;

            Debug.Log("Looking for g_runtimeSceneManager...");

            var target = new SigScanTarget { OnFound = Data.s_OnFound };

            if (Unity.Instance.Is64Bit)
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

            var scan = Unity.Instance.Scan(Unity.Instance.UnityPlayer, target);
            if (scan != 0)
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
