namespace ASLHelper.UnityHelper;

public class Il2CppHelper : MonoHelper
{
    public Il2CppHelper(Unity helper, string type, string version)
        : base(helper, type, version)
    {
        _is2020 = version == "2020";

        if (GetLoadedImages() != 0)
            return;

        _isMaster = false;
        if (GetLoadedImages() == 0)
            throw new Exception("Cannot load mono images! The signature could not be resolved!");
    }

    private readonly bool _is2020;
    private readonly bool _isMaster = true;
    private nint _loadedClasses = 0;

    #region Images
    private protected override nint ScanForImages()
    {
        return _helper.Scan(Data.s_MonoModule, _engine.Signatures["s_Assemblies"]);
    }

    private protected nint GetLoadedImages()
    {
        if (_loadedImages != 0)
            return _loadedImages;

        _loadedImages = ScanForImages();
        if (!_isMaster)
            _loadedImages = ReadRel(_loadedImages + (_helper.Is64Bit ? 0x3 : 0x1));

        return _loadedImages;
    }

    private protected override MonoImage GetImage(string imageName)
    {
        if (_imageCache.TryGetValue(imageName, out var image))
            return image;

        Debug.Log("Searching for image '" + imageName + "'...");

        while (!_helper.CancelSource.IsCancellationRequested)
        {
            var loaded_images = ReadPtr(GetLoadedImages());
            nint assembly;

            while ((assembly = ReadPtr(ReadPtr(loaded_images) + _engine["MonoAssembly"]["image"])) != 0)
            {
                if (ReadStr(ReadPtr(assembly + _engine["MonoImage"]["name"]), 64) != imageName)
                {
                    loaded_images += _helper.PtrSize;
                    continue;
                }

                var class_count = ReadI32(assembly + _engine["MonoImage"]["class_count"]);
                var table_offset =
                    !_is2020
                    ? ReadI32(assembly + _engine["MonoImage"]["table_offset"])
                    : ReadI32(ReadPtr(assembly + _engine["MonoImage"]["metadata_handle"]));

                image = new MonoImage
                {
                    Name = imageName,
                    Address = assembly,
                    ClassCount = class_count,
                    ClassCache = table_offset
                };

                Debug.Log("  => Found at 0x" + assembly.ToString("X") + ".");
                Debug.Log("    => class_count is " + class_count + ".");
                Debug.Log("    => table_offset is 0x" + table_offset.ToString("X") + ".");

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
    private nint GetLoadedClasses()
    {
        if (_loadedClasses != 0)
            return _loadedClasses;

        _loadedClasses = ReadPtr(_helper.Scan(Data.s_MonoModule, _engine.Signatures["s_TypeInfoTable"]));
        if (_loadedClasses == 0)
        {
            var msg = "Cannot load classes! The signature could not be resolved!";
            throw new Exception(msg);
        }

        return _loadedClasses;
    }

    private protected override MonoClass GetClass(MonoImage image, uint classToken, int depth = 0)
    {
        throw new NotImplementedException("Getting classes via their tokens is not supported for IL2CPP.");
    }

    private protected override MonoClass GetClass(MonoImage image, string className, int depth = 0)
    {
        string classNameSpace = null;
        var delimiter = className.LastIndexOf('.');
        if (delimiter > -1)
        {
            classNameSpace = className.Substring(0, delimiter);
            className = className.Substring(delimiter + 1);
        }

        Debug.Log("Searching for class '" + className + "'...");

        foreach (var klass in Classes(image))
        {
            var klassName = ClassName(klass);
            var klassNameSpace = ClassNameSpace(klass);

            if (klassName == className && (classNameSpace == null || klassNameSpace == classNameSpace))
            {
                var fields = GetAllFields(klass);

                var parent = klass;
                for (int i = 0; i < depth; ++i)
                {
                    parent = ClassParent(parent);
                    fields.AddRange(GetAllFields(parent));
                }

                var staticAddress = GetStaticAddress(parent);
                Debug.Log("  => Found at 0x" + klass.ToString("X") + ".");
                Debug.Log("  => Static field table at 0x" + staticAddress.ToString("X") + ".");

                foreach (var field in fields.OrderBy(f => f.Offset).Where(f => !f.IsConst))
                    Debug.Log(string.Format("    => 0x{0:X3}: {1,-6} {2}", field.Offset, field.IsStatic ? "static" : "", field.Name));

                return new MonoClass
                {
                    NameSpace = klassNameSpace,
                    Name = klassName,
                    Address = klass,
                    Static = staticAddress,
                    Fields = fields
                };
            }
        }

        Debug.Log("  => Not found!");
        return null;
    }

    private protected override IEnumerable<nint> Classes(MonoImage image)
    {
        var table = GetLoadedClasses();
        int count = image.ClassCount, offset = (int)image.ClassCache;

        for (int i = 0; i < count; ++i)
        {
            var klass = ClassFromIndex(table, offset + i);
            if (klass != 0)
                yield return klass;
        }
    }

    private protected override int ClassFieldCount(nint klass)
    {
        return ReadU16(klass + _engine["MonoClass"]["field_count"]);
    }

    private protected override nint GetStaticAddress(nint klass)
    {
        return ReadPtr(klass + _engine["MonoClass"]["data"]);
    }
    #endregion
}