using System.IO;
using System.Linq;
using System.Security.Cryptography;

using AslHelp.Memory;
using AslHelp.Shared;

public partial class Basic
{
    public string GetMD5Hash()
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return GetMD5Hash(MainModule);
    }

    public string GetMD5Hash(string moduleName)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return GetMD5Hash(Modules[moduleName]);
    }

    public string GetMD5Hash(Module module)
    {
        using var md5 = MD5.Create();
        return GetHash(module, md5);
    }

    public string GetSHA1Hash()
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return GetSHA1Hash(MainModule);
    }

    public string GetSHA1Hash(string moduleName)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return GetSHA1Hash(Modules[moduleName]);
    }

    public string GetSHA1Hash(Module module)
    {
        using var sha1 = SHA1.Create();
        return GetHash(module, sha1);
    }

    public string GetSHA256Hash()
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return GetSHA256Hash(MainModule);
    }

    public string GetSHA256Hash(string moduleName)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return GetSHA256Hash(Modules[moduleName]);
    }

    public string GetSHA256Hash(Module module)
    {
        using var sha256 = SHA256.Create();
        return GetHash(module, sha256);
    }

    public string GetSHA512Hash()
    {
        ThrowHelper.ThrowIfNull(MainModule);

        return GetSHA512Hash(MainModule);
    }

    public string GetSHA512Hash(string moduleName)
    {
        ThrowHelper.ThrowIfNull(Modules);

        return GetSHA512Hash(Modules[moduleName]);
    }

    public string GetSHA512Hash(Module module)
    {
        using var sha512 = SHA512.Create();
        return GetHash(module, sha512);
    }

    private static string GetHash(Module module, HashAlgorithm algorithm)
    {
        using FileStream fs = File.OpenRead(module.FileName);
        return string.Concat(algorithm.ComputeHash(fs).Select(b => $"{b:X2}"));
    }
}
