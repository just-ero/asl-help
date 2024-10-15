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

    // TODO: Use semi-auto property in 9.0.
#pragma warning disable IDE0032 // Use auto property
    private Module? _mainModule;
#pragma warning restore IDE0032
    public Module? MainModule
    {
        get
        {
            if (_mainModule is null)
            {
                _ = Game;
            }

            return _mainModule;
        }
    }

    // TODO: Use semi-auto property in 9.0.
#pragma warning disable IDE0032 // Use auto property
    private IReadOnlyKeyedCollection<string, Module>? _modules;
#pragma warning restore IDE0032
    public IReadOnlyKeyedCollection<string, Module>? Modules
    {
        get
        {
            if (_modules is null)
            {
                _ = Game;
            }

            return _modules;
        }
    }

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

        _modules = new ModuleCollection(game);
        _mainModule = Modules.FirstOrDefault();
    }

    protected override void DisposeMemory()
    {
        _handle = 0;

        Is64Bit = false;
        PointerSize = 0;

        _modules = null;
        _mainModule = null;
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
