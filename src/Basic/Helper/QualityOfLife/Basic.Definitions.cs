using AslHelp.MemUtils.Definitions;

public partial class Basic
{
    public TypeDefinition Define(string code, params string[] references)
    {
        return TypeDefinitionFactory.CreateFromSource(code, references);
    }
}
