using ASLHelper.UnityHelper;
using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;

namespace ASLHelper
{
    public partial class Unity
    {
        private bool _loadSceneManager;
        public bool LoadSceneManager
        {
            get => _loadSceneManager;
            set
            {
                if (Game != null)
                {
                    var msg = $"{nameof(LoadSceneManager)} must be set before entering the 'init {{}}' action.";
                    throw new InvalidOperationException(msg);
                }

                Debug.Log(value ? "  => Will try to load SceneManager." : "Will not load SceneManager.");
                _loadSceneManager = value;
            }
        }

        public SceneHelper Scenes { get; private set; }
    }

    namespace UnityHelper
    {
        public class SceneHelper
        {
            public SceneHelper()
            {
                SceneOffsets = Offsets;
                Active = new Scene(SceneOffsets[2]);
            }

            #region Properties
            public Scene Active { get; }
            public List<Scene> Loading => UpdateList(SceneOffsets[1]);
            public List<Scene> Loaded => UpdateList(SceneOffsets[3]);
            public int Count => Data.s_Helper.Game.ReadValue<int>(Data.s_Helper.Deref(Data.s_SceneManager, SceneOffsets[0]), -1);

            internal static int[] SceneOffsets;
            public int[] Offsets
            {
                get
                {
                    return SceneOffsets ?? (Data.s_Helper.Is64Bit
                           ? new[] { 0x18, 0x28, 0x48, 0x50, 0x10, 0x98 }
                           : new[] { 0x0C, 0x18, 0x28, 0x2C, 0x0C, 0x70 });
                }
                set
                {
                    if (value == null)
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
            #endregion

            private List<Scene> UpdateList(int offset)
            {
                var scenes = new List<Scene>();
                for (int i = 0; i < 64; i++)
                {
                    var scene = new Scene(offset, Data.s_Helper.PtrSize * i);
                    if (!scene.IsValid)
                        break;

                    scenes.Add(scene);
                }

                return scenes;
            }
        }
    }
}