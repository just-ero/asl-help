using System.Collections.Generic;
using System.Diagnostics;

using AslHelp.Collections;
using AslHelp.LiveSplit.Diagnostics;
using AslHelp.Memory;
using AslHelp.Memory.Ipc;
using AslHelp.Shared;

public partial class Basic
{
    private IProcessMemory? _memory;
    protected override IProcessMemory Memory
    {
        get
        {
            if (_memory is null)
            {
                if (Game is not Process game)
                {
                    const string Msg = "Game is not connected.";
                    ThrowHelper.ThrowInvalidOperationException(Msg);

                    return null;
                }

                using (AslDebug.Indent("Initializing memory..."))
                {
                    _memory = InitializeMemory(game);

                    AslDebug.Info("Success.");
                }
            }

            return _memory;
        }
    }

    public bool Is64Bit => Memory.Is64Bit;
    public byte PtrSize => Memory.PointerSize;

    public Module MainModule => Memory.MainModule;
    public IReadOnlyKeyedCollection<string, Module> Modules => Memory.Modules;

    public IEnumerable<MemoryRange> GetMemoryPages()
    {
        return Memory.GetMemoryPages();
    }

    protected virtual IProcessMemory InitializeMemory(Process process)
    {
        return new ProcessMemory(process);
    }

    protected override void DisposeMemory()
    {
        Game?.Dispose();

        _memory?.Dispose();
        _memory = null;
    }
}
