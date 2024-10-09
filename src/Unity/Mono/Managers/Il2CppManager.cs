using AslHelp.MemUtils.Exceptions;
using AslHelp.MemUtils.SigScan;
using AslHelp.Mono.Models;

namespace AslHelp.Mono.Managers;

internal class Il2CppManager : UnityMemManager
{
    private readonly Signature _assembliesTrg =
        Unity.Instance.Is64Bit
        ? new(12, "48 FF C5 80 3C ?? 00 75 ?? 48 8B 1D")
        : new(9, "8A 07 47 84 C0 75 ?? 8B 35");

    private readonly Signature _typeInfoDefinitionTableTrg =
        Unity.Instance.Is64Bit
        ? new(18, "48 C1 E9 0? BA 08 00 00 00")
        : new(2, "C3 A1 ???????? 83 3C ?? 00");

    private readonly bool _is2020;
    private nint _typeInfoDefinitionTable;

    public Il2CppManager(string version)
        : base("il2cpp", version)
    {
        _is2020 = version is "2020" or "29";
    }

    internal override nint FindAssemblies()
    {
        nint assemblies = ReadPtr(_game.ScanRel(_assembliesTrg, _game.MonoModule));

        if (assemblies == 0)
        {
            throw new FatalNotFoundException("Could not find assemblies table!");
        }

        return assemblies;
    }

    internal override IEnumerable<nint> EnumerateImages()
    {
        nint assemblies = FindAssemblies(), image;

        while ((image = AssemblyImage(ReadPtr(assemblies))) != 0)
        {
            yield return image;
            assemblies += _game.PtrSize;
        }
    }

    internal override string ImageName(nint image)
    {
        return ReadStr(image + _engine["MonoImage"]["nameNoExt"], 128);
    }

    internal override int ImageCacheSize(nint image)
    {
        return ReadI32(image + _engine["MonoImage"]["typeCount"]);
    }

    internal override int ImageCacheEntries(nint image)
    {
        return ReadI32(image + _engine["MonoImage"]["typeCount"]);
    }

    internal override nint ImageCacheTable(nint image)
    {
        if (_is2020)
        {
            return ReadI32(ReadPtr(image + _engine["MonoImage"]["metadataHandle"]));
        }
        else
        {
            return ReadI32(image + _engine["MonoImage"]["typeStart"]);
        }
    }

    internal nint FindTypeInfoDefinitionTable()
    {
        if (_typeInfoDefinitionTable != 0)
        {
            return _typeInfoDefinitionTable;
        }

        _typeInfoDefinitionTable = ReadPtr(_game.ScanRel(_typeInfoDefinitionTableTrg, _game.MonoModule));

        if (_typeInfoDefinitionTable == 0)
        {
            throw new FatalNotFoundException("Could not find classes table!");
        }

        return _typeInfoDefinitionTable;
    }

    internal override IEnumerable<nint> EnumerateClasses(nint cacheTable, int cacheSize)
    {
        nint table = FindTypeInfoDefinitionTable();
        int offset = (int)cacheTable;

        nint[] classes = new nint[cacheSize];

        if (!_game.TryReadArray_Internal<nint>(classes, table + (_game.PtrSize * offset)))
        {
            yield break;
        }

        for (int i = 0; i < cacheSize; i++)
        {
            if (classes[i] != 0)
            {
                yield return classes[i];
            }
        }
    }

    public override MonoClass GetClass(MonoImage image, uint classToken, int parent = 0)
    {
        string msg = "Retrieving a class via its token is not supported on IL2CPP";
        throw new NotSupportedException(msg);
    }

    internal override nint ClassStaticFields(nint klass)
    {
        return ReadPtr(klass + _engine["MonoClass"]["static_fields"]);
    }

    internal override int ClassFieldCount(nint klass)
    {
        return ReadU16(klass + _engine["MonoClass"]["field_count"]);
    }
}
