using System;

using AslHelp;

using NUnit.Framework;

namespace AslHelp.Tests;

[TestFixture]
public class ResultOfTTests
{
    private static Result<int> Ok(int value)
    {
        return Result.Ok(value);
    }

    private static Result<int> Err(string message)
    {
        return Result.Err<int>(message);
    }

    [Test]
    public void Ok_WithValue_IsOkAndCarriesValue()
    {
        Result<int> result = Ok(42);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsOk, Is.True);
            Assert.That(result.Value, Is.EqualTo(42));
            Assert.That(result.Error, Is.Null);
            Assert.That(result.ToString(), Does.Contain("42"));
        }
    }

    [Test]
    public void Err_WithMessage_IsErr()
    {
        Result<int> result = Err("boom");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsErr, Is.True);
            Assert.That(result.Error!.Message, Is.EqualTo("boom"));
            Assert.That(result.ToString(), Does.Contain("boom"));
        }
    }

    [Test]
    public void Err_WithNullError_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _ = Result.Err<int>((IResultError)null!));
    }

    [Test]
    public void Default_Always_IsOkWithDefaultValue()
    {
        Result<int> result = default;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsOk, Is.True);
            Assert.That(result.Value, Is.Zero);
        }
    }

    [Test]
    public void ImplicitConversion_FromValue_IsOk()
    {
        Result<int> result = 42;

        Assert.That(result.Unwrap(), Is.EqualTo(42));
    }

    [Test]
    public void ImplicitConversion_FromException_WrapsInExceptionError()
    {
        Result<int> result = new InvalidOperationException("boom");

        Assert.That(result.Error, Is.InstanceOf<ExceptionError>());
    }

    [Test]
    public void And_WhenOk_ReturnsOther()
    {
        Assert.That(Ok(1).And(Ok(2)).Unwrap(), Is.EqualTo(2));
    }

    [Test]
    public void And_WhenErr_PropagatesError()
    {
        Assert.That(Err("a").And(Ok(2)).Error!.Message, Is.EqualTo("a"));
    }

    [Test]
    public void AndNonGeneric_WhenErr_PropagatesError()
    {
        Assert.That(Err("a").And(Result.Ok()).Error!.Message, Is.EqualTo("a"));
    }

    [Test]
    public void AndThen_WhenOk_BindsValue()
    {
        Assert.That(Ok(2).AndThen(v => Result.Ok(v * 10)).Unwrap(), Is.EqualTo(20));
    }

    [Test]
    public void AndThen_WhenErr_PropagatesError()
    {
        Assert.That(Err("a").AndThen(v => Result.Ok(v * 10)).Error!.Message, Is.EqualTo("a"));
    }

    [Test]
    public void AndThenNonGeneric_WhenErr_PropagatesError()
    {
        Assert.That(Err("a").AndThen(v => Result.Ok()).Error!.Message, Is.EqualTo("a"));
    }

    [Test]
    public void Map_WhenOk_TransformsValue()
    {
        Assert.That(Ok(2).Map(v => v * 10).Unwrap(), Is.EqualTo(20));
    }

    [Test]
    public void Map_WhenErr_PropagatesError()
    {
        Assert.That(Err("a").Map(v => v * 10).Error!.Message, Is.EqualTo("a"));
    }

    [Test]
    public void MapErr_WhenErr_TransformsError()
    {
        Assert.That(
            Err("a").MapErr(e => new ResultError(e.Message + "!")).Error!.Message,
            Is.EqualTo("a!"));
    }

    [Test]
    public void MapErr_WhenOk_KeepsValue()
    {
        Assert.That(Ok(2).MapErr(e => new ResultError("x")).Unwrap(), Is.EqualTo(2));
    }

    [Test]
    public void MapOr_WhenOk_MapsValue()
    {
        Assert.That(Ok(2).MapOr(-1, v => v * 10), Is.EqualTo(20));
    }

    [Test]
    public void MapOr_WhenErr_UsesDefault()
    {
        Assert.That(Err("a").MapOr(-1, v => v * 10), Is.EqualTo(-1));
    }

    [Test]
    public void MapOrElse_WhenOk_MapsValue()
    {
        Assert.That(Ok(2).MapOrElse(v => v * 10, e => -1), Is.EqualTo(20));
    }

    [Test]
    public void MapOrElse_WhenErr_FallsBackToError()
    {
        Assert.That(Err("a").MapOrElse(v => v * 10, e => e.Message.Length), Is.EqualTo(1));
    }

    [Test]
    public void Match_WhenOk_InvokesOnOk()
    {
        Assert.That(Ok(2).Match(v => "v:" + v, e => "e:" + e.Message), Is.EqualTo("v:2"));
    }

    [Test]
    public void Match_WhenErr_InvokesOnErr()
    {
        Assert.That(Err("a").Match(v => "v:" + v, e => "e:" + e.Message), Is.EqualTo("e:a"));
    }

    [Test]
    public void Or_WhenErr_ReturnsAlternative()
    {
        Assert.That(Err("a").Or(Ok(2)).Unwrap(), Is.EqualTo(2));
    }

    [Test]
    public void OrElse_WhenErr_Recovers()
    {
        Assert.That(Err("a").OrElse(e => Ok(e.Message.Length)).Unwrap(), Is.EqualTo(1));
    }

    [Test]
    public void OrElse_WhenOk_ReturnsSelf()
    {
        Assert.That(Ok(5).OrElse(e => Ok(0)).Unwrap(), Is.EqualTo(5));
    }

    [Test]
    public void Unwrap_WhenOk_ReturnsValue()
    {
        Assert.That(Ok(2).Unwrap(), Is.EqualTo(2));
    }

    [Test]
    public void Unwrap_WhenErr_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => Err("a").Unwrap());
    }

    [Test]
    public void Expect_WhenOk_ReturnsValue()
    {
        Assert.That(Ok(2).Expect("want value"), Is.EqualTo(2));
    }

    [Test]
    public void Expect_WhenErr_ThrowsWithMessage()
    {
        InvalidOperationException? ex =
            Assert.Throws<InvalidOperationException>(() => Err("a").Expect("want value"));

        Assert.That(ex!.Message, Does.Contain("want value"));
    }

    [Test]
    public void UnwrapOr_WhenErr_ReturnsDefault()
    {
        Assert.That(Err("a").UnwrapOr(-1), Is.EqualTo(-1));
    }

    [Test]
    public void UnwrapOr_WhenOk_ReturnsValue()
    {
        Assert.That(Ok(2).UnwrapOr(-1), Is.EqualTo(2));
    }

    [Test]
    public void UnwrapOrDefault_WhenErr_ReturnsTypeDefault()
    {
        Assert.That(Err("a").UnwrapOrDefault(), Is.Zero);
    }

    [Test]
    public void UnwrapOrDefault_WhenOk_ReturnsValue()
    {
        Assert.That(Ok(2).UnwrapOrDefault(), Is.EqualTo(2));
    }

    [Test]
    public void UnwrapOrElse_WhenErr_ComputesFromError()
    {
        Assert.That(Err("a").UnwrapOrElse(e => e.Message.Length), Is.EqualTo(1));
    }

    [Test]
    public void UnwrapOrElse_WhenOk_ReturnsValue()
    {
        Assert.That(Ok(2).UnwrapOrElse(e => -1), Is.EqualTo(2));
    }

    [Test]
    public void UnwrapErr_WhenErr_ReturnsError()
    {
        Assert.That(Err("a").UnwrapErr().Message, Is.EqualTo("a"));
    }

    [Test]
    public void UnwrapErr_WhenOk_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => Ok(2).UnwrapErr());
    }

    [Test]
    public void ExpectErr_WhenErr_ReturnsError()
    {
        Assert.That(Err("a").ExpectErr("want err").Message, Is.EqualTo("a"));
    }

    [Test]
    public void ExpectErr_WhenOk_ThrowsWithMessage()
    {
        InvalidOperationException? ex =
            Assert.Throws<InvalidOperationException>(() => Ok(2).ExpectErr("want err"));

        Assert.That(ex!.Message, Does.Contain("want err"));
    }

    [Test]
    public void TryUnwrap_WhenOk_ReturnsTrueWithValue()
    {
        bool ok = Ok(2).TryUnwrap(out int value);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(value, Is.EqualTo(2));
        }
    }

    [Test]
    public void TryUnwrap_WhenErr_ReturnsFalse()
    {
        bool ok = Err("a").TryUnwrap(out int value);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.False);
            Assert.That(value, Is.Zero);
        }
    }

    [Test]
    public void TryUnwrapWithError_WhenErr_ReturnsFalseWithError()
    {
        bool ok = Err("a").TryUnwrap(out int value, out IResultError? error);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.False);
            Assert.That(value, Is.Zero);
            Assert.That(error!.Message, Is.EqualTo("a"));
        }
    }

    [Test]
    public void TryUnwrapWithError_WhenOk_ReturnsTrueWithoutError()
    {
        bool ok = Ok(2).TryUnwrap(out int value, out IResultError? error);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(value, Is.EqualTo(2));
            Assert.That(error, Is.Null);
        }
    }

    [Test]
    public void TryUnwrapErr_WhenErr_ReturnsTrueWithError()
    {
        bool ok = Err("a").TryUnwrapErr(out IResultError? error);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(error!.Message, Is.EqualTo("a"));
        }
    }

    [Test]
    public void Inspect_WhenOk_InvokesActionAndReturnsSelf()
    {
        int seen = 0;

        Result<int> returned = Ok(5).Inspect(v => seen = v);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(seen, Is.EqualTo(5));
            Assert.That(returned.Unwrap(), Is.EqualTo(5));
        }
    }

    [Test]
    public void InspectErr_WhenErr_InvokesAction()
    {
        string? seen = null;

        Err("a").InspectErr(e => seen = e.Message);

        Assert.That(seen, Is.EqualTo("a"));
    }

    [Test]
    public void AndThen_WhenOk_SatisfiesLeftIdentity()
    {
        static Result<int> f(int x)
        {
            return Result.Ok(x + 1);
        }

        Assert.That(Result.Ok(3).AndThen(f).Unwrap(), Is.EqualTo(f(3).Unwrap()));
    }

    [Test]
    public void AndThen_WithOk_SatisfiesRightIdentity()
    {
        Result<int> m = Result.Ok(3);

        Assert.That(m.AndThen(Result.Ok).Unwrap(), Is.EqualTo(m.Unwrap()));
    }

    [Test]
    public void AndThen_WhenChained_SatisfiesAssociativity()
    {
        static Result<int> f(int x)
        {
            return Result.Ok(x + 1);
        }

        static Result<int> g(int x)
        {
            return Result.Ok(x * 2);
        }

        Result<int> left = Result.Ok(3).AndThen(f).AndThen(g);
        Result<int> right = Result.Ok(3).AndThen(x => f(x).AndThen(g));

        Assert.That(left.Unwrap(), Is.EqualTo(right.Unwrap()));
    }
}
