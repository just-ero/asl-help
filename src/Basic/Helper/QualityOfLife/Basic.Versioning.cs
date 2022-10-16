using System.Security.Cryptography;

public partial class Basic
{
    public int GetMemorySize()
    {
        return GetMemorySize(MainModule);
    }

    public int GetMemorySize(string module)
    {
        return GetMemorySize(Modules[module]);
    }

    public int GetMemorySize(Module module)
    {
        if (module is null)
        {
            Debug.Warn("[GetHash] Module could not be found.");
            return 0;
        }

        return module.MemorySize;
    }

    public string GetMD5Hash()
    {
        return GetMD5Hash(MainModule);
    }

    public string GetMD5Hash(string module)
    {
        return GetMD5Hash(Modules[module]);
    }

    public string GetMD5Hash(Module module)
    {
        using MD5 md5 = MD5.Create();
        return GetHash(module, md5);
    }

    public string GetSHA1Hash()
    {
        return GetSHA1Hash(MainModule);
    }

    public string GetSHA1Hash(string module)
    {
        return GetSHA1Hash(Modules[module]);
    }

    public string GetSHA1Hash(Module module)
    {
        using SHA1 sha1 = SHA1.Create();
        return GetHash(module, sha1);
    }

    public string GetSHA256Hash()
    {
        return GetSHA256Hash(MainModule);
    }

    public string GetSHA256Hash(string module)
    {
        return GetSHA256Hash(Modules[module]);
    }

    public string GetSHA256Hash(Module module)
    {
        using SHA256 sha256 = SHA256.Create();
        return GetHash(module, sha256);
    }

    public string GetSHA384Hash()
    {
        return GetSHA384Hash(MainModule);
    }

    public string GetSHA384Hash(string module)
    {
        return GetSHA384Hash(Modules[module]);
    }

    public string GetSHA384Hash(Module module)
    {
        using SHA384 sha384 = SHA384.Create();
        return GetHash(module, sha384);
    }

    public string GetSHA512Hash()
    {
        return GetSHA512Hash(MainModule);
    }

    public string GetSHA512Hash(string module)
    {
        return GetSHA512Hash(Modules[module]);
    }

    public string GetSHA512Hash(Module module)
    {
        using SHA512 sha512 = SHA512.Create();
        return GetHash(module, sha512);
    }

    private string GetHash(Module module, HashAlgorithm algorithm)
    {
        if (module is null)
        {
            Debug.Warn("[GetHash] Module could not be found.");
            return null;
        }

        using FileStream reader = File.OpenRead(module.FilePath);
        return string.Concat(algorithm.ComputeHash(reader).Select(b => $"{b:X2}"));
    }
}
