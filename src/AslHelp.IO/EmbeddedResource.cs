using System.IO;
using System.Reflection;

using AslHelp.Shared;

namespace AslHelp.IO;

public static class EmbeddedResource
{
    public static void Extract(string resourceName, string outputPath)
    {
        using Stream source = GetResourceStream(resourceName);
        using FileStream destination = File.OpenWrite(outputPath);

        source.CopyTo(destination);
    }

    public static Stream GetResourceStream(string resourceName)
    {
        return GetResourceStreamFromAssembly(resourceName, Assembly.GetCallingAssembly());
    }

    public static Stream GetResourceStreamFromAssembly(string resourceName, Assembly assembly)
    {
        Stream? resourceStream = assembly.GetManifestResourceStream(resourceName);

        if (resourceStream is null)
        {
            string msg = $"Unable to find the specified resource '{resourceName}'.";
            ThrowHelper.ThrowFileNotFoundException(msg);
        }

        return resourceStream;
    }
}
