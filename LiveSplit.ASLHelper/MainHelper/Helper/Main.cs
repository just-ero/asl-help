using ASLHelper.Extensions;
using ASLHelper.MainHelper;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace ASLHelper
{
    public partial class Main : IDisposable
    {
        public Main(LiveSplitState state, object settings, object compiledScript)
        {
            Data.s_State = state;
            Data.s_Layout = state.Layout;
            Data.s_Components = state.Layout.Components;
            Data.s_LayoutComponents = state.Layout.LayoutComponents;

            Timer = new TimerHelper(state);
            Settings = new SettingsHelper(settings);
            Texts = new TextComponentHelper();

            _form = state.Form;
            _script =
                state.Layout.Components.Append(state.Run.AutoSplitter?.Component).Cast<dynamic>()
                .FirstOrDefault(c =>
                    c.ComponentName == "Scriptable Auto Splitter"
                    && ((c.Script as object).GetFieldValue("_methods") as IEnumerable<object>)
                       .FirstOrDefault()?.GetFieldValue("_compiled_code").Equals(compiledScript)
                )?.Script;

            Data.s_Helper = this;

            Debug.Log("Created ASL helper.");
        }

        #region Fields
        private Process _game;
        internal bool Is64Bit;
        internal int PtrSize;

        private readonly Form _form;
        private readonly object _script;
        #endregion

        #region Properties
        public Process Game
        {
            get
            {
                if (_game == null)
                {
                    _game = _script.GetFieldValue("_game");
                    Is64Bit = _game.Is64Bit();
                    PtrSize = Is64Bit ? 0x8 : 0x4;
                }

                return _game;
            }
            set
            {
                _game = value;
                Is64Bit = _game.Is64Bit();
                PtrSize = Is64Bit ? 0x8 : 0x4;

                _script.SetFieldValue("_game", value);
            }
        }

        public TimerHelper Timer { get; }
        public SettingsHelper Settings { get; }
        public TextComponentHelper Texts { get; }
        #endregion

        protected bool TryGetModule(out ProcessModuleWow64Safe module, params string[] names)
        {
            module = null;

            if (Game == null)
                return false;

            var modules = Game.ModulesWow64Safe();
            if (modules == null || modules.Length == 0)
                return false;

            module = modules.FirstOrDefault(m => names.Any(n => n.Equals(m?.ModuleName ?? "", StringComparison.OrdinalIgnoreCase)));
            return true;
        }

        public ProcessModuleWow64Safe GetModule(params string[] names)
        {
            if (names == null || names.Length == 0)
                return null;

            return Game?.ModulesWow64Safe()?.FirstOrDefault(m => names.Any(n => n.Equals(m?.ModuleName ?? "", StringComparison.OrdinalIgnoreCase)));
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);

            var closing = Debug.TraceIncludes("TimerForm_FormClosing", "OpenLayoutFromFile", "LoadDefaultLayout");
            if (!closing)
                Texts.RemoveAll();

            Data.Dispose();
        }
    }
}