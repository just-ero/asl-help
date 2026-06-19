#pragma warning disable IDE0044 // Add readonly modifier

using System;

using AslHelp.Reflection;

using NUnit.Framework;

namespace AslHelp.Tests;

[TestFixture]
public class ReflectionExtensionsTests
{
    private sealed class Sample
    {
        public int PublicField = 1;
        private string _privateField = "secret";

        public string Reveal()
        {
            return _privateField;
        }
    }

    [Test]
    public void GetFieldValue_OnPublicField_ReturnsValue()
    {
        Sample sample = new();

        Assert.That(sample.GetFieldValue<int>("PublicField"), Is.EqualTo(1));
    }

    [Test]
    public void GetFieldValue_OnPrivateField_ReturnsValue()
    {
        Sample sample = new();

        Assert.That(sample.GetFieldValue<string>("_privateField"), Is.EqualTo("secret"));
    }

    [Test]
    public void SetFieldValue_OnPrivateField_WritesValue()
    {
        Sample sample = new();

        sample.SetFieldValue("_privateField", "changed");

        Assert.That(sample.Reveal(), Is.EqualTo("changed"));
    }

    [Test]
    public void GetFieldValue_WhenFieldMissing_Throws()
    {
        Sample sample = new();

        Assert.Throws<MissingFieldException>(() => sample.GetFieldValue<int>("Nope"));
    }

    [Test]
    public void GetFieldValue_WhenTypeMismatches_Throws()
    {
        Sample sample = new();

        Assert.Throws<MissingFieldException>(() => sample.GetFieldValue<long>("PublicField"));
    }

    [Test]
    public void GetFieldValue_WithNullName_Throws()
    {
        Sample sample = new();

        Assert.Throws<ArgumentNullException>(() => sample.GetFieldValue<int>(null!));
    }

    [Test]
    public void GetFieldValue_WithNullTarget_Throws()
    {
        object? target = null;

        Assert.Throws<MissingFieldException>(() => target.GetFieldValue<int>("PublicField"));
    }

    [Test]
    public void SetFieldValue_WhenFieldMissing_Throws()
    {
        Sample sample = new();

        Assert.Throws<MissingFieldException>(() => sample.SetFieldValue("Nope", 1));
    }

    [Test]
    public void SetFieldValue_WithNullName_Throws()
    {
        Sample sample = new();

        Assert.Throws<ArgumentNullException>(() => sample.SetFieldValue(null!, 1));
    }

    [Test]
    public void SetFieldValue_WithNullTarget_Throws()
    {
        object? target = null;

        Assert.Throws<MissingFieldException>(() => target.SetFieldValue("PublicField", 1));
    }
}

[TestFixture]
public class UnsafeAccessorTests
{
    private sealed class Sample
    {
        public int PublicField = 1;
        private int _privateField = 7;

        public int Reveal()
        {
            return _privateField;
        }
    }

    [Test]
    public void CreateFieldGetter_OnPublicField_ReadsValue()
    {
        Sample sample = new();
        var getter = UnsafeAccessor.CreateFieldGetter<Sample, int>("PublicField");

        Assert.That(getter(sample), Is.EqualTo(1));
    }

    [Test]
    public void CreateFieldGetter_OnPrivateField_ReadsValue()
    {
        Sample sample = new();
        var getter = UnsafeAccessor.CreateFieldGetter<Sample, int>("_privateField");

        Assert.That(getter(sample), Is.EqualTo(7));
    }

    [Test]
    public void CreateFieldSetter_OnPrivateField_WritesValue()
    {
        Sample sample = new();
        var setter = UnsafeAccessor.CreateFieldSetter<Sample, int>("_privateField");

        setter(sample, 99);

        Assert.That(sample.Reveal(), Is.EqualTo(99));
    }

    [Test]
    public void CreateFieldGetter_WhenFieldMissing_Throws()
    {
        Assert.Throws<MissingFieldException>(
            () => UnsafeAccessor.CreateFieldGetter<Sample, int>("Nope"));
    }

    [Test]
    public void CreateFieldSetter_WhenTypeMismatches_Throws()
    {
        Assert.Throws<MissingFieldException>(
            () => UnsafeAccessor.CreateFieldSetter<Sample, string>("PublicField"));
    }

    [Test]
    public void CreateFieldSetter_WhenFieldMissing_Throws()
    {
        Assert.Throws<MissingFieldException>(
            () => UnsafeAccessor.CreateFieldSetter<Sample, int>("Nope"));
    }
}
