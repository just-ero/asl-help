using System.IO;
using System.Linq;
using System.Security.Cryptography;

using AslHelp.Memory;

namespace AslHelp.LiveSplit;

public partial class AslPluginBase
{
    public string GetMD5Hash()
    {
        return GetMD5Hash(Memory.MainModule);
    }

    public string GetMD5Hash(string moduleName)
    {
        return GetMD5Hash(Memory.Modules[moduleName]);
    }

    public string GetMD5Hash(Module module)
    {
        using var md5 = MD5.Create();
        return GetHash(module, md5);
    }

    public string GetSHA1Hash()
    {
        return GetSHA1Hash(Memory.MainModule);
    }

    public string GetSHA1Hash(string moduleName)
    {
        return GetSHA1Hash(Memory.Modules[moduleName]);
    }

    public string GetSHA1Hash(Module module)
    {
        using var sha1 = SHA1.Create();
        return GetHash(module, sha1);
    }

    public string GetSHA256Hash()
    {
        return GetSHA256Hash(Memory.MainModule);
    }

    public string GetSHA256Hash(string moduleName)
    {
        return GetSHA256Hash(Memory.Modules[moduleName]);
    }

    public string GetSHA256Hash(Module module)
    {
        using var sha256 = SHA256.Create();
        return GetHash(module, sha256);
    }

    public string GetSHA512Hash()
    {
        return GetSHA512Hash(Memory.MainModule);
    }

    public string GetSHA512Hash(string moduleName)
    {
        return GetSHA512Hash(Memory.Modules[moduleName]);
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
