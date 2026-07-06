using AslHelp.WikiGen.Api;

using NUnit.Framework;

namespace AslHelp.WikiGen.Tests;

[TestFixture]
public class MemberNamingTests
{
    [Test]
    public void For_SimpleName_FileAndDisplayAreTheName()
    {
        var (file, display) = MemberNaming.For("IsOk", MemberGroup.Methods);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(file, Is.EqualTo("IsOk"));
            Assert.That(display, Is.EqualTo("IsOk"));
        }
    }

    [Test]
    public void For_GenericName_CutsAtAngleBracket()
    {
        var (file, display) = MemberNaming.For("Map<TValue>", MemberGroup.Methods);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(file, Is.EqualTo("Map"));
            Assert.That(display, Is.EqualTo("Map"));
        }
    }

    [Test]
    public void For_NameWithParameters_CutsAtParenthesis()
    {
        var (file, display) = MemberNaming.For("Item(int)", MemberGroup.Methods);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(file, Is.EqualTo("Item"));
            Assert.That(display, Is.EqualTo("Item"));
        }
    }

    [Test]
    public void For_NameWithSpecialChars_SanitizesFileButKeepsDisplay()
    {
        // Characters other than letters, digits, '_' and '-' become '_' in the file name only.
        var (file, display) = MemberNaming.For("a+b", MemberGroup.Methods);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(file, Is.EqualTo("a_b"));
            Assert.That(display, Is.EqualTo("a+b"));
        }
    }

    [Test]
    public void For_Operator_DefaultsToImplicit()
    {
        var (file, display) = MemberNaming.For("op_Implicit", MemberGroup.Operators);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(file, Is.EqualTo("op_Implicit"));
            Assert.That(display, Is.EqualTo("implicit operator"));
        }
    }

    [Test]
    public void For_Operator_NameContainingExplicit_IsExplicit()
    {
        // The branch keys off the ordinal substring "explicit".
        var (file, display) = MemberNaming.For("op_explicit", MemberGroup.Operators);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(file, Is.EqualTo("op_Explicit"));
            Assert.That(display, Is.EqualTo("explicit operator"));
        }
    }
}
