using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

using AslHelp.Collections;
using AslHelp.Memory;
using AslHelp.Memory.Extensions;
using AslHelp.Shared;

public partial class Basic
{
    private nint _handle;

    public bool Is64Bit { get; private set; }
    public byte PointerSize { get; private set; }

    public Module? MainModule { get; private set; }
    public IReadOnlyKeyedCollection<string, Module>? Modules { get; private set; }

    public IEnumerable<MemoryRange> GetMemoryPages()
    {
        ThrowHelper.ThrowIfNull(Game);

        return Game.GetMemoryPages();
    }

    protected override void InitializeMemory(Process game)
    {
        _handle = game.Handle;

        Is64Bit = game.Is64Bit().Unwrap();
        PointerSize = (byte)(Is64Bit ? 0x8 : 0x4);

        Modules = new ModuleCollection(game);
        MainModule = Modules.FirstOrDefault();
    }

    protected override void DisposeMemory()
    {
        _handle = 0;

        Is64Bit = false;
        PointerSize = 0;

        Modules = null;
        MainModule = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static bool IsNativeInt<T>()
    {
        return typeof(T) == typeof(nint) || typeof(T) == typeof(nuint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected unsafe int GetNativeSizeOf<T>()
        where T : unmanaged
    {
        return IsNativeInt<T>() ? PointerSize : sizeof(T);
    }
}
