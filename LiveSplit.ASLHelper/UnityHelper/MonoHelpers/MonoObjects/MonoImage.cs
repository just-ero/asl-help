namespace ASLHelper.UnityHelper;

public class MonoImage
{
    public string Name { get; internal set; }
    public IntPtr Address { get; internal set; }
    public int ClassCount { get; internal set; }
    public IntPtr ClassCache { get; internal set; }
}