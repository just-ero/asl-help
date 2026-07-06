using AslHelp.WikiGen.Api;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class TypeRefTests
{
    [Test]
    public void TypeRef_StripsTheNamespacePrefix()
    {
        Assert.That(ApiModelBuilder.TypeRef("System.String", "System"), Is.EqualTo("String"));
    }

    [Test]
    public void TypeRef_ConvertsSingleGenericArityToHyphen()
    {
        Assert.That(ApiModelBuilder.TypeRef("AslHelp.Result`1", "AslHelp"), Is.EqualTo("Result-1"));
    }

    [Test]
    public void TypeRef_ConvertsMultiGenericArityToHyphen()
    {
        Assert.That(ApiModelBuilder.TypeRef("AslHelp.Logging.Logger`2", "AslHelp.Logging"), Is.EqualTo("Logger-2"));
    }

    [Test]
    public void TypeRef_UidNotUnderNamespace_IsLeftWhole()
    {
        Assert.That(ApiModelBuilder.TypeRef("Other.Thing", "AslHelp"), Is.EqualTo("Other.Thing"));
    }
}
