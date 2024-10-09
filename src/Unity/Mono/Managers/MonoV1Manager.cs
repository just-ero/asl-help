using AslHelp.Data;
using AslHelp.MemUtils.Exceptions;
using AslHelp.MemUtils.Reflect;
using AslHelp.MemUtils.SigScan;
using AslHelp.Mono.Models;

namespace AslHelp.Mono.Managers;

internal class MonoV1Manager : UnityMemManager
{
    private ScanTarget _assembliesTrg
    {
        get
        {
            ScanTarget trg = new() { OnFound = Memory.OnFound };

            if (_game.Is64Bit)
            {
                trg.Add(3, "48 8B 0D");
            }
            else
            {
                trg.Add(2, "FF 35");
                trg.Add(2, "8B 0D");
            }

            return trg;
        }
    }

    private protected readonly Dictionary<uint, MonoClass> _classTokenCache = new();

    public MonoV1Manager(string version)
        : base("mono", version)
    {
        if (version != "v1")
        {
            return;
        }

        MonoImage asmCs = Images["Assembly-CSharp"];

        foreach (MonoClass klass in asmCs)
        {
            nint image = klass.Address + _engine["MonoClass"]["image"];

            if (ReadPtr(image) == asmCs.Address)
            {
                return;
            }
            else if (ReadPtr(image + _game.PtrSize) == asmCs.Address)
            {
                _engine = EngineReflection.Load("Unity", "mono", "v1_cattrs");
                return;
            }
        }

        throw new Exception("Could not load Mono V1!");
    }

    internal override nint FindAssemblies()
    {
        nint mono_assembly_foreach = _game.MonoModule.Symbols["mono_assembly_foreach"].Address;
        if (mono_assembly_foreach == 0)
        {
            throw new FatalNotFoundException("Could not find debug symbol 'mono_assembly_foreach'!");
        }

        nint assemblies = ReadPtr(_game.Scan(_assembliesTrg, mono_assembly_foreach, 0x100));
        if (assemblies == 0)
        {
            throw new FatalNotFoundException("Could not find assemblies table!");
        }

        return assemblies;
    }

    internal override IEnumerable<nint> EnumerateImages()
    {
        nint assemblies = FindAssemblies();

        while (assemblies != 0)
        {
            yield return AssemblyImage(GListData(assemblies));
            assemblies = GListNext(assemblies);
        }
    }

    internal nint GListData(nint gList)
    {
        return ReadPtr(gList + _engine["GList"]["data"]);
    }

    internal nint GListNext(nint gList)
    {
        return ReadPtr(gList + _engine["GList"]["next"]);
    }

    internal override string ImageName(nint image)
    {
        return ReadStr(image + _engine["MonoImage"]["assembly_name"], 128);
    }

    internal override int ImageCacheSize(nint image)
    {
        nint class_cache = image + _engine["MonoImage"]["class_cache"];
        return ReadI32(class_cache + _engine["MonoInternalHashTable"]["size"]);
    }

    internal override int ImageCacheEntries(nint image)
    {
        nint class_cache = image + _engine["MonoImage"]["class_cache"];
        return ReadI32(class_cache + _engine["MonoInternalHashTable"]["num_entries"]);
    }

    internal override nint ImageCacheTable(nint image)
    {
        nint class_cache = image + _engine["MonoImage"]["class_cache"];
        return ReadPtr(class_cache + _engine["MonoInternalHashTable"]["table"]);
    }

    internal override IEnumerable<nint> EnumerateClasses(nint cacheTable, int cacheSize)
    {
        nint[] classes = new nint[cacheSize];

        if (!_game.TryReadArray_Internal<nint>(classes, cacheTable))
        {
            yield break;
        }

        for (int i = 0; i < classes.Length; i++)
        {
            for (nint klass = classes[i]; klass != 0; klass = ClassNextClassCache(klass))
            {
                yield return klass;
            }
        }
    }

    public override MonoClass GetClass(MonoImage image, uint classToken, int parent = 0)
    {
        if (_classTokenCache.TryGetValue(classToken, out MonoClass monoClass))
        {
            return monoClass;
        }

        Debug.Info($"Searching for class with token 0x{classToken:X}...");

        nint klass = ClassFromIndex(image.CacheTable, (int)(classToken % image.CacheSize));
        for (; klass != 0; klass = ClassNextClassCache(klass))
        {
            if (ReadU32(klass + _engine["MonoClass"]["type_token"]) != classToken)
            {
                continue;
            }

            monoClass = GetClass(klass, parent);
            _classTokenCache[classToken] = monoClass;

            return monoClass;
        }

        string msg = $"Class with token 0x{classToken} could not be found!";
        throw new NotFoundException(msg);
    }

    internal nint ClassVTable(nint klass)
    {
        nint runtime_info = ReadPtr(klass + _engine["MonoClass"]["runtime_info"]);
        return ReadPtr(runtime_info + _engine["MonoClassRuntimeInfo"]["domain_vtables"]);
    }

    internal virtual nint ClassNextClassCache(nint klass)
    {
        return ReadPtr(klass + _engine["MonoClass"]["next_class_cache"]);
    }

    internal override nint ClassStaticFields(nint klass)
    {
        return ReadPtr(ClassVTable(klass) + _engine["MonoVTable"]["data"]);
    }

    internal override int ClassFieldCount(nint klass)
    {
        return ReadI32(klass + _engine["MonoClass"]["field.count"]);
    }
}
