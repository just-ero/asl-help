namespace ASLHelper.UnityHelper;

public class MonoV1Helper : MonoHelper
{
    public MonoV1Helper(Unity helper, string type, string version)
        : base(helper, type, version)
    {
        if (version != "v1")
            return;

        var asmCs = GetImage("Assembly-CSharp");
        foreach (var klass in Classes(asmCs))
        {
            var image = klass + _engine["MonoClass"]["image"];

            if (ReadPtr(image) == asmCs.Address)
            {
                break;
            }
            else if (ReadPtr(image + _helper.PtrSize) == asmCs.Address)
            {
                Debug.Log("  => Loading cattrs...");
                _engine = EngineReflection.Load("Unity", "mono", "v1_cattrs");
                break;
            }
        }
    }

    private protected override nint ScanForImages()
    {
        return _helper.Scan(Unity.Instance.MonoModule, _engine.Signatures["loaded_images_hashes"]);
    }

    #region Images
    private protected nint GetLoadedImages()
    {
        if (_loadedImages != 0)
            return _loadedImages;

        _loadedImages = ReadPtr(ScanForImages());
        if (_loadedImages == 0)
        {
            var msg = "Cannot load mono images! The scan target could not be resolved!";
            throw new Exception(msg);
        }

        return _loadedImages;
    }

    private protected override MonoImage GetImage(string imageName)
    {
        if (_imageCache.TryGetValue(imageName, out var image))
            return image;

        Debug.Log("Searching for image '" + imageName + "'...");

        uint hash = 0;
        foreach (var c in (imageName + '\0').Substring(1))
            hash = (hash << 5) - (hash + c);

        while (!_helper.CancelSource.IsCancellationRequested)
        {
            var table_size = ReadI32(GetLoadedImages() + _engine["GHashTable"]["table_size"]);
            var slot = ReadPtr(ReadPtr(GetLoadedImages() + _engine["GHashTable"]["table"]) + (_helper.PtrSize * (int)(hash % table_size)));

            for (; slot != 0; slot = ReadPtr(slot + _engine["Slot"]["next"]))
            {
                if (ReadStr(ReadPtr(slot + _engine["Slot"]["key"]), 64) != imageName)
                    continue;

                slot = ReadPtr(slot + _engine["Slot"]["value"]);
                var class_cache_size = ReadI32(slot + _engine["MonoImage"]["class_cache_size"]);
                var class_cache_table = ReadPtr(slot + _engine["MonoImage"]["class_cache_table"]);
                if (class_cache_size == 0 || class_cache_table == 0)
                    continue;

                image = new MonoImage
                {
                    Name = imageName,
                    Address = slot,
                    ClassCount = class_cache_size,
                    ClassCache = class_cache_table
                };

                Debug.Log("  => Found at 0x" + slot.ToString("X") + ".");
                Debug.Log("    => class_cache.size is " + image.ClassCount + ".");
                Debug.Log("    => class_cache.table at 0x" + image.ClassCache.ToString("X") + ".");

                _imageCache[imageName] = image;
                return image;
            }

            if (_helper.CancelSource.IsCancellationRequested)
                break;

            Debug.Log("  => Not found! Retrying in 5 seconds...");
            Task.Delay(5000).Wait(_helper.CancelSource.Token);
        }

        return null;
    }
    #endregion

    #region Classes
    private protected override MonoClass GetClass(MonoImage image, uint classToken, int depth = 0)
    {
        Debug.Log("Searching for class with token 0x" + classToken.ToString("X") + "...");

        var klass = ClassFromIndex(image.ClassCache, (int)(classToken % image.ClassCount));
        for (; klass != 0; klass = NextClass(klass))
        {
            if (ReadU32(klass + _engine["MonoClass"]["type_token"]) != classToken)
                continue;

            return CreateMonoClass(klass, depth);
        }

        Debug.Log("  => Not found!");

        return null;
    }

    private protected override IEnumerable<nint> Classes(MonoImage image)
    {
        for (int i = 0; i < image.ClassCount; ++i)
        {
            var klass = ClassFromIndex(image.ClassCache, i);
            for (; klass != 0; klass = NextClass(klass))
                yield return klass;
        }
    }

    private protected nint ClassVTable(nint klass)
    {
        var runtime_info = ReadPtr(klass + _engine["MonoClass"]["runtime_info"]);
        return ReadPtr(runtime_info + _engine["MonoClassRuntimeInfo"]["domain_vtables"]);
    }

    private protected nint NextClass(nint klass)
    {
        return ReadPtr(klass + _engine["MonoClass"]["next"]);
    }

    private protected override int ClassFieldCount(nint klass)
    {
        return ReadI32(klass + _engine["MonoClass"]["field_count"]);
    }

    private protected override nint GetStaticAddress(nint klass)
    {
        return ReadPtr(ClassVTable(klass) + _engine["MonoVTable"]["data"]);
    }
    #endregion
}
