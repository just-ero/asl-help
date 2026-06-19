using System;

using NUnit.Framework;

namespace AslHelp.Tests;

[TestFixture]
public class ResultErrorTests
{
    [Test]
    public void ToString_WithMessage_FormatsTypeAndMessage()
    {
        ResultError error = new("boom");

        Assert.That(error.ToString(), Is.EqualTo("ResultError: boom"));
    }
}

[TestFixture]
public class ExceptionErrorTests
{
    [Test]
    public void Constructor_FromException_CapturesExceptionAndMessage()
    {
        InvalidOperationException ex = new("kaboom");

        ExceptionError error = new(ex);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(error.Exception, Is.SameAs(ex));
            Assert.That(error.Message, Is.EqualTo("kaboom"));
        }
    }

    [Test]
    public void Constructor_WithCustomMessage_OverridesExceptionMessage()
    {
        InvalidOperationException ex = new("kaboom");

        ExceptionError error = new(ex, "friendlier");

        Assert.That(error.Message, Is.EqualTo("friendlier"));
    }

    [Test]
    public void Constructor_FromNullException_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new ExceptionError(null!));
    }

    [Test]
    public void ToString_WithException_FormatsExceptionTypeAndMessage()
    {
        ExceptionError error = new(new InvalidOperationException("kaboom"));

        Assert.That(error.ToString(), Is.EqualTo("InvalidOperationException: kaboom"));
    }
}
