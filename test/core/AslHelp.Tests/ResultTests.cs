using System;

using NUnit.Framework;

namespace AslHelp.Tests;

[TestFixture]
public class ResultTests
{
    [Test]
    public void Ok_ByDefault_IsOk()
    {
        Result result = Result.Ok();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsOk, Is.True);
            Assert.That(result.IsErr, Is.False);
            Assert.That(result.Error, Is.Null);
            Assert.That(result.ToString(), Is.EqualTo("Result.Ok()"));
        }
    }

    [Test]
    public void Err_WithMessage_IsErr()
    {
        Result result = Result.Err("boom");

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
        Assert.Throws<ArgumentNullException>(() => _ = Result.Err((IResultError)null!));
    }

    [Test]
    public void Err_WithNullMessage_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _ = Result.Err((string)null!));
    }

    [Test]
    public void Default_Always_IsOk()
    {
        Result result = default;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsOk, Is.True);
            Assert.That(result.IsErr, Is.False);
        }
    }

    [Test]
    public void ImplicitConversion_FromError_IsErr()
    {
        Result result = new ResultError("boom");

        Assert.That(result.IsErr, Is.True);
    }

    [Test]
    public void ImplicitConversion_FromException_WrapsInExceptionError()
    {
        Result result = new InvalidOperationException("boom");

        Assert.That(result.Error, Is.InstanceOf<ExceptionError>());
    }

    [Test]
    public void ImplicitConversion_FromNullException_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            Result _ = (Exception)null!;
        });
    }

    [Test]
    public void And_WhenOk_ReturnsOther()
    {
        Assert.That(Result.Ok().And(Result.Err("b")).Error!.Message, Is.EqualTo("b"));
    }

    [Test]
    public void And_WhenErr_ReturnsSelf()
    {
        Assert.That(Result.Err("a").And(Result.Ok()).Error!.Message, Is.EqualTo("a"));
    }

    [Test]
    public void AndGeneric_WhenOk_ReturnsOther()
    {
        Assert.That(Result.Ok().And(Result.Ok(5)).Unwrap(), Is.EqualTo(5));
    }

    [Test]
    public void AndGeneric_WhenErr_PropagatesError()
    {
        Assert.That(Result.Err("a").And(Result.Ok(5)).Error!.Message, Is.EqualTo("a"));
    }

    [Test]
    public void AndThen_WhenOk_InvokesContinuation()
    {
        Assert.That(Result.Ok().AndThen(() => Result.Err("b")).Error!.Message, Is.EqualTo("b"));
    }

    [Test]
    public void AndThen_WhenErr_DoesNotInvokeContinuation()
    {
        bool called = false;

        Result result = Result.Err("a").AndThen(() =>
        {
            called = true;
            return Result.Ok();
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(called, Is.False);
            Assert.That(result.Error!.Message, Is.EqualTo("a"));
        }
    }

    [Test]
    public void AndThenGeneric_WhenOk_InvokesContinuation()
    {
        Assert.That(Result.Ok().AndThen(() => Result.Ok(5)).Unwrap(), Is.EqualTo(5));
    }

    [Test]
    public void AndThenGeneric_WhenErr_PropagatesError()
    {
        Assert.That(Result.Err("a").AndThen(() => Result.Ok(5)).Error!.Message, Is.EqualTo("a"));
    }

    [Test]
    public void AndThen_WithNullContinuation_IsValidatedOnlyWhenOk()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ArgumentNullException>(() => Result.Ok().AndThen(null!));
            Assert.DoesNotThrow(() => Result.Err("a").AndThen(null!));
        }
    }

    [Test]
    public void Map_WhenOk_LiftsValue()
    {
        Assert.That(Result.Ok().Map(42).Unwrap(), Is.EqualTo(42));
    }

    [Test]
    public void Map_WhenErr_PropagatesError()
    {
        Assert.That(Result.Err("a").Map(42).Error!.Message, Is.EqualTo("a"));
    }

    [Test]
    public void MapErr_WhenErr_TransformsError()
    {
        Assert.That(
            Result.Err("a").MapErr(e => new ResultError(e.Message + "!")).Error!.Message,
            Is.EqualTo("a!"));
    }

    [Test]
    public void MapErr_WhenOk_DoesNotInvokeFn()
    {
        bool called = false;

        Result result = Result.Ok().MapErr(e =>
        {
            called = true;
            return new ResultError("x");
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsOk, Is.True);
            Assert.That(called, Is.False);
        }
    }

    [Test]
    public void Or_WhenOk_ReturnsSelf()
    {
        Assert.That(Result.Ok().Or(Result.Err("b")).IsOk, Is.True);
    }

    [Test]
    public void Or_WhenErr_ReturnsAlternative()
    {
        Assert.That(Result.Err("a").Or(Result.Ok()).IsOk, Is.True);
    }

    [Test]
    public void OrElse_WhenOk_DoesNotInvokeFn()
    {
        bool called = false;

        Result result = Result.Ok().OrElse(e =>
        {
            called = true;
            return Result.Err("b");
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(called, Is.False);
            Assert.That(result.IsOk, Is.True);
        }
    }

    [Test]
    public void OrElse_WhenErr_Recovers()
    {
        Assert.That(Result.Err("a").OrElse(e => Result.Ok()).IsOk, Is.True);
    }

    [Test]
    public void Match_WhenOk_InvokesOnOk()
    {
        Assert.That(Result.Ok().Match(() => "ok", e => "err:" + e.Message), Is.EqualTo("ok"));
    }

    [Test]
    public void Match_WhenErr_InvokesOnErr()
    {
        Assert.That(Result.Err("a").Match(() => "ok", e => "err:" + e.Message), Is.EqualTo("err:a"));
    }

    [Test]
    public void Unwrap_WhenOk_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => Result.Ok().Unwrap());
    }

    [Test]
    public void Unwrap_WhenErr_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => Result.Err("a").Unwrap());
    }

    [Test]
    public void Expect_WhenOk_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => Result.Ok().Expect("should be ok"));
    }

    [Test]
    public void Expect_WhenErr_ThrowsWithMessage()
    {
        InvalidOperationException? ex =
            Assert.Throws<InvalidOperationException>(() => Result.Err("a").Expect("custom message"));

        Assert.That(ex!.Message, Does.Contain("custom message"));
    }

    [Test]
    public void UnwrapErr_WhenErr_ReturnsError()
    {
        Assert.That(Result.Err("a").UnwrapErr().Message, Is.EqualTo("a"));
    }

    [Test]
    public void UnwrapErr_WhenOk_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => Result.Ok().UnwrapErr());
    }

    [Test]
    public void ExpectErr_WhenErr_ReturnsError()
    {
        Assert.That(Result.Err("a").ExpectErr("want error").Message, Is.EqualTo("a"));
    }

    [Test]
    public void ExpectErr_WhenOk_ThrowsWithMessage()
    {
        InvalidOperationException? ex =
            Assert.Throws<InvalidOperationException>(() => Result.Ok().ExpectErr("want error"));

        Assert.That(ex!.Message, Does.Contain("want error"));
    }

    [Test]
    public void TryUnwrapErr_WhenErr_ReturnsTrueWithError()
    {
        bool ok = Result.Err("a").TryUnwrapErr(out IResultError? error);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.True);
            Assert.That(error!.Message, Is.EqualTo("a"));
        }
    }

    [Test]
    public void TryUnwrapErr_WhenOk_ReturnsFalse()
    {
        bool ok = Result.Ok().TryUnwrapErr(out IResultError? error);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ok, Is.False);
            Assert.That(error, Is.Null);
        }
    }

    [Test]
    public void Inspect_WhenOk_InvokesActionAndReturnsSelf()
    {
        int count = 0;

        Result returned = Result.Ok().Inspect(() => count++);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(returned.IsOk, Is.True);
        }
    }

    [Test]
    public void Inspect_WhenErr_DoesNotInvokeAction()
    {
        int count = 0;

        Result.Err("a").Inspect(() => count++);

        Assert.That(count, Is.Zero);
    }

    [Test]
    public void InspectErr_WhenErr_InvokesActionAndReturnsSelf()
    {
        string? seen = null;

        Result returned = Result.Err("a").InspectErr(e => seen = e.Message);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(seen, Is.EqualTo("a"));
            Assert.That(returned.IsErr, Is.True);
        }
    }

    [Test]
    public void InspectErr_WhenOk_DoesNotInvokeAction()
    {
        bool called = false;

        Result.Ok().InspectErr(e => called = true);

        Assert.That(called, Is.False);
    }
}
