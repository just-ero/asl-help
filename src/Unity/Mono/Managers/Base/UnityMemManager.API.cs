using AslHelp.MemUtils.Exceptions;
using AslHelp.Mono.Models;

namespace AslHelp.Mono.Managers;

public abstract partial class UnityMemManager
{
    public ImageCache Images { get; } = new();

    public abstract MonoClass GetClass(MonoImage image, uint classToken, int parent = 0);

    public MonoImage GetImage(string imageName)
    {
        return Images[imageName];
    }

    public MonoClass GetClass(string className, int parent = 0)
    {
        return GetClass("Assembly-CSharp", className, parent);
    }

    public MonoClass GetClass(string imageName, string className, int parent = 0)
    {
        return GetClass(GetImage(imageName), className, parent);
    }

    public MonoClass GetClass(MonoImage image, string className, int parent = 0)
    {
        if (image.TryGetValue(className, out MonoClass klass))
        {
            return GetClass(klass.Address, parent);
        }
        else
        {
            string msg = $"Class '{className}' could not be found.";
            throw new NotFoundException(msg);
        }
    }

    public MonoClass GetClass(uint classToken, int parent = 0)
    {
        return GetClass("Assembly-CSharp", classToken, parent);
    }

    public MonoClass GetClass(string imageName, uint classToken, int parent = 0)
    {
        return GetClass(GetImage(imageName), classToken, parent);
    }

    public MonoClass this[string className, int parents = 0]
        => GetClass("Assembly-CSharp", className, parents);

    public MonoClass this[string imageName, string className, int parents = 0]
        => GetClass(imageName, className, parents);
}
