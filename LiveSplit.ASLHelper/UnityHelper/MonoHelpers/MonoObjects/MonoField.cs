namespace ASLHelper.UnityHelper
{
    public class MonoField
    {
        public string Name { get; internal set; }
        public int Offset { get; internal set; }
        public bool IsConst { get; internal set; }
        public bool IsStatic { get; internal set; }
    }
}