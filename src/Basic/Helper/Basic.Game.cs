using AslHelp.Collections;
using AslHelp.MemUtils;

public partial class Basic
{
    private Process _game;
    public Process Game
    {
        get
        {
            _game?.Refresh();

            if (_game is null || _game.HasExited)
            {
                _game = ScriptGame;

                UpdateProcData(_game);
            }

            return _game;
        }
        set
        {
            _game = value;
            ScriptGame = value;

            UpdateProcData(value);
        }
    }

    public bool Is64Bit { get; private set; } = true;
    public byte PtrSize { get; private set; }
    public Module MainModule => Modules.FirstOrDefault();

    public ModuleCache Modules { get; } = new();
    public IEnumerable<MemoryPage> Pages => Game.MemoryPages(Is64Bit);

    private void UpdateProcData(Process proc)
    {
        if (proc is null)
        {
            Is64Bit = false;
            PtrSize = 0;
        }
        else
        {
            Is64Bit = _game.Is64Bit();
            PtrSize = (byte)(Is64Bit ? 0x8 : 0x4);
        }

        Modules.Clear();
    }
}
